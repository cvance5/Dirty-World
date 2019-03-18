#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GizmoDrawers
{
    public class SpaceHistoryGizmoDrawer : MonoBehaviour
    {
        private int _historyIndex = 0;

        private Color _drawColor;
        private string _spaceName;
        private List<List<IntVector2>> _history = new List<List<IntVector2>>();

        private void Awake() =>
            // All spaces start out...not existing
            _history.Add(new List<IntVector2>());

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = _drawColor;

            var extents = _history[_historyIndex];

            if (extents.Count > 0)
            {
                GizmoShapeDrawer.DrawByExtents(extents);
                Handles.Label(extents[0], _spaceName);
            }
        }

        public void LogChange(WorldObjects.Spaces.Space current)
        {
            _history.Add(current.Extents);
            _historyIndex = _history.Count - 1;

            _spaceName = current.Name;
            _drawColor = SpaceColorUtility.GetOutlineColor(current);
            gameObject.name = $"History for {current.Name}";
        }

        public void ResetHistory() => _historyIndex = 0;

        public bool CanTrackBackwards => _historyIndex > 0;
        public void TrackBackwards()
        {
            if (CanTrackBackwards)
            {
                _historyIndex--;
            }
        }

        public bool CanTrackForwards => _historyIndex < _history.Count - 1;
        public void TrackForwards()
        {
            if (CanTrackForwards)
            {
                _historyIndex++;
            }
        }
    }
}
#endif