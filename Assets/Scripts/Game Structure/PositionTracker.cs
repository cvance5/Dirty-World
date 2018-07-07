using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldObjects;

public class PositionTracker : Singleton<PositionTracker>
{
    private static Dictionary<ITrackable, PositionData> _trackedTargets
        = new Dictionary<ITrackable, PositionData>();
    private static Dictionary<ITrackable, List<Action<PositionData, PositionData>>> _onPositionChangedCallbacks
        = new Dictionary<ITrackable, List<Action<PositionData, PositionData>>>();

    private static Coroutine _updateMethod;
    private static WaitForSeconds _positionUpdateTick = new WaitForSeconds(.25f);

    public static void BeginTracking(ITrackable target)
    {
        if (!_trackedTargets.ContainsKey(target))
        {
            var position = target.GetPosition();

            var chunk = World.GetContainingChunk(position);
            var space = chunk.GetSpaceForPosition(position);

            var initialPositionData = new PositionData(chunk, space);

            _trackedTargets.Add(target, initialPositionData);

            _log.Info($"Now tracking {target} at initial position {initialPositionData}.");

            if (_trackedTargets.Count == 1)
            {
                _log.Info("Starting position update coroutine.");
                _updateMethod = Instance.StartCoroutine(PositionUpdate());
            }
        }
    }

    public static void StopTracking(ITrackable target)
    {
        if (_trackedTargets.ContainsKey(target))
        {
            _trackedTargets.Remove(target);

            _log.Info($"No longer tracking {target}.");

            if (_trackedTargets.Count == 0)
            {
                _log.Info("Stopping position update coroutine.");
                Instance.StopCoroutine(_updateMethod);
            }

            _onPositionChangedCallbacks.Remove(target);
        }
    }

    public static void Subscribe(ITrackable target, Action<PositionData, PositionData> callback)
    {
        List<Action<PositionData, PositionData>> callbacks;

        if (_onPositionChangedCallbacks.TryGetValue(target, out callbacks))
        {
            if (callbacks.Contains(callback)) throw new ArgumentException("Duplicate callbacks have been subscribed!");
            else callbacks.Add(callback);
        }
        else
        {
            _onPositionChangedCallbacks.Add(target, new List<Action<PositionData, PositionData>>() { callback });
        }
    }

    public static void Unsubscribe(ITrackable target, Action<PositionData, PositionData> callback)
    {
        var callbacks = new List<Action<PositionData, PositionData>>();

        if (_onPositionChangedCallbacks.TryGetValue(target, out callbacks))
        {
            if (!callbacks.Contains(callback)) throw new ArgumentException("Callback not found to unsubscribe!");
            else callbacks.Remove(callback);
        }
        else throw new ArgumentException("Target does not have any callbacks!");
    }

    private static IEnumerator PositionUpdate()
    {
        while (true)
        {
            foreach (var trackedTarget in _trackedTargets.ToList())
            {
                var target = trackedTarget.Key;
                var position = target.GetPosition();
                var newPositionData = new PositionData();
                var oldPositionData = trackedTarget.Value;

                if (!oldPositionData.Chunk.Contains(position))
                {
                    newPositionData.Chunk = World.GetContainingChunk(position);
                }
                else newPositionData.Chunk = oldPositionData.Chunk;

                if (oldPositionData.Space == null ||
                    !oldPositionData.Space.Contains(position))
                {
                    newPositionData.Space = newPositionData.Chunk.GetSpaceForPosition(position);
                }
                else newPositionData.Space = oldPositionData.Space;

                if (oldPositionData != newPositionData)
                {
                    _log.Info($"{target} has changed from {oldPositionData} to {newPositionData}.");

                    List<Action<PositionData, PositionData>> callbacksToCall;
                    if (_onPositionChangedCallbacks.TryGetValue(target, out callbacksToCall))
                    {
                        foreach (var callback in callbacksToCall)
                        {
                            callback?.Invoke(oldPositionData, newPositionData);
                        }
                    }
                    _trackedTargets[target] = newPositionData;
                }
            }

            yield return _positionUpdateTick;
        }
    }

    private static readonly Log _log = new Log("PositionTracker");
}
