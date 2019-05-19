#if UNITY_EDITOR

using UnityEngine;
using WorldObjects.Spaces;

namespace GizmoDrawers
{
    public static class SpaceColorUtility
    {
        public static Color GetOutlineColor(WorldObjects.Spaces.Space space)
        {
            if (space.Name.Contains("Tunnel"))
            {
                return Color.red;
            }
            else if (space.Name.Contains("Monster Den"))
            {
                return Color.white;
            }
            else if (space.Name.Contains("Treasure Room"))
            {
                return Color.yellow;
            }
            else if (space.Name.Contains("Room"))
            {
                return Color.black;
            }
            else throw new System.ArgumentOutOfRangeException($"No color assigned for {space.GetType()}.");
        }
    }
}
#endif