using Items;
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

        public override BlockTypes GetBlockType(IntVector2 position)
        {
            if (!Extents.Contains(position)) throw new System.ArgumentOutOfRangeException($"{Name} does not contain {position}.  Cannot get block.");
            else if (_blockOverride.TryGetValue(position, out var overrideType))
            {
                return overrideType;
            }
            else return position == _chestPosition ? BlockTypes.Gold : BlockTypes.None;
        }
    }
}