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

            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Rebuild World"))
                {
                    _target.RebuildWorld();
                }

                GUILayout.Space(20);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            {
                GUILayout.Label("Build Spaces");
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
            GUILayout.EndVertical();
        }
    }
}