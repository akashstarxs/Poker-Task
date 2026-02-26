using System.Collections;
using Poker.Core.Models;
using Poker.Core.Enums;
using UnityEngine;

public class PokerGameManager : MonoBehaviour
{
    private GameSnapshot _snapshot;
    private GameStateController _stateController;
    private TurnManager _turnManager;
    private PotService _potService;
    private BetService _betService;
    private DeckService _deckService;
    private DealService _dealService;
    private HandEvaluator _handEvaluator;
    private AIDecisionService _ai;

    public System.Action<GameSnapshot> OnSnapshotChanged;

    void Start()
    {
        SubscribeEvents();
        StartMatch();
    }

    public void StartMatch()
    {
        // If multiplayer injected players → keep them
        if (_snapshot == null)
        {
            _snapshot = new GameSnapshot
            {
                Players = new(),
                Round = PokerRound.PreFlop,
                CurrentPlayerIndex = 0,
                Pot = 0
            };
        }

        // Single player fallback
        if (_snapshot.Players == null || _snapshot.Players.Count == 0)
        {
            _snapshot.Players = new()
            {
                new Player("1","Player",1000,false),
                new Player("2","AI",1000,true)
            };
        }

        _snapshot.Round = PokerRound.PreFlop;
        _snapshot.CurrentPlayerIndex = 0;
        _snapshot.Pot = 0;

        _deckService = new DeckService();
        _deckService.Initialize();

        _dealService = new DealService(_deckService);
        DealServiceLocator.DealService = _dealService;

        _potService = new PotService();
        _betService = new BetService(_potService);
        _handEvaluator = new HandEvaluator();

        // Only create AI if any AI player exists
        if (_snapshot.Players.Exists(p => p.IsAI))
            _ai = new AIDecisionService(_handEvaluator);

        _stateController = new GameStateController(_snapshot);
        _turnManager = new TurnManager(_snapshot);

        _stateController.ChangeState(new PreFlopState());
        _turnManager.StartTurn();

        EmitSnapshot();
    }

    private void SubscribeEvents()
    {
        EventManager.Instance.Subscribe(GameEvents.TURN_STARTED, OnTurnStarted);
        EventManager.Instance.Subscribe(GameEvents.PLAYER_ACTION, OnPlayerAction);
        EventManager.Instance.Subscribe(GameEvents.BETTING_ROUND_COMPLETE, OnBettingRoundComplete);
    }

    private void EmitSnapshot()
    {
        OnSnapshotChanged?.Invoke(_snapshot);
        EventManager.Instance.TriggerEvent(GameEvents.STATE_CHANGED, _snapshot);
    }

    private void OnPlayerAction(object data)
    {
        var action = (PlayerAction)data;

        _betService.ProcessAction(action);

        EventManager.Instance.TriggerEvent(GameEvents.POT_UPDATED, _potService.Pot);

        _turnManager.EndTurn();

        EmitSnapshot();
    }

    private void OnBettingRoundComplete(object _)
    {
        _turnManager.ResetRoundCounter();
        AdvanceRound();
    }

    private void AdvanceRound()
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

    private void ResolveShowdown()
    {
        var winner = _handEvaluator.DetermineWinner(_snapshot);

        _potService.DistributeToWinner(winner);

        EventManager.Instance.TriggerEvent(GameEvents.POT_UPDATED, _potService.Pot);
        EventManager.Instance.TriggerEvent(GameEvents.SHOWDOWN_RESULT, winner);

        EmitSnapshot();
    }

    private void RestartMatch()
    {
        StartMatch();
        EventManager.Instance.TriggerEvent(GameEvents.MATCH_RESTART, null);
    }

    private void OnTurnStarted(object data)
    {
        var player = (Player)data;

        if (_ai == null || !player.IsAI) return;

        var action = _ai.Decide(_snapshot, player);
        StartCoroutine(ExecuteAI(action));
    }

    private IEnumerator ExecuteAI(PlayerAction action)
    {
        yield return new WaitForSeconds(1f);
        EventManager.Instance.TriggerEvent(GameEvents.PLAYER_ACTION, action);
    }

    // =========================
    // Multiplayer Extensions
    // =========================

    public void RegisterNetworkPlayer(Player player)
    {
        if (_snapshot == null)
        {
            _snapshot = new GameSnapshot
            {
                Players = new(),
                CurrentPlayerIndex = 0,
                Pot = 0
            };
        }

        if (!_snapshot.Players.Exists(p => p.Id == player.Id))
            _snapshot.Players.Add(player);
    }

    public void ReceiveNetworkAction(Player player, int actionType, int amount)
    {
        if (_snapshot.Players[_snapshot.CurrentPlayerIndex] != player)
            return;

        var action = new PlayerAction(player, (ActionType)actionType, amount);
        OnPlayerAction(action);
    }
}