﻿using Data.Serialization.SerializableSpaces;
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

            GUILayout.BeginVertical();
            {
                GUILayout.Space(20);
                GUILayout.Label("Add Crafters");

                if (GUILayout.Button("Add Enemy Spawn Crafter"))
                {
                    _target.AddEnemySpawnCrafter();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            {
                GUILayout.Space(20);
                if (GUILayout.Button("Serialize"))
                {
                    _json = JsonConvert.SerializeObject(SerializableSpaceHelper.ToSerializableSpace(_target.Build()), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                }

                _json = GUILayout.TextArea(_json, GUILayout.Height(100));

                if (GUILayout.Button("Deserialize"))
                {
                    _target.InitializeFromJSON(_json);
                }
            }
            GUILayout.EndVertical();
        }
    }
}