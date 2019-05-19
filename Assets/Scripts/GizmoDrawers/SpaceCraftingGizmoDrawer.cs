#if UNITY_EDITOR

using Tools.SpaceCrafting;
using UnityEditor;
using UnityEngine;

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
            Gizmos.color = SpaceColorUtility.GetOutlineColor(space);
            GizmoShapeDrawer.DrawByExtents(space.Extents);
            Handles.Label(space.Extents.Shapes[0].Vertices[0], $"{prefixName} {space.Name}");
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