using WorldObjects.Blocks;
using WorldObjects.Hazards;

namespace WorldObjects.Spaces
{
    public class Shaft : Space
    {
        public override bool IsHazardous => false;

        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public Shaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;

            Extents.Add(BottomLeftCorner);
            Extents.Add(TopRightCorner);

            Name = $"Shaft from {BottomLeftCorner} to {TopRightCorner}.";
        }

        public override bool Contains(IntVector2 position) =>
            !(position.X < BottomLeftCorner.X ||
            position.Y < BottomLeftCorner.Y ||
            position.X > TopRightCorner.X ||
            position.Y > TopRightCorner.Y);

        public override BlockTypes GetBlock(IntVector2 position)
        {
            BlockTypes block = BlockTypes.None;

            if (position.Y == TopRightCorner.Y)
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