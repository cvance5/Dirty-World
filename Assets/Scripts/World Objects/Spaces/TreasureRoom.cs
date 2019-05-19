using Items;
using MathConcepts;
using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class TreasureRoom : Room
    {
        public override string Name => $"Treasure {base.Name}";

        public readonly Item[] Treasure;

        private readonly IntVector2 _chestPosition;

        public TreasureRoom(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, Item[] treasure)
            : base(bottomLeftCorner, topRightCorner)
        {
            Treasure = treasure;

            _chestPosition = new IntVector2((bottomLeftCorner.X + topRightCorner.X) / 2, (bottomLeftCorner.Y + topRightCorner.Y) / 2);
        }
    }
}