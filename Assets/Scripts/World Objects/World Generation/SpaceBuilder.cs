using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration
{
    public abstract class SpaceBuilder
    {
        protected ChunkBuilder _containingChunk { get; private set; }

        public SpaceBuilder(ChunkBuilder containingChunk) => _containingChunk = containingChunk;

        public abstract SpaceBuilder Clamp(IntVector2 direction, int amount);

        public abstract Space Build();
    }
}