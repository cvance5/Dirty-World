#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace GizmoDrawers
{
    public class GizmoShapeDrawer : MonoBehaviour
    {
        public static void DrawRectangle(Vector2 topRight, Vector2 bottomLeft)
        {
            var topLeft = new Vector2(bottomLeft.x, topRight.y);
            var bottomRight = new Vector2(topRight.x, bottomLeft.y);

            DrawRectangle(topLeft, topRight, bottomRight, bottomLeft);
        }

        public static void DrawRectangle(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }

        public static void DrawByExtents(List<IntVector2> extents)
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