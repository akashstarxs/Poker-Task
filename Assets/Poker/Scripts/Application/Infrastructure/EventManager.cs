using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : GenericSingleton<EventManager>
{
     // Dictionary to hold events and their corresponding actions
    private Dictionary<string, Action<object>> _eventDictionary = new Dictionary<string, Action<object>>();

    /// <summary>
    /// Subscribe a callback to an event.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="listener">Callback to be invoked when the event is triggered.</param>
    public void Subscribe(string eventName, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            _eventDictionary[eventName] = thisEvent + listener;
        }
        else
        {
            _eventDictionary[eventName] = listener;
        }
    }

    /// <summary>
    /// Unsubscribe a callback from an event.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="listener">Callback to be removed.</param>
    public void Unsubscribe(string eventName, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent -= listener;
            if (thisEvent == null)
            {
                _eventDictionary.Remove(eventName);
            }
            else
            {
                _eventDictionary[eventName] = thisEvent;
            }
        }
    }

    /// <summary>
    /// Trigger an event, invoking all subscribed listeners.
    /// </summary>
    /// <param name="eventName">Name of the event to trigger.</param>
    /// <param name="eventData">Data to pass to listeners (optional).</param>
    public void TriggerEvent(string eventName, object eventData = null)
    {
        if (_eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.Invoke(eventData);
        }
        else
        {
            Debug.LogWarning($"Event '{eventName}' does not exist.");
        }
    }
}
