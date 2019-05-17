using MathConcepts;
using System.Collections.Generic;
using WorldObjects;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;

namespace Data
{
    public class PositionData
    {
        public Chunk Chunk;
        public ChunkBuilder Builder;
        public Space Space { get; }
        public IntVector2 Position { get; }

        public PositionData() { }

        public PositionData(Space space, IntVector2 position)
        {
            Space = space;
            Position = position;
        }

        public override string ToString() => $"Chunk: {Chunk} Space: {Space}";

        public override bool Equals(object obj)
        {
            var data = obj as PositionData;
            return data != null &&
                   EqualityComparer<Chunk>.Default.Equals(Chunk, data.Chunk) &&
                   EqualityComparer<ChunkBuilder>.Default.Equals(Builder, data.Builder) &&
                   EqualityComparer<Space>.Default.Equals(Space, data.Space) &&
                   EqualityComparer<IntVector2>.Default.Equals(Position, data.Position);
        }

        public override int GetHashCode()
        {
            var hashCode = -659631415;
            hashCode = hashCode * -1521134295 + EqualityComparer<Chunk>.Default.GetHashCode(Chunk);
            hashCode = hashCode * -1521134295 + EqualityComparer<ChunkBuilder>.Default.GetHashCode(Builder);
            hashCode = hashCode * -1521134295 + EqualityComparer<Space>.Default.GetHashCode(Space);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(Position);
            return hashCode;
        }

        public static bool operator ==(PositionData lhs, object rhs) => rhs == null ? false : lhs.Equals(rhs);
        public static bool operator !=(PositionData lhs, object rhs) => !(lhs == rhs);
    }
}