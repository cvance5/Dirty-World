using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace WorldObjects.WorldGeneration
{
    public class BlockBuilder
    {
        public IntVector2 Position { get; private set; }
        public bool IsFill { get; private set; }
        public Space Space { get; private set; }

        private bool _exists;

        public BlockTypes _block = BlockTypes.None;
        public FeatureTypes _feature = FeatureTypes.None;

        public BlockBuilder(IntVector2 position)
        {
            Position = position;
            _exists = true;
            IsFill = true;
        }

        public BlockBuilder SetType(BlockTypes type)
        {
            _block = type;
            IsFill = false;
            return this;
        }

        public BlockBuilder SetSpace(Space space)
        {
            Space = space;
            SetType(space.GetBlockType(Position));
            SetFeature(space.GetFeatureType(Position));
            return this;
        }

        public BlockBuilder SetFeature(FeatureTypes feature)
        {
            _feature = feature;
            return this;
        }

        public BlockBuilder Remove()
        {
            _exists = false;
            return this;
        }

        public BlockTypes GetBlock()
        {
            var type = BlockTypes.None;

            if (_exists)
            {
                type = _block;
            }

            return type;
        }

        public FeatureTypes GetFeature()
        {
            var type = FeatureTypes.None;

            if (_exists)
            {
                type = _feature;
            }

            return type;
        }
    }
}