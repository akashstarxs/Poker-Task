using System.Collections;
using UnityEngine;
using Poker.Core.Models;

public class TurnTimerService : MonoBehaviour
{
    [SerializeField] private float turnDuration = 15f;

    private Coroutine _timerRoutine;
    private Player _currentPlayer;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvents.TURN_STARTED, OnTurnStarted);
        EventManager.Instance.Subscribe(GameEvents.PLAYER_ACTION, OnPlayerAction);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvents.TURN_STARTED, OnTurnStarted);
        EventManager.Instance.Unsubscribe(GameEvents.PLAYER_ACTION, OnPlayerAction);
    }

    private void OnTurnStarted(object data)
    {
        _currentPlayer = (Player)data;

        if (_timerRoutine != null)
            StopCoroutine(_timerRoutine);

        _timerRoutine = StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        float time = turnDuration;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        OnTimeout();
    }

    private void OnPlayerAction(object _)
    {
        if (_timerRoutine != null)
            StopCoroutine(_timerRoutine);
    }

    private void OnTimeout()
    {
        Debug.Log($"Turn timeout: {_currentPlayer.Name}");

        EventManager.Instance.TriggerEvent(
            GameEvents.PLAYER_ACTION,
            new TimeoutAction(_currentPlayer)
        );
    }
}