namespace WorldObjects.WorldGeneration
{
    public class BlockBuilder
    {
        public IntVector2 WorldPosition { get; private set; }
        public bool IsFill { get; private set; }

        private bool _exists;
        private BlockTypes _type;

        public BlockBuilder(IntVector2 worldPosition)
        {
            WorldPosition = worldPosition;
            _type = BlockTypes.Dirt;
            _exists = true;
            IsFill = true;
        }

        public BlockBuilder SetType(BlockTypes type)
        {
            _type = type;
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
            BlockTypes type = BlockTypes.None;

            if (_exists)
            {
                type = _type;
            }

            return type;
        }
    }
}