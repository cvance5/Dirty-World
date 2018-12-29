using Items;
using WorldObjects.Spaces;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class TreasureRoomBuilder : RoomBuilder
    {
        public override bool IsValid => _size >= _minimumSize && _treasure != null;

        private Item[] _treasure;

        public TreasureRoomBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder) => _treasure = null;

        public TreasureRoomBuilder(RoomBuilder roomToPromote)
            : base(roomToPromote) => _treasure = null;

        public TreasureRoomBuilder SetTreasure(params Item[] treasure)
        {
            _treasure = treasure;

            return this;
        }

        protected override Space BuildRaw() => new TreasureRoom(_bottomLeftCorner, _topRightCorner, _treasure);
    }
}