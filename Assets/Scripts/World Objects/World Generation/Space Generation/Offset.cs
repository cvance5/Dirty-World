using MathConcepts;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class Offset
    {
        public static readonly Offset IDENTITY = new Offset(IntVector2.Zero, Quaternion.identity);

        public IntVector2 Position { get; }
        public Quaternion Rotation { get; }

        public Offset(IntVector2 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Offset(int x, int y, float rotation)
            : this(new IntVector2(x, y), Quaternion.Euler(0, 0, rotation)) { }

        public override bool Equals(object obj) =>
                   obj is Offset offset &&
                   EqualityComparer<IntVector2>.Default.Equals(Position, offset.Position) &&
                   Rotation.Equals(offset.Rotation);

        public override int GetHashCode()
        {
            var hashCode = -388643783;
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(Position);
            hashCode = hashCode * -1521134295 + EqualityComparer<Quaternion>.Default.GetHashCode(Rotation);
            return hashCode;
        }
    }
}