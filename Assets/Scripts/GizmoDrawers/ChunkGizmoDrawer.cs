#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;
using WorldObjects;
using WorldObjects.WorldGeneration;

namespace GizmoDrawers
{
    public class ChunkGizmoDrawer : Singleton<ChunkGizmoDrawer>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [Header("Chunks")]
        [SerializeField]
        private bool _showChunkLoadOrder = true;

        [SerializeField]
        private bool _showChunkBounderies = true;

        [SerializeField]
        private List<bool> _showChunk = new List<bool>();

        [Header("Builders")]
        [SerializeField]
        private bool _showBuilderCreationOrder = true;

        [SerializeField]
        private bool _showBuilderBoundaries = true;

        [SerializeField]
        private List<bool> _showBuilder = new List<bool>();
#pragma warning restore IDE0044 // Add readonly modifier

        private World _worldToDraw;

        public static void SetWorldToDraw(World world) => Instance._worldToDraw = world;

        private void OnDrawGizmosSelected()
        {
            if (_worldToDraw == null) return;

            var chunksInLoadOrder = _worldToDraw.ChunkArchitect.ChunkCache;

            UpdateShowChunkList(chunksInLoadOrder.Count);

            for (var loadOrder = 0; loadOrder < chunksInLoadOrder.Count; loadOrder++)
            {
                if (!_showChunk[loadOrder]) continue;

                var chunk = chunksInLoadOrder[loadOrder];

                if (_showChunkLoadOrder)
                {
                    LabelChunkLoadOrder(chunk, loadOrder);
                }

                if (_showChunkBounderies)
                {
                    DrawChunkBoundaries(chunk);
                }
            }

            var buildersInCreationOrder = _worldToDraw.ChunkArchitect.BuilderCache;

            UpdateShowBuilderList(buildersInCreationOrder.Count);

            for (var creationOrder = 0; creationOrder < buildersInCreationOrder.Count; creationOrder++)
            {
                if (!_showBuilder[creationOrder]) continue;

                var builder = buildersInCreationOrder[creationOrder];

                if (_showBuilderCreationOrder)
                {
                    LabelBuilderCreationOrder(builder, creationOrder);
                }

                if (_showBuilderBoundaries)
                {
                    DrawBuilderBoundaries(builder);
                }
            }
        }

        private void UpdateShowBuilderList(int count)
        {
            while (_showBuilder.Count < count)
            {
                _showBuilder.Add(true);
            }

            while (_showBuilder.Count > count)
            {
                _showBuilder.RemoveAt(_showBuilder.Count - 1);
            }
        }

        private void UpdateShowChunkList(int count)
        {
            while (_showChunk.Count < count)
            {
                _showChunk.Add(true);
            }

            while (_showChunk.Count > count)
            {
                _showChunk.RemoveAt(_showChunk.Count - 1);
            }
        }

        private void LabelChunkLoadOrder(Chunk chunk, int loadOrder)
        {
            var _loadOrderStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 25
            };

            var centerpoint = GeometryTools.CenterOfRectangle(chunk.TopRightCorner, chunk.BottomLeftCorner);

            Handles.Label(centerpoint, loadOrder.ToString(), _loadOrderStyle);
        }

        private void LabelBuilderCreationOrder(ChunkBuilder builder, int creationOrder)
        {
            var _loadOrderStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 25
            };

            var centerpoint = GeometryTools.CenterOfRectangle(builder.TopRightCorner, builder.BottomLeftCorner);

            Handles.Label(centerpoint, creationOrder.ToString(), _loadOrderStyle);
        }

        private void DrawChunkBoundaries(Chunk chunk)
        {
            Gizmos.color = Color.white;
            GizmoShapeDrawer.DrawRectangle(chunk.TopRightCorner, chunk.BottomLeftCorner);
        }

        private void DrawBuilderBoundaries(ChunkBuilder builder)
        {
            Gizmos.color = Color.blue;
            GizmoShapeDrawer.DrawRectangle(builder.TopRightCorner, builder.BottomLeftCorner);
        }
    }
}
#endif