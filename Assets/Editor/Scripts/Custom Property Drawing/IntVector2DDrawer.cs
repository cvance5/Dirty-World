#if UNITY_EDITOR
using MathConcepts;
using UnityEditor;
using UnityEngine;

namespace CustomPropertyDrawing
{
    [CustomPropertyDrawer(typeof(IntVector2))]
    public class IntVector2DDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, property.displayName);
            EditorGUI.PropertyField(new Rect(position.position.x, position.position.y + (position.height / 2), position.width / 2, position.height / 2), property.FindPropertyRelative("X"), new GUIContent("X"));
            EditorGUI.PropertyField(new Rect(position.position.x + (position.width / 2), position.position.y + (position.height / 2), position.width / 2, position.height / 2), property.FindPropertyRelative("Y"), new GUIContent("Y"));
        }
    }
}
#endif