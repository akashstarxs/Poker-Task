using UnityEngine;
using TMPro;
using System.Linq;
using Unity.Netcode;
using Poker.Core.Models;
using Poker.Core.Enums;

public class MultiplayerUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] MultiplayerGameController controller;

    [Header("UI")]
    [SerializeField] TMP_Text roundText;
    [SerializeField] TMP_Text potText;

    [SerializeField] Transform communityPanel;
    [SerializeField] Transform playerPanel;
    [SerializeField] Transform opponentPanel;
    [SerializeField] CardView cardPrefab;

    [SerializeField] GameObject buttonsPanel;

    private GameSnapshot _snapshot;

    void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvents.STATE_CHANGED, OnSnapshot);
    }

    void OnDisable()
    {
        if (EventManager.Instance == null) return;
        EventManager.Instance.Unsubscribe(GameEvents.STATE_CHANGED, OnSnapshot);
    }

    // =========================
    // SNAPSHOT
    // =========================

    void OnSnapshot(object data)
    {
        _snapshot = (GameSnapshot)data;

        roundText.text = $"Round: {_snapshot.Round}";
        potText.text = $"Pot: {_snapshot.Pot}";

        RenderCommunity();
        RenderHands();
        UpdateTurnUI();
    }

    // =========================
    // TURN OWNERSHIP (minimal)
    // =========================

    void UpdateTurnUI()
    {
        if (_snapshot.Players.Count == 0) return;

        var myId = NetworkManager.Singleton.LocalClientId.ToString();

        var isMyTurn =
            _snapshot.Players[_snapshot.CurrentPlayerIndex].Id == myId;

        buttonsPanel.SetActive(isMyTurn);
    }

    // =========================
    // ACTIONS
    // =========================

    public void Bet() => Send(ActionType.Bet, 50);
    public void Call() => Send(ActionType.Call, 20);
    public void Check() => Send(ActionType.Check, 0);
    public void Fold() => Send(ActionType.Fold, 0);

    void Send(ActionType type, int amount)
    {
        controller.SendActionServerRpc((int)type, amount);
    }

    // =========================
    // RENDERING
    // =========================

    void RenderCommunity()
    {
        Clear(communityPanel);

        foreach (var card in _snapshot.CommunityCards)
        {
            var v = Instantiate(cardPrefab, communityPanel);
            v.Bind(card);
        }
    }

    void RenderHands()
    {
        Clear(playerPanel);
        Clear(opponentPanel);

        var myId = NetworkManager.Singleton.LocalClientId.ToString();

        var me = _snapshot.Players.FirstOrDefault(p => p.Id == myId);
        var opp = _snapshot.Players.FirstOrDefault(p => p.Id != myId);

        if (me != null && _snapshot.PlayerHands.ContainsKey(me.Id))
        {
            foreach (var c in _snapshot.PlayerHands[me.Id])
            {
                var v = Instantiate(cardPrefab, playerPanel);
                v.Bind(c);
            }
        }

        // opponent hidden until showdown
        if (_snapshot.Round == PokerRound.Showdown && opp != null &&
            _snapshot.PlayerHands.ContainsKey(opp.Id))
        {
            foreach (var c in _snapshot.PlayerHands[opp.Id])
            {
                var v = Instantiate(cardPrefab, opponentPanel);
                v.Bind(c);
            }
        }
    }

    void Clear(Transform t)
    {
        foreach (Transform c in t)
            Destroy(c.gameObject);
    }
}