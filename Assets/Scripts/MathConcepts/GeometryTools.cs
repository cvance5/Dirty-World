using UnityEngine;

namespace MathConcepts
{
    public static class GeometryTools
    {
        public static Vector2 CenterOfRectangle(Vector2 topRight, Vector2 bottomLeft) => Vector2.Lerp(topRight, bottomLeft, .5f);
    }
}