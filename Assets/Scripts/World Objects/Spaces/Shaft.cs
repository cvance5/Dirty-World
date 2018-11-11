using WorldObjects.Blocks;
using WorldObjects.Hazards;

namespace WorldObjects.Spaces
{
    public class Shaft : Space
    {
        public override string Name => $"Shaft from {BottomLeftCorner} to {TopRightCorner}.";

        public override bool IsHazardous => false;
        public bool IsUncapped { get; }
        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Height => TopRightCorner.Y - BottomLeftCorner.Y;
        public int Width => TopRightCorner.X - BottomLeftCorner.X;

        public Shaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, bool isUncapped)
        {
            IsUncapped = isUncapped;

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

        public override BlockTypes GetBlock(IntVector2 position)
        {
            var block = BlockTypes.None;

            if (!IsUncapped &&
                position.Y == TopRightCorner.Y)
            {
                if (Chance.CoinFlip)
                {
                    block = BlockTypes.Stone;
                }
                else
                {
                    block = BlockTypes.Dirt;
                }
            }

            return block;
        }

        public override HazardTypes GetHazard(IntVector2 position) => HazardTypes.None;
    }
}