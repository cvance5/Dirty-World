using Tools.SpaceCrafting;
using UnityEditor;
using UnityEngine;

namespace CustomPropertyDrawing.Tools.SpaceCrafting
{
    [CustomEditor(typeof(SpaceCraftingManager), false)]
    public class SpaceCraftingManagerEditor : UnityEditor.Editor
    {
        private SpaceCraftingManager _target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _target = target as SpaceCraftingManager;

            if (GUILayout.Button("Rebuild World"))
            {
                _target.RebuildWorld();
            }

            if (GUILayout.Button("New Shaft"))
            {
                _target.AddNewCrafter<ShaftCrafter>();
            }

            if (GUILayout.Button("New Corridor"))
            {
                _target.AddNewCrafter<CorridorCrafter>();
            }

            if (GUILayout.Button("New Room"))
            {
                _target.AddNewCrafter<RoomCrafter>();
            }

            if (GUILayout.Button("New Monster Den"))
            {
                _target.AddNewCrafter<MonsterDenCrafter>();
            }

            if (GUILayout.Button("New Laboratory"))
            {
                _target.AddNewCrafter<LaboratoryCrafter>();
            }
        }
    }
}