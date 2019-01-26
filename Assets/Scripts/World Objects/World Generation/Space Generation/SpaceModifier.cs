using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public abstract class SpaceModifier
    {
        protected ChunkBuilder _chunkBuilder { get; private set; }
        protected SpaceBuilder _spaceBuilder { get; private set; }

        public abstract ModifierTypes Type { get; }

        public SpaceModifier(ChunkBuilder chunkBuilder, SpaceBuilder spaceBuilder)
        {
            _chunkBuilder = chunkBuilder;
            _spaceBuilder = spaceBuilder;
        }

        public abstract void Modify(Space target);
    }
}