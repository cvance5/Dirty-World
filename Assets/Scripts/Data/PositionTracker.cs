﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldObjects;

namespace Data
{
    public class PositionTracker : Singleton<PositionTracker>
    {
        private static Dictionary<ITrackable, PositionData> _trackedTargets
            = new Dictionary<ITrackable, PositionData>();
        private static Dictionary<ITrackable, List<Action<ITrackable, PositionData, PositionData>>> _onPositionChangedCallbacks
            = new Dictionary<ITrackable, List<Action<ITrackable, PositionData, PositionData>>>();

        private static Coroutine _updateMethod;
        private static readonly WaitForSecondsRealtime _positionUpdateTick = new WaitForSecondsRealtime(.075f);

        private static World _worldToTrack;

        public static void SetWorldToTrack(World worldToTrack) => _worldToTrack = worldToTrack;

        public static void BeginTracking(ITrackable target)
        {
            if (!_trackedTargets.ContainsKey(target))
            {
                var position = target.Position;
                var space = _worldToTrack.SpaceArchitect.GetContainingSpace(position);

                var initialPositionData = new PositionData(space, position);

                var chunk = _worldToTrack.ChunkArchitect.GetContainingChunk(position);
                if (chunk != null)
                {
                    initialPositionData.Chunk = chunk;
                }
                else
                {
                    var builder = _worldToTrack.ChunkArchitect.GetContainingBuilder(position);
                    initialPositionData.Builder = builder;
                }

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

        public static void Subscribe(ITrackable target, Action<ITrackable, PositionData, PositionData> callback)
        {
            if (_onPositionChangedCallbacks.TryGetValue(target, out var callbacks))
            {
                if (callbacks.Contains(callback)) throw new ArgumentException("Duplicate callbacks have been subscribed!");
                else callbacks.Add(callback);
            }
            else
            {
                _onPositionChangedCallbacks.Add(target, new List<Action<ITrackable, PositionData, PositionData>>() { callback });
            }
        }

        public static void Unsubscribe(ITrackable target, Action<ITrackable, PositionData, PositionData> callback)
        {
            if (_onPositionChangedCallbacks.TryGetValue(target, out var callbacks))
            {
                if (!callbacks.Contains(callback)) throw new ArgumentException("Callback not found to unsubscribe!");
                else callbacks.Remove(callback);
            }
            else throw new ArgumentException("Target does not have any callbacks!");
        }

        public static PositionData GetCurrentPosition(ITrackable target)
        {
            if (!_trackedTargets.TryGetValue(target, out var data))
            {
                throw new InvalidOperationException($"Cannot get data of untracked target.");
            }

            return data;
        }

        private static IEnumerator PositionUpdate()
        {
            while (true)
            {
                foreach (var trackedTarget in _trackedTargets.ToList())
                {
                    var target = trackedTarget.Key;
                    var oldPositionData = trackedTarget.Value;

                    var position = target.Position;
                    var space = oldPositionData.Space;

                    // See if any spaces contain this position
                    if (space == null || !space.Contains(position))
                    {
                        space = _worldToTrack.SpaceArchitect.GetContainingSpace(position) ?? null;
                    }

                    var newPositionData = new PositionData(space, position);

                    // See if we are still in the same chunk (if any)
                    if (oldPositionData.Chunk == null ||
                        !oldPositionData.Chunk.Contains(position))
                    {
                        // Else see what chunk we are in
                        newPositionData.Chunk = _worldToTrack.ChunkArchitect.GetContainingChunk(position);
                    }
                    else newPositionData.Chunk = oldPositionData.Chunk;

                    // If we aren't in a chunk, we must be in a builder
                    if (newPositionData.Chunk == null)
                    {
                        newPositionData.Builder = _worldToTrack.ChunkArchitect.GetContainingBuilder(position);
                    }

                    // If the position has changed, it is an update, so call everyone
                    if (oldPositionData != newPositionData)
                    {
                        if (_onPositionChangedCallbacks.TryGetValue(target, out var callbacksToCall))
                        {
                            // Duplicate the list briefly, so that any callbacks that remove things from the list don't interrupt execution
                            foreach (var callback in callbacksToCall.ToList())
                            {
                                callback?.Invoke(target, oldPositionData, newPositionData);
                            }
                        }
                        _trackedTargets[target] = newPositionData;
                    }
                }

                yield return _positionUpdateTick;
            }
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("PositionTracker");
    }
}