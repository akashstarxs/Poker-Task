using Poker.Core.Enums;
using Poker.Core.Models;

public class PreFlopState : IGameState
{
    public void Enter(GameSnapshot snapshot)
    {
        snapshot.Round = PokerRound.PreFlop;

        // Deal cards 
        // Start betting

        UnityEngine.Debug.Log("Entered PreFlop");
    }

    public void Exit(GameSnapshot snapshot)
    {
    }
}