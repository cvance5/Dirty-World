using UnityEngine;

public class UnityStopwatch
{
    private readonly double _startTime;

    private double? _pauseTime;
    public bool IsPaused => _pauseTime.HasValue;

    private double _cumulativePauseOffset;

    public double Delta => (Time.time - _startTime) - _cumulativePauseOffset;

    public UnityStopwatch() => _startTime = Time.time;

    public void Pause()
    {
        if (_pauseTime.HasValue)
        {
            _log.Warning($"Stopwatch is already paused, cannot be paused again.");
        }
        else _pauseTime = Time.time;
    }

    public void Unpause()
    {
        if (!_pauseTime.HasValue)
        {
            _log.Warning($"Stopwatch is not paused, cannot be unpaused.");
        }
        else
        {
            var pauseDelta = Time.time - _pauseTime;
            _cumulativePauseOffset += pauseDelta.Value;
            _pauseTime = null;
        }
    }

    private static readonly Log _log = new Log("Unity Stopwatch");
}
