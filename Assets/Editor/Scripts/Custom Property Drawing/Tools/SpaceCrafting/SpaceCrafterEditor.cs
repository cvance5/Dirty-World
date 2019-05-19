using Data.Serialization;
using Tools.SpaceCrafting;
using UnityEditor;
using UnityEngine;
using WorldObjects.Spaces;

namespace CustomPropertyDrawing.Tools.SpaceCrafting
{
    [CustomEditor(typeof(SpaceCrafter), true)]
    public class SpaceCraftingEditor : Editor
    {
        private CustomSpace _asset;

        private SpaceCrafter _target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _asset = EditorGUILayout.ObjectField("Asset", _asset, typeof(CustomSpace), false) as CustomSpace;

            _target = target as SpaceCrafter;

            GUILayout.BeginVertical();
            {
                GUILayout.Space(20);
                GUILayout.Label("Add Crafters");

                if (GUILayout.Button("Add Enemy Spawn Crafter"))
                {
                    _target.AddEnemySpawnCrafter();
                }

                if(GUILayout.Button("Add Block Override"))
                {
                    _target.AddBlockOverride();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            {
                GUILayout.Space(20);
                if (GUILayout.Button("Save"))
                {
                    if (_asset == null)
                    {
                        _asset = CreateInstance<CustomSpace>();
                        AssetDatabase.CreateAsset(_asset, ASSET_PATH + System.IO.Path.GetRandomFileName() + ".asset");
                    }

                    Undo.RecordObject(_asset, "Set SerializableSpace JSON");
                    _asset.Set(new SerializableSpace(_target.Build()));
                    EditorUtility.SetDirty(_asset);

                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Load"))
                {
                    if (_asset != null)
                    {
                        _target.InitializeFromSpace(_asset.Build());
                    }
                    else
                    {
                        _log.Warning($"No asset loaded!  Drag a custom space into the asset field to load.");
                    }
                }
            }
            GUILayout.EndVertical();
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("SpaceCrafterEditor");

        private const string ASSET_PATH = "Assets/Data/CustomSpaces/";
    }
}