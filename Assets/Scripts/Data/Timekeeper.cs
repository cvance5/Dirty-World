using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class Timekeeper : Singleton<Timekeeper>
    {
        public static SmartEvent OnPause = new SmartEvent();
        public static SmartEvent OnUnpause = new SmartEvent();

        public static bool IsPaused = false;

        private static Dictionary<string, UnityStopwatch> _stopwatches = new Dictionary<string, UnityStopwatch>();

        public static void TogglePause(bool isPaused)
        {
            if (isPaused)
            {
                Time.timeScale = 0;
                _log.Info($"Pausing game...");
            }
            else if(IsPaused)
            {
                Time.timeScale = 1;
                _log.Info($"Unpausing game.");
            }

            IsPaused = isPaused;
            Instance.OnApplicationPause(isPaused);
        }

        public static UnityStopwatch StartStopwatch(string stopwatchName)
        {
            if (_stopwatches.ContainsKey(stopwatchName))
            {
                throw new ArgumentException($"Stopwatch by the name of `{stopwatchName}` already exists.");
            }

            var stopwatch = new UnityStopwatch();
            _stopwatches.Add(stopwatchName, stopwatch);

            return stopwatch;
        }

        public static UnityStopwatch GetStopwatch(string stopwatchName)
        {
            _stopwatches.TryGetValue(stopwatchName, out var stopwatch);
            return stopwatch;
        }

        public static double EndStopwatch(string stopwatchName)
        {
            if (_stopwatches.TryGetValue(stopwatchName, out var stopwatch))
            {
                _stopwatches.Remove(stopwatchName);
            }
            else _log.Warning($"No stopwatch `{stopwatchName}` found to end.");

            return stopwatch?.Delta ?? 0;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) PauseAllStopwatches();
            else UnpauseAllStopwatches();
        }

        public static void PauseAllStopwatches()
        {
            foreach (var stopwatch in _stopwatches.Values)
            {
                if (!stopwatch.IsPaused) stopwatch.Pause();
            }
        }

        public static void UnpauseAllStopwatches()
        {
            foreach (var stopwatch in _stopwatches.Values)
            {
                if (stopwatch.IsPaused) stopwatch.Unpause();
            }
        }

        public static Coroutine SetTimer(float delay, Action callback) => Instance.StartCoroutine(WaitForTimer(delay, callback));

        public static Coroutine SetTimer(int frames, Action callback) => Instance.StartCoroutine(WaitForFrames(frames, callback));

        private static IEnumerator WaitForTimer(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private static IEnumerator WaitForFrames(int frames, Action callback)
        {
            while (frames > 0)
            {
                yield return new WaitForEndOfFrame();
                frames--;
            }

            callback?.Invoke();
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("Timekeeper");
    }
}