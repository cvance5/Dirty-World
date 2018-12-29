using UnityEngine;

namespace WorldObjects
{
    public static class Directions
    {
        public static readonly IntVector2 Up = new IntVector2(Vector2.up);
        public static readonly IntVector2 Right = new IntVector2(Vector2.right);
        public static readonly IntVector2 Down = new IntVector2(Vector2.down);
        public static readonly IntVector2 Left = new IntVector2(Vector2.left);

        public static IntVector2 RandomLeftOrRight => Chance.CoinFlip ? Left : Right;
        public static IntVector2 RandomUpOrDown => Chance.CoinFlip ? Up : Down;

        public static readonly IntVector2[] Cardinals = new IntVector2[]
        {
            Up,
            Right,
            Down,
            Left
        };

        public static readonly IntVector2[] Ordinals = new IntVector2[]
        {
            Up + Left,
            Up + Right,
            Down + Right,
            Down + Left
        };

        public static readonly IntVector2[] Compass = new IntVector2[]
        {
            Up + Left,
            Up,
            Up + Right,
            Right,
            Down + Right,
            Down,
            Down + Left,
            Left
        };
    }
}