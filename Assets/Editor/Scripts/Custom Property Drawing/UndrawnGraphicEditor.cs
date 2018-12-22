using UI.Components.UnityModifiers;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace CustomPropertyDrawing
{
    [CanEditMultipleObjects, CustomEditor(typeof(UndrawnGraphic), false)]
    public class UndrawnGrapicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Script, new GUILayoutOption[0]);
            // skipping AppearanceControlsGUI
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}