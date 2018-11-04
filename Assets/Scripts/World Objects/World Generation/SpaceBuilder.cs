using System.Collections.Generic;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration
{
    public abstract class SpaceBuilder
    {
        protected ChunkBuilder _containingChunk { get; private set; }
        protected Dictionary<IntVector2, int> _boundedDirections { get; private set; } =
              new Dictionary<IntVector2, int>();

        public SpaceBuilder(ChunkBuilder containingChunk) => _containingChunk = containingChunk;

        public SpaceBuilder AddBoundary(IntVector2 direction, int amount)
        {
            _boundedDirections[direction] = amount;

            Clamp(direction, amount);

            if (_boundedDirections.ContainsKey(-direction))
            {
                // We are bounded in both sides and can't just shift that way
                Cut(-direction, _boundedDirections[-direction]);
            }

            return this;
        }

        protected abstract void Clamp(IntVector2 direction, int amount);
        protected abstract void Cut(IntVector2 direction, int amount);

        public abstract Space Build();
    }
}