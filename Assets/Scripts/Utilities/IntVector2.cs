using UnityEngine;

public struct IntVector2
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public IntVector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public IntVector2(Vector2 vec)
    {
        X = Mathf.RoundToInt(vec.x);
        Y = Mathf.RoundToInt(vec.y);
    }

    public static IntVector2 operator +(IntVector2 lhs, IntVector2 rhs) => new IntVector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
    public static IntVector2 operator +(IntVector2 lhs, Vector2 rhs) => lhs + new IntVector2(rhs);

    public override string ToString() => $"[{X} , {Y}]";

    public override bool Equals(object obj)
    {
        if (!(obj is IntVector2))
        {
            return false;
        }

        var vector = (IntVector2)obj;
        return X == vector.X &&
               Y == vector.Y;
    }
}
