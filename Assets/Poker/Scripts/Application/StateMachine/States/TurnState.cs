using Poker.Core.Enums;
using Poker.Core.Models;

public class TurnState : IGameState
{
    public void Enter(GameSnapshot snapshot)
    {
        snapshot.Round = PokerRound.Turn;
        UnityEngine.Debug.Log("Entered Turn");
    }

    public void Exit(GameSnapshot snapshot) { }
}