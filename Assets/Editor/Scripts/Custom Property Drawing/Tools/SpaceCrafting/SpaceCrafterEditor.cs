using Data.Serialization.SerializableSpaces;
using Newtonsoft.Json;
using Tools.SpaceCrafting;
using UnityEditor;
using UnityEngine;

namespace CustomPropertyDrawing.Tools.SpaceCrafting
{
    [CustomEditor(typeof(SpaceCrafter), true)]
    public class SpaceCraftingEditor : Editor
    {
        private SpaceCrafter _target;
        private string _json;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _target = target as SpaceCrafter;

            if (GUILayout.Button("Serialize"))
            {
                _json = JsonConvert.SerializeObject(SerializableSpaceHelper.ToSerializableSpace(_target.Build()));
            }

            GUILayout.TextArea(_json, GUILayout.Height(250));
        }
    }
}