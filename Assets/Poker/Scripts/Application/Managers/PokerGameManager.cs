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
        
        _turnManager = new TurnManager(_snapshot);
        _turnManager.StartTurn();
        
        _potService = new PotService();
        _betService = new BetService(_potService);

        EventManager.Instance.TriggerEvent(GameEvents.STATE_CHANGED, _snapshot);
        EventManager.Instance.Subscribe(GameEvents.PLAYER_ACTION, OnPlayerAction);
        EventManager.Instance.Subscribe(GameEvents.BETTING_ROUND_COMPLETE, OnBettingRoundComplete);
    }
    
    private void OnPlayerAction(object data)
    {
        var action = (PlayerAction)data;

        _betService.ProcessAction(action);

        EventManager.Instance.TriggerEvent(GameEvents.POT_UPDATED, _potService.Pot);

        _turnManager.EndTurn();
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
                RestartMatch();
                break;
        }

        _turnManager.StartTurn();
    }
    
    private void ResolveShowdown()
    {
        var winner = _snapshot.Players[0]; // replace with evaluator later

        _potService.DistributeToWinner(winner);

        EventManager.Instance.TriggerEvent(GameEvents.POT_UPDATED, _potService.Pot);
    }
    
    private void RestartMatch()
    {
        StartMatch();

        EventManager.Instance.TriggerEvent(GameEvents.MATCH_RESTART, null);
    }
    
    
}