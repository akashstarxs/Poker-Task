using Poker.Core.Models;

public class TurnManager
{
    private readonly GameSnapshot _snapshot;
    private int _actionsThisRound;

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
    

    private void MoveNext()
    {
        _snapshot.CurrentPlayerIndex++;

        if (_snapshot.CurrentPlayerIndex >= _snapshot.Players.Count)
            _snapshot.CurrentPlayerIndex = 0;
    }
    public void EndTurn()
    {
        _actionsThisRound++;

        if (IsBettingRoundComplete())
        {
            EventManager.Instance.TriggerEvent(GameEvents.BETTING_ROUND_COMPLETE, null);
            return;
        }

        MoveNext();
        StartTurn();
    }
    private bool IsBettingRoundComplete()
    {
        return _actionsThisRound >= _snapshot.Players.Count;
    }
    public void ResetRoundCounter()
    {
        _actionsThisRound = 0;
    }
}