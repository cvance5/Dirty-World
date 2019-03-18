#if UNITY_EDITOR

using UnityEngine;
using WorldObjects.Spaces;

namespace GizmoDrawers
{
    public static class SpaceColorUtility
    {
        public static Color GetOutlineColor(WorldObjects.Spaces.Space space)
        {
            if (space is Corridor)
            {
                return Color.red;
            }
            else if (space is Shaft)
            {
                return Color.green;
            }
            else if (space is MonsterDen)
            {
                return Color.white;
            }
            else if (space is TreasureRoom)
            {
                return Color.yellow;
            }
            else if (space is Room)
            {
                return Color.black;
            }
            else if (space is Laboratory)
            {
                return Color.gray;
            }
            else throw new System.ArgumentOutOfRangeException($"No color assigned for {space.GetType()}.");
        }
    }
}
#endif