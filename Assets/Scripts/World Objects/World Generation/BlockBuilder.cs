using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.PropGeneration;

namespace WorldObjects.WorldGeneration
{
    public class BlockBuilder
    {
        public IntVector2 Position { get; private set; }
        public bool IsFill { get; private set; }
        public Space Space { get; private set; }

        private bool _exists;

        public BlockTypes _block = BlockTypes.None;
        public PropTypes _prop = PropTypes.None;

        public BlockBuilder(IntVector2 position)
        {
            Position = position;
            _exists = true;
            IsFill = true;
        }

        public BlockBuilder SetType(BlockTypes type)
        {
            if (Space == null)
            {
                _block = type;
                IsFill = false;
            }
            return this;
        }

        public BlockBuilder SetSpace(Space space)
        {
            Space = space;

            _block = space.GetBlockType(Position);
            IsFill = false;

            SetProp(space.GetProp(Position));
            return this;
        }

        public BlockBuilder SetProp(PropTypes prop)
        {
            _prop = prop;
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

        public PropTypes GetProp()
        {
            var type = PropTypes.None;

            if (_exists)
            {
                type = _prop;
            }

            return type;
        }
    }
}