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
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
        }
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
    
    
    [ServerRpc(RequireOwnership = false)]
    public void SendActionServerRpc(int actionType, int amount, ServerRpcParams rpc = default)
    {
        var clientId = rpc.Receive.SenderClientId;

        if (!_clientPlayers.ContainsKey(clientId)) return;

        var player = _clientPlayers[clientId];

        game.ReceiveNetworkAction(player, actionType, amount);
    }
}