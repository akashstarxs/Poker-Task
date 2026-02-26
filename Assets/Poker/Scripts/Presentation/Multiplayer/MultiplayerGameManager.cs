using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Poker.Core.Models;

public class MultiplayerGameManager : NetworkBehaviour
{
    [SerializeField] PokerGameManager game; 
    private Dictionary<ulong, Player> _clientPlayers = new();
    private int _maxPlayers = 2;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        game.OnSnapshotChanged += OnGameSnapshotChanged;

        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        
        if (IsServer)
        {
            game.OnSnapshotChanged += OnGameSnapshotChanged;
            EventManager.Instance.Subscribe(GameEvents.TURN_STARTED, OnTurnStartedServer);
        }
    }
    void OnGameSnapshotChanged(GameSnapshot snapshot)
    {
        BroadcastSnapshot(snapshot);
    }

    void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;
        if (_clientPlayers.Count >= _maxPlayers) return;

       
        var player = new Player(
            clientId.ToString(),
            $"Player {_clientPlayers.Count + 1}",
            1000,
            false
        );

        _clientPlayers[clientId] = player;

        // register player inside existing game manager
        game.RegisterNetworkPlayer(player);

        Debug.Log($"Mapped client {clientId} → {player.Name}");

        // start match when ready
        if (_clientPlayers.Count == _maxPlayers)
            game.StartMatch();
    }
    void OnTurnStartedServer(object data)
    {
        var player = (Player)data;

        // find owning client
        foreach (var kv in _clientPlayers)
        {
            if (kv.Value == player)
            {
                BroadcastTurnOwnerClientRpc(kv.Key);
                break;
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SendActionServerRpc(int actionType, int amount, ServerRpcParams rpc = default)
    {
        var clientId = rpc.Receive.SenderClientId;

        if (!_clientPlayers.ContainsKey(clientId)) return;

        var player = _clientPlayers[clientId];

        game.ReceiveNetworkAction(player, actionType, amount);
    }
    
    void BroadcastSnapshot(GameSnapshot snapshot)
    {
        var json = JsonUtility.ToJson(snapshot);
        BroadcastSnapshotClientRpc(json);
    }
    [ClientRpc]
    void BroadcastSnapshotClientRpc(string json)
    {
        var snapshot = JsonUtility.FromJson<GameSnapshot>(json);

        // reuse existing UI pipeline
        EventManager.Instance.TriggerEvent(GameEvents.STATE_CHANGED, snapshot);
    }
    [ClientRpc]
    void BroadcastTurnOwnerClientRpc(ulong clientId)
    {
        EventManager.Instance.TriggerEvent(GameEvents.TURN_OWNER_CHANGED, clientId);
    }
}