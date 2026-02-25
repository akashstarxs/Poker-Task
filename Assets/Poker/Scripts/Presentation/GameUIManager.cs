using System.Collections;
using TMPro;
using UnityEngine;
using Poker.Core.Models;
using System.Linq;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text roundText;
    [SerializeField] TMP_Text potText;
    [SerializeField] TMP_Text communityText;
    [SerializeField] TMP_Text handText;
    [SerializeField] GameObject buttonsPanel;
    
    [SerializeField] Transform communityPanel;
    [SerializeField] Transform aiPanel;
    [SerializeField] Transform playerPanel;
    [SerializeField] CardView cardPrefab;
    
    
    [SerializeField] TMP_Text WinnerText;
    [SerializeField] GameObject WinnerPanel;

    private GameSnapshot _snapshot;
    private Player _current;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvents.STATE_CHANGED, OnState);
        EventManager.Instance.Subscribe(GameEvents.TURN_STARTED, OnTurn);
        EventManager.Instance.Subscribe(GameEvents.POT_UPDATED, OnPot);
        EventManager.Instance.Subscribe(GameEvents.SHOWDOWN_RESULT, OnWinner);
    }

    private void OnDisable()
    {
        if(EventManager.Instance == null)return;
        EventManager.Instance.Unsubscribe(GameEvents.STATE_CHANGED, OnState);
        EventManager.Instance.Unsubscribe(GameEvents.TURN_STARTED, OnTurn);
        EventManager.Instance.Unsubscribe(GameEvents.POT_UPDATED, OnPot);
        EventManager.Instance.Unsubscribe(GameEvents.SHOWDOWN_RESULT, OnWinner);
    }
    
    void OnWinner(object data)
    {
        var winner = (Player)data;
        WinnerPanel.SetActive(true);
        WinnerText.text = $"{winner.Name} wins!";
        StartCoroutine( HideWinner());
    }
    
    IEnumerator HideWinner()
    {
        yield return new WaitForSeconds(3f);
        WinnerPanel.SetActive(false);
        WinnerText.text = string.Empty;
    }

    private void OnState(object data)
    {
        _snapshot = (GameSnapshot)data;

        roundText.text = $"Round: {_snapshot.Round}";

        // communityText.text =
        //     "Community:\n" +
        //     string.Join("\n", _snapshot.CommunityCards.Select(c => c.ToString()));

        var player = _snapshot.Players[0];

        // handText.text =
        //     "Your Hand:\n" +
        //     string.Join("\n", _snapshot.PlayerHands[player.Id].Select(c => c.ToString()));

        RenderCommunity();
        RenderHands();
    }

    private void OnTurn(object data)
    {
        _current = (Player)data;
        buttonsPanel.SetActive(!_current.IsAI);
    }

    private void OnPot(object data)
    {
        potText.text = $"Pot: {data}";
    }

    public void Bet() => Send(ActionType.Bet, 50);
    public void Call() => Send(ActionType.Call, 20);
    public void Check() => Send(ActionType.Check, 0);
    public void Fold() => Send(ActionType.Fold, 0);

    private void Send(ActionType type, int amount)
    {
        EventManager.Instance.TriggerEvent(
            GameEvents.PLAYER_ACTION,
            new PlayerAction(_current, type, amount)
        );
    }
    
    
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
        Clear(aiPanel);

        var player = _snapshot.Players.Find(p => !p.IsAI);
        var ai = _snapshot.Players.Find(p => p.IsAI);

        foreach (var card in _snapshot.PlayerHands[player.Id])
        {
            var v = Instantiate(cardPrefab, playerPanel);
            v.Bind(card);
        }

        foreach (var card in _snapshot.PlayerHands[ai.Id])
        {
            var v = Instantiate(cardPrefab, aiPanel);
            v.Bind(card);
        }
    }

    void Clear(Transform t)
    {
        foreach (Transform c in t)
            Destroy(c.gameObject);
    }
}