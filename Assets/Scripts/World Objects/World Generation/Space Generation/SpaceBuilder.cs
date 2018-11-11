using System.Collections.Generic;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public abstract class SpaceBuilder
    {
        protected ChunkBuilder _containingChunk { get; private set; }
        protected Dictionary<IntVector2, int> _boundedDirections { get; private set; } =
              new Dictionary<IntVector2, int>();

        private readonly List<ModifierTypes> _modifiersApplied = new List<ModifierTypes>();

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

        public SpaceBuilder AddModifier(ModifierTypes modifier)
        {
            if (!_modifiersApplied.Contains(modifier))
            {
                _modifiersApplied.Add(modifier);
            }

            return this;
        }

        protected abstract void Clamp(IntVector2 direction, int amount);
        protected abstract void Cut(IntVector2 direction, int amount);

        protected Space ApplyModifiers(Space builtSpace)
        {
            foreach (var modifier in _modifiersApplied)
            {
                builtSpace = SpacePicker.ApplyModifier(builtSpace, modifier);
            }

            return builtSpace;
        }

        public abstract Space Build();
    }
}