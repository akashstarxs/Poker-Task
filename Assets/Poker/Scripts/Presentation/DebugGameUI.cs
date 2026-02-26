using UnityEngine;
using Poker.Core.Models;

public class DebugGameUI : MonoBehaviour
{
    private GameSnapshot _snapshot;
    private Player _currentPlayer;
    private int _pot;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvents.STATE_CHANGED, OnStateChanged);
        EventManager.Instance.Subscribe(GameEvents.TURN_STARTED, OnTurnStarted);
        EventManager.Instance.Subscribe(GameEvents.POT_UPDATED, OnPotUpdated);
    }

    private void OnDisable()
    {
        if(EventManager.Instance == null)
            return;
        EventManager.Instance.Unsubscribe(GameEvents.STATE_CHANGED, OnStateChanged);
        EventManager.Instance.Unsubscribe(GameEvents.TURN_STARTED, OnTurnStarted);
        EventManager.Instance.Unsubscribe(GameEvents.POT_UPDATED, OnPotUpdated);
    }

    private void OnStateChanged(object data)
    {
        _snapshot = (GameSnapshot)data;
    }

    private void OnTurnStarted(object data)
    {
        _currentPlayer = (Player)data;
    }

    private void OnPotUpdated(object data)
    {
        _pot = (int)data;
    }

    private void OnGUI()
    {
        if (_snapshot == null) return;

        GUILayout.Label($"ROUND: {_snapshot.Round}");
        GUILayout.Label($"POT: {_pot}");
        GUILayout.Space(10);

        GUILayout.Label("COMMUNITY:");
        foreach (var c in _snapshot.CommunityCards)
            GUILayout.Label(c.ToString());

        GUILayout.Space(10);

        GUILayout.Label("PLAYERS:");
        foreach (var p in _snapshot.Players)
        {
            GUILayout.Label($"{p.Name} — Chips: {p.Chips}");

            if (_snapshot.PlayerHands.TryGetValue(p.Id, out var hand))
            {
                foreach (var card in hand)
                    GUILayout.Label($"  {card}");
            }
        }

        GUILayout.Space(20);

        if (_currentPlayer != null && !_currentPlayer.IsAI)
        {
            GUILayout.Label($"YOUR TURN: {_currentPlayer.Name}");

            if (GUILayout.Button("BET"))
                Send(ActionType.Bet, 50);

            if (GUILayout.Button("CALL"))
                Send(ActionType.Call, 20);

            if (GUILayout.Button("CHECK"))
                Send(ActionType.Check, 0);

            if (GUILayout.Button("FOLD"))
                Send(ActionType.Fold, 0);
        }
    }

    private void Send(ActionType type, int amount)
    {
        EventManager.Instance.TriggerEvent(
            GameEvents.PLAYER_ACTION,
            new PlayerAction(_currentPlayer, type, amount)
        );
    }
}