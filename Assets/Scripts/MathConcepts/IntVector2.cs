using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MathConcepts
{
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

        public IntVector2(float x, float y)
            : this(Mathf.RoundToInt(x), Mathf.RoundToInt(y)) { }

        public IntVector2(Vector2 vec)
        {
            X = Mathf.RoundToInt(vec.x);
            Y = Mathf.RoundToInt(vec.y);
        }

        public IntVector2(IntVector2 other)
        {
            X = other.X;
            Y = other.Y;
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

        public static bool IsOnLine(IntVector2 lineSampleA, IntVector2 lineSampleB, IntVector2 point)
        {
            // if AC is horizontal
            if (lineSampleA.X == point.X) return lineSampleB.X == point.X;
            // if AC is vertical.
            if (lineSampleA.Y == point.Y) return lineSampleB.Y == point.Y;
            // match the gradients
            return (lineSampleA.X - point.X) * (lineSampleA.Y - point.Y) == (point.X - lineSampleB.X) * (point.Y - lineSampleB.Y);
        }

        // https://stackoverflow.com/questions/42868214/determine-if-a-point-is-between-2-other-points-on-a-line
        public static bool IsBetween(IntVector2 lineStart, IntVector2 lineEnd, IntVector2 point)
        {
            if (point == lineStart || point == lineEnd) return true;

            var isOnLine = IsOnLine(lineStart, lineEnd, point);

            if (!isOnLine) return false;
            else
            {
                var dxc = point.X - lineStart.X;
                var dyc = point.Y - lineStart.Y;

                var dxl = lineEnd.X - lineStart.X;
                var dyl = lineEnd.Y - lineStart.Y;

                if (Mathf.Abs(dxl) >= Mathf.Abs(dyl))
                    return dxl > 0 ?
                      lineStart.X <= point.X && point.X <= lineEnd.X :
                      lineEnd.X <= point.X && point.X <= lineStart.X;
                else return dyl > 0 ?
                      lineStart.Y <= point.Y && point.Y <= lineEnd.Y :
                      lineEnd.Y <= point.Y && point.Y <= lineStart.Y;
            }
        }

        public static IntVector2 Nearest(IntVector2 position, List<IntVector2> options)
        {
            var nearestDistance = Distance(position, options[0]);
            var nearest = options[0];

            foreach (var option in options)
            {
                var distance = Distance(position, option);
                if (distance < nearestDistance)
                {
                    nearest = option;
                    nearestDistance = distance;
                }
            }

            return nearest;
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
}