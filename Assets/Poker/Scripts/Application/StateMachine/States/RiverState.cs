using Poker.Core.Enums;
using Poker.Core.Models;

public class RiverState : IGameState
{
    public void Enter(GameSnapshot snapshot)
    {
        snapshot.Round = PokerRound.River;
        UnityEngine.Debug.Log("Entered River");
    }

    public void Exit(GameSnapshot snapshot) { }
}