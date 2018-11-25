using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class Corridor : Space
    {
        public override string Name => $"Corridor from {BottomLeftCorner} to {TopRightCorner}.";

        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Height => TopRightCorner.Y - BottomLeftCorner.Y;
        public int Length => TopRightCorner.X - BottomLeftCorner.X;
        public override int Area => Height * Length;

        public Corridor(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;

            Extents.Add(BottomLeftCorner);
            Extents.Add(new IntVector2(BottomLeftCorner.X, TopRightCorner.Y));
            Extents.Add(TopRightCorner);
            Extents.Add(new IntVector2(TopRightCorner.X, BottomLeftCorner.Y));
        }

        public override bool Contains(IntVector2 position) =>
            !(position.X < BottomLeftCorner.X ||
            position.Y < BottomLeftCorner.Y ||
            position.X > TopRightCorner.X ||
            position.Y > TopRightCorner.Y);

        public override IntVector2 GetRandomPosition() =>
            new IntVector2(UnityEngine.Random.Range(BottomLeftCorner.X, TopRightCorner.X),
                           UnityEngine.Random.Range(BottomLeftCorner.Y, TopRightCorner.Y));


        public override BlockTypes GetBlockType(IntVector2 position)
        {
            if (!Contains(position))
            {
                throw new System.ArgumentOutOfRangeException($"{Name} does not contain {position}.  Cannot get block.");
            }
            return BlockTypes.None;
        }
    }
}