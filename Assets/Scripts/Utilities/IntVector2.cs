using UnityEngine;

public class IntVector2
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

    public static implicit operator IntVector2(Vector2 source) => new IntVector2(source);
    public static implicit operator Vector2(IntVector2 source) => new Vector2(source.X, source.Y);
    public static implicit operator Vector3(IntVector2 source) => new Vector3(source.X, source.Y, 0);

    public override string ToString() => $"[{X} , {Y}]";

    public bool Equals(IntVector2 other) => X == other.X && Y == other.Y;
    public override bool Equals(object obj) => obj is IntVector2 ? Equals(obj as IntVector2) : false;

    public override int GetHashCode()
    {
        var hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        return hashCode;
    }
}
