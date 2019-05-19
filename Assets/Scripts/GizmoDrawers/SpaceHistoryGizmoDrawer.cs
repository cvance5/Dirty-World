#if UNITY_EDITOR
using MathConcepts.Geometry;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.Debug;

namespace GizmoDrawers
{
    public class SpaceHistoryGizmoDrawer : MonoBehaviour
    {
        private int _historyIndex = 0;

        private Color _drawColor;
        private string _spaceName;
        private List<Extents> _history = new List<Extents>();

        private void Awake() =>
            // All spaces start out...not existing
            _history.Add(null);

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = _drawColor;

            var extents = _history[_historyIndex];

            if (extents != null)
            {
                GizmoShapeDrawer.DrawByExtents(extents);
                Handles.Label(extents.Max, _spaceName);
            }
        }

        public void LogChange(WorldObjects.Spaces.Space current)
        {
            _history.Add(new Extents(current.Extents.Shapes));
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

        private static readonly Log _log = new Log("SpaceHistoryGizmoDrawer");
    }
}
#endif