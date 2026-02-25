using Poker.Core.Enums;
using Poker.Core.Models;

public class TurnState : IGameState
{
    public void Enter(GameSnapshot snapshot)
    {
        snapshot.Round = PokerRound.Turn;
        DealServiceLocator.DealService.DealTurn(snapshot);
        UnityEngine.Debug.Log("Entered Turn");
    }

    public void Exit(GameSnapshot snapshot) { }
}