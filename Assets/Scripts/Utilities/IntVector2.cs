using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class IntVector2
{
    [JsonProperty("X")]
    public int X;
    [JsonProperty("Y")]
    public int Y;

    [JsonConstructor]
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

    public float Magnitude => Mathf.Sqrt((X * X) + (Y * Y));
    public Vector2 Normalized
    {
        get
        {
            var mag = Magnitude;
            return new Vector2(X / mag, Y / mag);
        }
    }

    public static IntVector2 Zero => new IntVector2(0, 0);

    public static float Distance(IntVector2 pos1, IntVector2 pos2) => ((pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Y - pos2.Y) * (pos1.Y - pos2.Y));
    public static float Distance(IntVector2 pos1, Vector2 pos2) => ((pos1.X - pos2.x) * (pos1.X - pos2.x) + (pos1.Y - pos2.y) * (pos1.Y - pos2.y));
    public static IntVector2 Lerp(IntVector2 start, IntVector2 target, float time) => new IntVector2((int)Mathf.Lerp(start.X, target.X, time), (int)Mathf.Lerp(start.Y, target.Y, time));

    public static bool IsOnLine(IntVector2 lineStart, IntVector2 lineEnd, IntVector2 point)
    {
        // if AC is horizontal
        if (lineStart.X == point.X) return lineEnd.X == point.X;
        // if AC is vertical.
        if (lineStart.Y == point.Y) return lineEnd.Y == point.Y;
        // match the gradients
        return (lineStart.X - point.X) * (lineStart.Y - point.Y) == (point.X - lineEnd.X) * (point.Y - lineEnd.Y);
    }

    public static IntVector2 operator +(IntVector2 lhs, IntVector2 rhs) => new IntVector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
    public static IntVector2 operator +(IntVector2 lhs, Vector2 rhs) => lhs + new IntVector2(rhs);
    public static IntVector2 operator -(IntVector2 lhs, IntVector2 rhs) => new IntVector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
    public static IntVector2 operator -(IntVector2 vec) => new IntVector2(-vec.X, -vec.Y);

    public static IntVector2 operator *(IntVector2 vec, int operand) => new IntVector2(vec.X * operand, vec.Y * operand);
    public static IntVector2 operator /(IntVector2 vec, float operand) => new IntVector2(Mathf.CeilToInt(vec.X / operand), Mathf.CeilToInt(vec.Y / operand));

    public static bool operator ==(IntVector2 lhs, object rhs) => lhs is null ? rhs == null : lhs.Equals(rhs);
    public static bool operator !=(IntVector2 lhs, object rhs) => !(lhs == rhs);

    public static implicit operator IntVector2(Vector2 source) => new IntVector2(source);
    public static implicit operator Vector2(IntVector2 source) => new Vector2(source.X, source.Y);
    public static implicit operator Vector3(IntVector2 source) => new Vector3(source.X, source.Y, 0);

    public override string ToString() => $"[{X} , {Y}]";

    public bool Equals(IntVector2 other) => X == other.X && Y == other.Y;
    public override bool Equals(object obj) => obj is IntVector2 ? Equals(obj as IntVector2) : false;

    public override int GetHashCode()
    {
        var hashCode = 1861411795;
        hashCode *= -1521134295 + X.GetHashCode();
        hashCode *= -1521134295 + Y.GetHashCode();
        return hashCode;
    }
}
