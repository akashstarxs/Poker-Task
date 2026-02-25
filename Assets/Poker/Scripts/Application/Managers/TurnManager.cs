using Poker.Core.Models;

public class TurnManager
{
    private readonly GameSnapshot _snapshot;

    public TurnManager(GameSnapshot snapshot)
    {
        _snapshot = snapshot;
    }

    public Player GetCurrentPlayer()
    {
        return _snapshot.Players[_snapshot.CurrentPlayerIndex];
    }

    public void StartTurn()
    {
        var player = GetCurrentPlayer();

        EventManager.Instance.TriggerEvent(
            GameEvents.TURN_STARTED,
            player
        );
    }

    public void EndTurn()
    {
        MoveNext();
        StartTurn();
    }

    private void MoveNext()
    {
        _snapshot.CurrentPlayerIndex++;

        if (_snapshot.CurrentPlayerIndex >= _snapshot.Players.Count)
            _snapshot.CurrentPlayerIndex = 0;
    }
}