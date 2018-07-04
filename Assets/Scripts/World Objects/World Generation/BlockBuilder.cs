namespace WorldObjects.WorldGeneration
{
    public class BlockBuilder
    {
        private bool _exists;
        private BlockTypes _type;
        public IntVector2 WorldPosition { get; private set; }

        public BlockBuilder(IntVector2 worldPosition)
        {
            WorldPosition = worldPosition;
            _type = BlockTypes.Dirt;
            _exists = true;
        }

        public BlockBuilder SetType(BlockTypes type)
        {
            _type = type;
            return this;
        }

        public BlockBuilder Remove()
        {
            _exists = false;
            return this;
        }

        public BlockTypes Build()
        {
            BlockTypes type = BlockTypes.None;

            if (_exists)
            {
                type = _type;
            }

            return type;
        }
    }
}