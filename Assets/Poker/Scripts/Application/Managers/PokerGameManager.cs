using Poker.Core.Models;
using Poker.Core.Enums;
using UnityEngine;

public class PokerGameManager : MonoBehaviour
{
    private GameSnapshot _snapshot;
    private GameStateController _stateController;

    void Start()
    {
        StartMatch();
    }

    public void StartMatch()
    {
        _stateController = new GameStateController(_snapshot);
        _stateController.ChangeState(new PreFlopState());
        _snapshot = new GameSnapshot
        {
            Players = new()
            {
                new Player("1","Player",1000,false),
                new Player("2","AI",1000,true)
            },
            Round = PokerRound.PreFlop,
            CurrentPlayerIndex = 0,
            Pot = 0
        };

        EventManager.Instance.TriggerEvent(GameEvents.STATE_CHANGED, _snapshot);
    }
}