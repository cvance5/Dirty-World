using MathConcepts;
using WorldObjects.Hazards;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.HazardGeneration
{
    public abstract class HazardBuilder
    {
        public IntVector2 Position { get; private set; }

        public HazardBuilder(IntVector2 position) => Position = position;

        public abstract Hazard Build(Chunk containingChunk, Space containingSpace);
    }
}