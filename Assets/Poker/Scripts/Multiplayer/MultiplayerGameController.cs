using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Poker.Core.Models;
using Poker.Core.Enums;

public class MultiplayerGameController : NetworkBehaviour
{
    private GameSnapshot _snapshot;
    private GameStateController _stateController;
    private TurnManager _turnManager;

    private DeckService _deck;
    private DealService _deal;
    private BetService _bet;
    private PotService _pot;
    private HandEvaluator _eval;

    private readonly Dictionary<ulong, Player> _clientPlayers = new();

    public System.Action<GameSnapshot> OnSnapshot;

    #region Network lifecycle

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _snapshot = new GameSnapshot
        {
            Players = new(),
            Pot = 0,
            CurrentPlayerIndex = 0,
            Round = PokerRound.PreFlop
        };

        NetworkManager.OnClientConnectedCallback += OnClientConnected;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
    }

    #endregion

    #region Player Mapping

    void OnClientConnected(ulong clientId)
    {
        if (_clientPlayers.Count >= 2) return;

        var player = new Player(
            clientId.ToString(),
            $"Player {_clientPlayers.Count + 1}",
            1000,
            false
        );

        _snapshot.Players.Add(player);
        _clientPlayers[clientId] = player;

        Debug.Log($"Client {clientId} mapped to {player.Name}");

        if (_clientPlayers.Count == 2)
            StartMatch();
    }

    #endregion

    #region Match Lifecycle

    void StartMatch()
    {
        _deck = new DeckService();
        _deck.Initialize();

        _deal = new DealService(_deck);
        DealServiceLocator.DealService = _deal;

        _pot = new PotService();
        _bet = new BetService(_pot);
        _eval = new HandEvaluator();

        _stateController = new GameStateController(_snapshot);
        _turnManager = new TurnManager(_snapshot);

        EventManager.Instance.Subscribe(GameEvents.BETTING_ROUND_COMPLETE, OnBettingRoundComplete);

        _stateController.ChangeState(new PreFlopState());
        _turnManager.StartTurn();

        EmitSnapshot();
    }

    void RestartMatch()
    {
        _snapshot.Round = PokerRound.PreFlop;
        _snapshot.CurrentPlayerIndex = 0;
        _snapshot.Pot = 0;

        _deck.Initialize();

        _stateController.ChangeState(new PreFlopState());
        _turnManager.StartTurn();

        EmitSnapshot();
    }

    #endregion

    #region Gameplay

    [ServerRpc(RequireOwnership = false)]
    public void SendActionServerRpc(int actionType, int amount, ServerRpcParams rpc = default)
    {
        var clientId = rpc.Receive.SenderClientId;

        if (!_clientPlayers.ContainsKey(clientId)) return;

        var player = _clientPlayers[clientId];

        // server authority validation
        if (_snapshot.Players[_snapshot.CurrentPlayerIndex] != player)
            return;

        var action = new PlayerAction(player, (ActionType)actionType, amount);
        ProcessAction(action);
    }

    void ProcessAction(PlayerAction action)
    {
        _bet.ProcessAction(action);

        _turnManager.EndTurn();

        EmitSnapshot();
    }

    void OnBettingRoundComplete(object _)
    {
        _turnManager.ResetRoundCounter();
        AdvanceRound();
    }

    void AdvanceRound()
    {
        switch (_snapshot.Round)
        {
            case PokerRound.PreFlop:
                _stateController.ChangeState(new FlopState());
                break;

            case PokerRound.Flop:
                _stateController.ChangeState(new TurnState());
                break;

            case PokerRound.Turn:
                _stateController.ChangeState(new RiverState());
                break;

            case PokerRound.River:
                _stateController.ChangeState(new ShowdownState());
                break;

            case PokerRound.Showdown:
                ResolveShowdown();
                RestartMatch();
                return;
        }

        _turnManager.StartTurn();
        EmitSnapshot();
    }

    void ResolveShowdown()
    {
        var winner = _eval.DetermineWinner(_snapshot);

        _pot.DistributeToWinner(winner);

        BroadcastWinnerClientRpc(winner.Id);

        EmitSnapshot();
    }

    #endregion

    #region Snapshot Sync

    void EmitSnapshot()
    {
        OnSnapshot?.Invoke(_snapshot);

        var json = JsonUtility.ToJson(_snapshot);
        BroadcastSnapshotClientRpc(json);
    }

    [ClientRpc]
    void BroadcastSnapshotClientRpc(string json)
    {
        var snapshot = JsonUtility.FromJson<GameSnapshot>(json);
        EventManager.Instance.TriggerEvent(GameEvents.STATE_CHANGED, snapshot);
    }

    [ClientRpc]
    void BroadcastWinnerClientRpc(string winnerId)
    {
        EventManager.Instance.TriggerEvent(GameEvents.SHOWDOWN_RESULT, winnerId);
    }

    #endregion
}