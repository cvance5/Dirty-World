using WorldObjects.Blocks;

namespace WorldObjects.WorldGeneration
{
    public class BlockBuilder
    {
        public IntVector2 Position { get; private set; }
        public BlockTypes Type { get; private set; }
        public bool IsFill { get; private set; }

        private bool _exists;

        public BlockBuilder(IntVector2 position)
        {
            Position = position;
            Type = BlockTypes.Dirt;
            _exists = true;
            IsFill = true;
        }

        public BlockBuilder SetType(BlockTypes type)
        {
            Type = type;
            IsFill = false;
            return this;
        }

        public BlockBuilder Remove()
        {
            _exists = false;
            return this;
        }

        public BlockTypes Build()
        {
            var type = BlockTypes.None;

            if (_exists)
            {
                type = Type;
            }

            return type;
        }
    }
}