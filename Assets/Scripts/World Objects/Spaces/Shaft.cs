using UnityEngine;
using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class Shaft : Space
    {
        public override string Name => $"Shaft from {BottomLeftCorner} to {TopRightCorner}.";

        public bool IsUncapped { get; }

        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Height => TopRightCorner.Y - BottomLeftCorner.Y;
        public int Width => TopRightCorner.X - BottomLeftCorner.X;
        public override int Area => Height * Width;

        public Shaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, bool isUncapped)
        {
            IsUncapped = isUncapped;

            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;

            Extents = new System.Collections.Generic.List<IntVector2>()
            {
                BottomLeftCorner,
                new IntVector2(BottomLeftCorner.X, TopRightCorner.Y),
                TopRightCorner,
                new IntVector2(TopRightCorner.X, BottomLeftCorner.Y)
            };
        }

        public override bool Contains(IntVector2 position) =>
            position.X >= BottomLeftCorner.X &&
            position.Y >= BottomLeftCorner.Y &&
            position.X <= TopRightCorner.X &&
            position.Y <= TopRightCorner.Y;

        public override IntVector2 GetRandomPosition() =>
            new IntVector2(Random.Range(BottomLeftCorner.X, TopRightCorner.X),
                           Random.Range(BottomLeftCorner.Y, TopRightCorner.Y));

        public override BlockTypes GetBlockType(IntVector2 position)
        {
            if (!Contains(position)) throw new System.ArgumentOutOfRangeException($"{Name} does not contain {position}.  Cannot get block.");

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
    }
}