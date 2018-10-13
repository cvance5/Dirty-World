using System.Collections.Generic;
using WorldObjects;
using WorldObjects.Spaces;

namespace Data
{
    public class PositionData
    {
        public Chunk Chunk;
        public Space Space;
        public IntVector2 Position;

        public PositionData() { }

        public PositionData(Chunk chunk, Space space, IntVector2 position)
        {
            Chunk = chunk;
            Space = space;
            Position = position;
        }

        public override string ToString() => $"Chunk: {Chunk} Space: {Space}";

        public override bool Equals(object obj)
        {
            var data = obj as PositionData;
            return data != null &&
                   EqualityComparer<Chunk>.Default.Equals(Chunk, data.Chunk) &&
                   EqualityComparer<Space>.Default.Equals(Space, data.Space);
        }

        public static bool operator ==(PositionData lhs, object rhs) => rhs == null ? false : lhs.Equals(rhs);
        public static bool operator !=(PositionData lhs, object rhs) => !(lhs == rhs);

        public override int GetHashCode()
        {
            var hashCode = -190340137;
            hashCode = hashCode * -1521134295 + EqualityComparer<Chunk>.Default.GetHashCode(Chunk);
            hashCode = hashCode * -1521134295 + EqualityComparer<Space>.Default.GetHashCode(Space);
            return hashCode;
        }
    }
}