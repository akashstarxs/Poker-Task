using Poker.Core.Models;

public interface IGameState
{
    void Enter(GameSnapshot snapshot);
    void Exit(GameSnapshot snapshot);
}