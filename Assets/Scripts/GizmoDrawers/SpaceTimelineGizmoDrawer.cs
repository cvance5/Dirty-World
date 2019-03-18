#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
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
            SpaceBuilder.OnSpaceBuilderChanged += LogSpaceHistory;
            CustomSpace.OnCustomSpaceBuilt += LogCustomSpace;
        }

        private static void LogSpaceHistory(SpaceBuilder changedBuilder)
        {
            var space = changedBuilder.Build();

            if (space is ComplexSpace)
            {
                var complexSpace = space as ComplexSpace;
                foreach (var region in complexSpace.Regions)
                {
                    foreach (var regionSpace in region.Spaces)
                    {
                        if (!_historyPerBuilder.TryGetValue(changedBuilder, out var history))
                        {
                            history = CreateHistory();

                            _historyPerBuilder[changedBuilder] = history;
                        }

                        history.LogChange(regionSpace);
                        _timeline.Add(history);
                    }
                }
            }
            else
            {
                if (!_historyPerBuilder.TryGetValue(changedBuilder, out var history))
                {
                    history = CreateHistory();

                    _historyPerBuilder[changedBuilder] = history;
                }

                history.LogChange(space);
                _timeline.Add(history);
            }
        }

        private static void LogCustomSpace(WorldObjects.Spaces.Space customSpace)
        {
            if (customSpace is ComplexSpace)
            {
                var complexSpace = customSpace as ComplexSpace;
                foreach (var region in complexSpace.Regions)
                {
                    foreach (var regionSpace in region.Spaces)
                    {
                        var history = CreateHistory();

                        history.LogChange(regionSpace);
                        _timeline.Add(history);
                    }
                }
            }
            else
            {
                var history = CreateHistory();

                history.LogChange(customSpace);
                _timeline.Add(history);
            }
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
            SpaceBuilder.OnSpaceBuilderChanged -= LogSpaceHistory;
            CustomSpace.OnCustomSpaceBuilt -= LogCustomSpace;
        }
    }
}
#endif