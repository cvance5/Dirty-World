#if UNITY_EDITOR
using UnityEngine;
using WorldObjects.Spaces.Geometry;

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

        public static void DrawByExtents(Extents extents)
        {
            foreach (var shape in extents.Shapes)
            {
                DrawByShape(shape);
            }
        }

        public static void DrawByShape(Shape shape)
        {
            for (var i = 0; i < shape.Vertices.Count - 1; i++)
            {
                Gizmos.DrawLine(shape.Vertices[i], shape.Vertices[i + 1]);
            }
            Gizmos.DrawLine(shape.Vertices[shape.Vertices.Count - 1], shape.Vertices[0]);
        }
    }
}
#endif