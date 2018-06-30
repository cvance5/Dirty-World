using System;
using System.Collections.Generic;

/// <summary>
/// Collect fires of given events, and fire a callback when the threshold is met.
/// </summary>
public class EventCollector
{
    private List<SmartEvent> _events = new List<SmartEvent>();
    private Action _onComplete;

    private int _threshold;

    public EventCollector(int threshold, Action onComplete)
    {
        _threshold = threshold;
        _onComplete = onComplete;
    }

    public void AddEvent(SmartEvent smartEvent)
    {
        smartEvent += OnEventFire;
    }

    private void OnEventFire()
    {
        _threshold--;
        CheckComplete();
    }

    private void CheckComplete()
    {
        if(_threshold == 0)
        {
            _onComplete.Invoke();
            for (int eventNumber = 0; eventNumber < _events.Count; eventNumber++)
            {
                _events[eventNumber] -= OnEventFire;
            }
        }
    }
}

/// <summary>
/// Collect fires of given events, and fire a callback when enough unique instances have been raised.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventCollector<T>
{
    private List<SmartEvent<T>> _events = new List<SmartEvent<T>>();
    private List<T> _dataCollected = new List<T>();
    private Action _onComplete;

    private int _numberToCollect;

    public EventCollector(int numberToCollect, Action onComplete)
    {
        _numberToCollect = numberToCollect;
        _onComplete = onComplete;
    }

    public void AddEvent(SmartEvent<T> smartEvent)
    {
        smartEvent += OnEventFire;
    }

    private void OnEventFire(T data)
    {
        if(!_dataCollected.Contains(data))
        {
            _numberToCollect--;
            CheckComplete();
        }
    }

    private void CheckComplete()
    {
        if (_numberToCollect == 0)
        {
            _onComplete.Invoke();
            for (int eventNumber = 0; eventNumber < _events.Count; eventNumber++)
            {
                _events[eventNumber] -= OnEventFire;
            }
        }
    }
}
