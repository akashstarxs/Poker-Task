using UnityEngine;
using TMPro;
using System.Linq;
using Unity.Netcode;
using Poker.Core.Models;
using Poker.Core.Enums;

public class MultiplayerUI : MonoBehaviour
{

    [SerializeField] MultiplayerGameController controller;

   
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

    
    void OnSnapshot(object data)
    {
        _snapshot = (GameSnapshot)data;

        roundText.text = $"Round: {_snapshot.Round}";
        potText.text = $"Pot: {_snapshot.Pot}";

        RenderCommunity();
        RenderHands();
        UpdateTurnUI();
    }


    void UpdateTurnUI()
    {
        if (_snapshot.Players.Count == 0) return;

        var myId = NetworkManager.Singleton.LocalClientId.ToString();

        var isMyTurn =
            _snapshot.Players[_snapshot.CurrentPlayerIndex].Id == myId;

        buttonsPanel.SetActive(isMyTurn);
    }



    public void Bet() => Send(ActionType.Bet, 50);
    public void Call() => Send(ActionType.Call, 20);
    public void Check() => Send(ActionType.Check, 0);
    public void Fold() => Send(ActionType.Fold, 0);

    void Send(ActionType type, int amount)
    {
        controller.SendActionServerRpc((int)type, amount);
    }



    void RenderCommunity()
    {
        if (_snapshot == null) return;
        if (_snapshot.CommunityCards == null) return;

        Clear(communityPanel);

        foreach (var card in _snapshot.CommunityCards)
        {
            var v = Instantiate(cardPrefab, communityPanel);
            v.Bind(card);
        }
    }

    void RenderHands()
    {
        if (_snapshot == null) return;
        if (_snapshot.PlayerHands == null) return;
        if (_snapshot.Players == null || _snapshot.Players.Count == 0) return;

        Clear(playerPanel);
        Clear(opponentPanel);

        var player = _snapshot.Players.FirstOrDefault(p => !p.IsAI);
        var ai = _snapshot.Players.FirstOrDefault(p => p.IsAI);

        if (player != null &&
            _snapshot.PlayerHands.ContainsKey(player.Id) &&
            _snapshot.PlayerHands[player.Id] != null)
        {
            foreach (var card in _snapshot.PlayerHands[player.Id])
            {
                var v = Instantiate(cardPrefab, playerPanel);
                v.Bind(card);
            }
        }

        if (ai != null &&
            _snapshot.PlayerHands.ContainsKey(ai.Id) &&
            _snapshot.PlayerHands[ai.Id] != null)
        {
            foreach (var card in _snapshot.PlayerHands[ai.Id])
            {
                var v = Instantiate(cardPrefab, opponentPanel);
                v.Bind(card);
            }
        }
    }

    void Clear(Transform t)
    {
        foreach (Transform c in t)
            Destroy(c.gameObject);
    }
}