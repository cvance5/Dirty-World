#if UNITY_EDITOR

using Tools.SpaceCrafting;
using UnityEditor;
using UnityEngine;
using WorldObjects.Spaces;

namespace GizmoDrawers
{
    public class SpaceCraftingGizmoDrawer : Singleton<SpaceCraftingGizmoDrawer>
    {
        private void Awake() => DontDestroyOnLoad(gameObject);

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                if (SpaceCraftingManager.Instance != null)
                {
                    foreach (var crafter in SpaceCraftingManager.Instance.GetComponentsInChildren<SpaceCrafter>())
                    {
                        DrawCrafter(crafter);
                    }
                }
            }
        }

        private void DrawCrafter(SpaceCrafter crafter)
        {
            DrawSpace(crafter.Build());

            foreach (var childCrafter in crafter.GetComponentsInChildren<SpaceCrafter>())
            {
                if (childCrafter == crafter) continue;

                DrawCrafter(childCrafter);
            }
        }

        private void DrawSpace(WorldObjects.Spaces.Space space, string prefixName = "")
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

            GizmoShapeDrawer.DrawByExtents(space.Extents);
            Handles.Label(space.Extents.Shapes[0].Vertices[0], $"{prefixName} {space.Name}");

            if (space is ComplexSpace)
            {
                var complexSpace = space as ComplexSpace;
                for (var regionNumber = 0; regionNumber < complexSpace.Regions.Count; regionNumber++)
                {
                    var region = complexSpace.Regions[regionNumber];
                    for (var spaceNumber = 0; spaceNumber < region.Spaces.Count; spaceNumber++)
                    {
                        DrawSpace(region.Spaces[spaceNumber]);
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

        private void DrawCircle(Vector2 center, float radius) => Gizmos.DrawSphere(center, radius);
    }
}
#endif