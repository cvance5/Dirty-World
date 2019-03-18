#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GizmoDrawers
{
    [CustomEditor(typeof(SpaceHistoryGizmoDrawer))]
    [CanEditMultipleObjects]
    public class SpaceHistoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var historyObject = (SpaceHistoryGizmoDrawer)target;

            if (historyObject.CanTrackForwards)
            {
                if (GUILayout.Button("Step Forward"))
                {
                    historyObject.TrackForwards();
                }
            }

            if (historyObject.CanTrackBackwards)
            {
                if (GUILayout.Button("Step Backwards"))
                {
                    historyObject.TrackBackwards();
                }
            }
        }
    }
}
#endif