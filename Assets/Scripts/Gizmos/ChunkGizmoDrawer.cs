#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WorldObjects;
using WorldObjects.Spaces;
using Space = WorldObjects.Spaces.Space;

namespace Utilities.Editor
{
    public class ChunkGizmoDrawer : MonoBehaviour
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

        [Header("Blueprints")]
        [SerializeField]
        private bool _showBlueprintCreationOrder = true;

        [SerializeField]
        private bool _showBlueprintBoundaries = true;

        [SerializeField]
        private bool _showBlueprintSpaces = true;

        [SerializeField]
        private List<bool> _showBlueprint = new List<bool>();

        [Header("Spaces")]
        [SerializeField]
        private bool _showSpaceNames = true;

#pragma warning restore IDE0044 // Add readonly modifier

        private void Awake() => DontDestroyOnLoad(gameObject);

        private void OnDrawGizmosSelected()
        {
            if (GameManager.World == null) return;

            var chunksInLoadOrder = GameManager.World.LoadedChunks;

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
                    foreach (var space in chunk.Spaces)
                    {
                        DrawSpace(space);
                    }
                }
            }

            var blueprintsInCreationOrder = GameManager.World.PendingBlueprints;

            UpdateShowBlueprintList(blueprintsInCreationOrder.Count);

            for (var creationOrder = 0; creationOrder < blueprintsInCreationOrder.Count; creationOrder++)
            {
                if (!_showBlueprint[creationOrder]) continue;

                var blueprint = blueprintsInCreationOrder[creationOrder];

                if (_showBlueprintCreationOrder)
                {
                    LabelBlueprintCreationOrder(blueprint, creationOrder);
                }

                if (_showBlueprintBoundaries)
                {
                    DrawBlueprintBoundaries(blueprint);
                }

                if (_showBlueprintSpaces)
                {
                    foreach (var space in blueprint.Spaces)
                    {
                        DrawSpace(space);
                    }
                }
            }
        }

        private void UpdateShowBlueprintList(int count)
        {
            while (_showBlueprint.Count < count)
            {
                _showBlueprint.Add(true);
            }

            while (_showBlueprint.Count > count)
            {
                _showBlueprint.RemoveAt(_showBlueprint.Count - 1);
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

        private void LabelBlueprintCreationOrder(ChunkBlueprint blueprint, int creationOrder)
        {
            var _loadOrderStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 25
            };

            var centerpoint = GeometryTools.CenterOfRectangle(blueprint.TopRightCorner, blueprint.BottomLeftCorner);

            Handles.Label(centerpoint, creationOrder.ToString(), _loadOrderStyle);
        }

        private void DrawChunkBoundaries(Chunk chunk)
        {
            Gizmos.color = Color.white;
            DrawRectangle(chunk.TopRightCorner, chunk.BottomLeftCorner);
        }

        private void DrawBlueprintBoundaries(ChunkBlueprint blueprint)
        {
            Gizmos.color = Color.blue;
            DrawRectangle(blueprint.TopRightCorner, blueprint.BottomLeftCorner);
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

        private void DrawCircle(Vector2 center, float radius) => Gizmos.DrawSphere(center, radius);
    }
}
#endif