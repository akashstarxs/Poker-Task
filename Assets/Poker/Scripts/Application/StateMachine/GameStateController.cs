using Poker.Core.Enums;
using Poker.Core.Models;

public class GameStateController
{
    private IGameState _currentState;
    private readonly GameSnapshot _snapshot;

    public GameStateController(GameSnapshot snapshot)
    {
        _snapshot = snapshot;
    }

    public void ChangeState(IGameState newState)
    {
        _currentState?.Exit(_snapshot);

        _currentState = newState;

        _currentState.Enter(_snapshot);

        EventManager.Instance.TriggerEvent(GameEvents.STATE_CHANGED, _snapshot);
    }
}