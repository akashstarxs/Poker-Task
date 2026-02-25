using Poker.Core.Enums;
using Poker.Core.Models;

public class FlopState : IGameState
{
    public void Enter(GameSnapshot snapshot)
    {
        snapshot.Round = PokerRound.Flop;
        DealServiceLocator.DealService.DealFlop(snapshot);
        UnityEngine.Debug.Log("Entered Flop");
    }

    public void Exit(GameSnapshot snapshot) { }
}