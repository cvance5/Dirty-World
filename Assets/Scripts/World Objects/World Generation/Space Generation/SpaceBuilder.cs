using System.Collections.Generic;
using Utilities.Debug;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public abstract class SpaceBuilder
    {
        public abstract bool IsValid { get; }

        protected ChunkBuilder _chunkBuilder { get; private set; }
        protected Dictionary<IntVector2, int> _boundedDirections { get; private set; } =
              new Dictionary<IntVector2, int>();

        private readonly List<SpaceModifier> _modifiersApplied = new List<SpaceModifier>();

        public SpaceBuilder(ChunkBuilder chunkBuilder) => _chunkBuilder = chunkBuilder;

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
            switch (modifier)
            {
                case ModifierTypes.Cavernous:
                    _modifiersApplied.Add(new CavernousModifier(_chunkBuilder, this));
                    break;

                default: throw new System.ArgumentException($"Unknown modifier of type `{modifier}`.  Cannot construct.");
            }

            return this;
        }

        public abstract void Shift(IntVector2 shift);

        public abstract int PassesBy(IntVector2 direction, int amount);
        public abstract bool Contains(IntVector2 point);

        public abstract IntVector2 GetRandomPoint();
        public abstract int GetMaximalValue(IntVector2 direction);

        public abstract SpaceBuilder Align(IntVector2 direction, int amount);
        public abstract void Clamp(IntVector2 direction, int amount);
        public abstract void Cut(IntVector2 direction, int amount);

        public Space Build()
        {
            var rawSpace = BuildRaw();

            foreach (var modifierApplied in _modifiersApplied)
            {
                modifierApplied.Modify(rawSpace);
            }

            return rawSpace;
        }

        protected abstract Space BuildRaw();

        private static readonly Log _log = new Log("SpaceBuilder");
    }
}