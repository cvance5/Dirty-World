#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace GizmoDrawers
{
    public class SpaceTimelineGizmoDrawer : Singleton<SpaceTimelineGizmoDrawer>
    {
        private static Dictionary<SpaceBuilder, SpaceHistoryGizmoDrawer> _historyPerBuilder
                 = new Dictionary<SpaceBuilder, SpaceHistoryGizmoDrawer>();

        private static List<SpaceHistoryGizmoDrawer> _spaceHistories = new List<SpaceHistoryGizmoDrawer>();

        private static List<SpaceHistoryGizmoDrawer> _timeline = new List<SpaceHistoryGizmoDrawer>();

        private static int _timelineIndex = 0;

        private void OnDrawGizmosSelected()
        {
            foreach (var history in _spaceHistories)
            {
                history.ResetHistory();
            }

            if (_timeline.Count > 0)
            {
                for (var nextStep = 0; nextStep <= _timelineIndex; nextStep++)
                {
                    _timeline[nextStep].TrackForwards();
                }
            }

            foreach (var history in _spaceHistories)
            {
                history.OnDrawGizmosSelected();
            }
        }

        public static void BeginTracking()
        {
            SpaceArchitect.OnNewSpaceBuilderDeclared += BeginLoggingSpace;
            CustomSpace.OnCustomSpaceBuilt += LogCustomSpace;
        }

        private static void BeginLoggingSpace(SpaceBuilder newBuilder)
        {
            if (_historyPerBuilder.ContainsKey(newBuilder))
            {
                throw new System.ArgumentException($"Already tracking spaceBuilder {newBuilder.Name}.");
            }
            else
            {
                var history = CreateHistory();
                _historyPerBuilder[newBuilder] = history;
            }

            newBuilder.OnSpaceBuilderChanged += LogSpaceHistory;
        }

        private static void LogSpaceHistory(SpaceBuilder changedBuilder)
        {
            if (changedBuilder.IsValid)
            {
                var space = changedBuilder.Build();
                
                if(!_historyPerBuilder.TryGetValue(changedBuilder, out var history))
                {
                    throw new System.InvalidOperationException($"No history has been established for {changedBuilder.Name}.");
                }

                history.LogChange(space);
                _timeline.Add(history);
            }
        }

        private static void LogCustomSpace(WorldObjects.Spaces.Space customSpace)
        {
            var history = CreateHistory();

            history.LogChange(customSpace);
            _timeline.Add(history);
        }

        private static SpaceHistoryGizmoDrawer CreateHistory()
        {
            var gameobject = new GameObject();
            var history = gameobject.AddComponent<SpaceHistoryGizmoDrawer>();
            history.transform.SetParent(Instance.transform);
            _spaceHistories.Add(history);

            return history;
        }

        public bool CanTrackBackwards => _timelineIndex > 0;
        public void TrackBackwards()
        {
            if (CanTrackBackwards)
            {
                _timelineIndex--;
            }
        }

        public bool CanTrackForwards => _timelineIndex < _timeline.Count - 1;
        public void TrackForwards()
        {
            if (CanTrackForwards)
            {
                _timelineIndex++;
            }
        }

        private void OnDestroy()
        {
            SpaceArchitect.OnNewSpaceBuilderDeclared -= BeginLoggingSpace;
            CustomSpace.OnCustomSpaceBuilt -= LogCustomSpace;

            foreach (var kvp in _historyPerBuilder)
            {
                kvp.Key.OnSpaceBuilderChanged -= LogSpaceHistory;
            }
        }
    }
}
#endif