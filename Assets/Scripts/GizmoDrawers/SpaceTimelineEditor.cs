#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GizmoDrawers
{
    [CustomEditor(typeof(SpaceTimelineGizmoDrawer))]
    [CanEditMultipleObjects]
    public class SpaceTimelineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var timelineObject = (SpaceTimelineGizmoDrawer)target;

            if (timelineObject.CanTrackForwards)
            {
                if (GUILayout.Button("Step Forward"))
                {
                    timelineObject.TrackForwards();
                }
            }

            if (timelineObject.CanTrackBackwards)
            {
                if (GUILayout.Button("Step Backwards"))
                {
                    timelineObject.TrackBackwards();
                }
            }
        }
    }
}
#endif