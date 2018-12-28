using Items;
using UnityEngine;
using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class TreasureRoom : Space
    {
        public override string Name => $"Treasure Room from {BottomLeftCorner} to {TopRightCorner}";

        public readonly IntVector2 BottomLeftCorner;
        public readonly IntVector2 TopRightCorner;

        public readonly Item[] Treasure;

        private readonly IntVector2 _chestPosition;

        public TreasureRoom(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, Item[] treasure)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;
            Treasure = treasure;

            _chestPosition = new IntVector2((bottomLeftCorner.X + topRightCorner.X) / 2, (bottomLeftCorner.Y + topRightCorner.Y) / 2);

            Extents.Add(BottomLeftCorner);
            Extents.Add(new IntVector2(BottomLeftCorner.X, TopRightCorner.Y));
            Extents.Add(TopRightCorner);
            Extents.Add(new IntVector2(TopRightCorner.X, BottomLeftCorner.Y));
        }

        public override bool Contains(IntVector2 position) =>
            position.X >= BottomLeftCorner.X &&
            position.Y >= BottomLeftCorner.Y &&
            position.X <= TopRightCorner.X &&
            position.Y <= TopRightCorner.Y;

        public override BlockTypes GetBlockType(IntVector2 position) => position == _chestPosition ? BlockTypes.Gold : BlockTypes.None;

        public override IntVector2 GetRandomPosition() =>
            new IntVector2(Random.Range(BottomLeftCorner.X, TopRightCorner.X),
                           Random.Range(BottomLeftCorner.Y, TopRightCorner.Y));
    }
}