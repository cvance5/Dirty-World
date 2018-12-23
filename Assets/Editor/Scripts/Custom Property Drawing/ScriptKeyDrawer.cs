using Narrative;
using UnityEditor;
using UnityEngine;

namespace CustomPropertyDrawing
{
    [CustomPropertyDrawer(typeof(ScriptKey))]
    public class ScriptKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stringProperty = property.FindPropertyRelative("_key");
            EditorGUI.PropertyField(position, stringProperty, new GUIContent(property.displayName));
        }
    }
}