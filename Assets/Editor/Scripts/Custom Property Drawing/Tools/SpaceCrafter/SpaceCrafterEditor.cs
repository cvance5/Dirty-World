using UnityEditor;
using UnityEngine;

namespace Tools.SpaceCrafting.Editor
{
    [CustomEditor(typeof(SpaceCraftingManager), false)]
    public class SpaceCrafterEditor : UnityEditor.Editor
    {
        private SpaceCraftingManager _target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _target = target as SpaceCraftingManager;

            if(GUILayout.Button("Rebuild World"))
            {
                _target.RebuildWorld();
            }

            if(GUILayout.Button("New Shaft"))
            {
                _target.AddNewCrafter<ShaftCrafter>();
            }
        }
    }
}