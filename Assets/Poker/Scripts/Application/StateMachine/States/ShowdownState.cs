using Poker.Core.Enums;
using Poker.Core.Models;

public class ShowdownState : IGameState
{
    public void Enter(GameSnapshot snapshot)
    {
        snapshot.Round = PokerRound.Showdown;
        UnityEngine.Debug.Log("Entered Showdown");
    }

    public void Exit(GameSnapshot snapshot) { }
}