using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration
{
    public abstract class SpaceBuilder
    {
        public SpaceBuilder(ChunkBuilder containingChunk) { }

        public abstract SpaceBuilder Clamp(IntVector2 direction, int amount);

        public abstract Space Build();
    }
}