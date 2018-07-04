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

        public Block Build()
        {
            Block block = null;

            if (_exists)
            {
                var blockObject = BlockLoader.CreateBlock(_type, WorldPosition);
                block = blockObject.GetComponent<Block>();
            }

            return block;
        }
    }
}