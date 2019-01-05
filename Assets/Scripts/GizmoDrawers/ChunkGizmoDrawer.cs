#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;
using WorldObjects;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;
using Space = WorldObjects.Spaces.Space;

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
        private bool _showChunkSpaces = true;

        [SerializeField]
        private List<bool> _showChunk = new List<bool>();

        [Header("Builders")]
        [SerializeField]
        private bool _showBuilderCreationOrder = true;

        [SerializeField]
        private bool _showBuilderBoundaries = true;

        [SerializeField]
        private bool _showBuilderSpaces = true;

        [SerializeField]
        private List<bool> _showBuilder = new List<bool>();

        [Header("Spaces")]
        [SerializeField]
        private bool _showSpaceNames = true;
#pragma warning restore IDE0044 // Add readonly modifier

        private World _worldToDraw;
        private WorldBuilder _builderToDraw;

        private void Awake() => DontDestroyOnLoad(gameObject);

        public static void SetWorldToDraw(World world, WorldBuilder builder)
        {
            Instance._worldToDraw = world;
            Instance._builderToDraw = builder;
        }

        private void OnDrawGizmosSelected()
        {
            if (_worldToDraw == null) return;

            var chunksInLoadOrder = _worldToDraw.LoadedChunks;

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

                if (_showChunkSpaces)
                {
                    foreach (var space in _worldToDraw.Spaces)
                    {
                        DrawSpace(space);
                    }
                }
            }

            var buildersInCreationOrder = _builderToDraw.ChunkBuilders;

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

                if (_showBuilderSpaces)
                {
                    foreach (var space in builder.Spaces)
                    {
                        DrawSpace(space);
                    }
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
            DrawRectangle(chunk.TopRightCorner, chunk.BottomLeftCorner);
        }

        private void DrawBuilderBoundaries(ChunkBuilder builder)
        {
            Gizmos.color = Color.blue;
            DrawRectangle(builder.TopRightCorner, builder.BottomLeftCorner);
        }

        private void DrawSpace(Space space, string prefixName = "")
        {
            if (space is Corridor)
            {
                Gizmos.color = Color.red;
            }
            else if (space is Shaft)
            {
                Gizmos.color = Color.green;
            }
            else if (space is MonsterDen)
            {
                Gizmos.color = Color.white;
            }
            else if (space is TreasureRoom)
            {
                Gizmos.color = Color.yellow;
            }
            else if (space is Room)
            {
                Gizmos.color = Color.black;
            }
            else if (space is Laboratory)
            {
                Gizmos.color = Color.gray;
            }

            DrawByExtents(space.Extents);
            if (_showSpaceNames)
            {
                Handles.Label(space.Extents[0], $"{prefixName}{space.Name}");
            }

            if (space is ComplexSpace)
            {
                var complexSpace = space as ComplexSpace;
                for(int regionNumber = 0; regionNumber < complexSpace.Regions.Count; regionNumber++)
                {
                    var region = complexSpace.Regions[regionNumber];
                    for (var spaceNumber = 0; spaceNumber < region.Spaces.Count; spaceNumber++)
                    {
                        DrawSpace(region.Spaces[spaceNumber], $"{complexSpace.Name}'s #{spaceNumber + regionNumber}: ");
                    }
                }
            }
        }

        private void DrawRectangle(Vector2 topRight, Vector2 bottomLeft)
        {
            var topLeft = new Vector2(bottomLeft.x, topRight.y);
            var bottomRight = new Vector2(topRight.x, bottomLeft.y);

            DrawRectangle(topLeft, topRight, bottomRight, bottomLeft);
        }

        private void DrawRectangle(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }

        private void DrawByExtents(List<IntVector2> extents)
        {
            for (var i = 0; i < extents.Count - 1; i++)
            {
                Gizmos.DrawLine(extents[i], extents[i + 1]);
            }

            Gizmos.DrawLine(extents[extents.Count - 1], extents[0]);
        }
    }
}
#endif