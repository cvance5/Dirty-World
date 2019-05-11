using System.Collections.Generic;
using Utilities.Debug;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public abstract class SpaceBuilder : IBoundary
    {
        public static SmartEvent<SpaceBuilder> OnSpaceBuilderChanged = new SmartEvent<SpaceBuilder>();

        public abstract bool IsValid { get; }

        protected ChunkBuilder _chunkBuilder { get; private set; }
        protected Dictionary<IntVector2, int> _boundedDirections { get; private set; } =
              new Dictionary<IntVector2, int>();

        protected readonly List<ModifierTypes> _modifiersApplied = new List<ModifierTypes>();

        // Used reflectively via Activator
        public SpaceBuilder(ChunkBuilder chunkBuilder) => _chunkBuilder = chunkBuilder;
        public SpaceBuilder(SpaceBuilder spaceToCopy) => _chunkBuilder = spaceToCopy._chunkBuilder;

        public SpaceBuilder AddBoundary(IntVector2 direction, int amount)
        {
            // Tightness is the "magnitude" of the boundary, regardless of direction
            // It makes it so that smaller numbers always equal a deeper restriction in the 
            // given direction, despite the fact that some X/Y coordinates are negative to begin with
            if (IsStrongerBoundary(direction, amount))
            {
                _boundedDirections[direction] = amount;

                Clamp(direction, amount);

                if (_boundedDirections.ContainsKey(-direction))
                {
                    // We are bounded in both sides and can't just shift that way
                    Cut(-direction, _boundedDirections[-direction]);
                }
            }

            return this;
        }

        public SpaceBuilder AddModifiers(List<ModifierTypes> modifiersApplied)
        {
            _modifiersApplied.AddRange(modifiersApplied);
            return this;
        }

        public SpaceBuilder AddModifier(ModifierTypes modifier)
        {
            _modifiersApplied.Add(modifier);
            return this;
        }

        public bool IsStrongerBoundary(IntVector2 direction, int amount)
        {
            if (_boundedDirections.TryGetValue(direction, out var existingBoundaryTightness))
            {
                if (direction == Directions.Up || direction == Directions.Right)
                {
                    return amount < existingBoundaryTightness;
                }
                else if (direction == Directions.Down || direction == Directions.Left)
                {
                    return amount > existingBoundaryTightness;
                }
            }

            return true;
        }

        public abstract void Shift(IntVector2 shift);

        public abstract int PassesBy(IntVector2 direction, int amount);
        public abstract bool Contains(IntVector2 point);
        public bool IntersectsWith(SpaceBuilder other) =>
               !(other is null) &&
               // Can't intersect with yourself!
               this != other &&
             // If any of my edges is beyond your opposite edge, we do not intersect
             // Otherwise, we do.
             // This method only works if we don't have spaces with gaps in the middle
             !(GetMaximalValue(Directions.Up) <= other.GetMaximalValue(Directions.Down) ||
               GetMaximalValue(Directions.Right) <= other.GetMaximalValue(Directions.Left) ||
               GetMaximalValue(Directions.Down) >= other.GetMaximalValue(Directions.Up) ||
               GetMaximalValue(Directions.Left) >= other.GetMaximalValue(Directions.Right));


        public abstract IntVector2 GetRandomPoint();
        public abstract int GetMaximalValue(IntVector2 direction);

        public abstract SpaceBuilder Align(IntVector2 direction, int amount);
        public abstract void Clamp(IntVector2 direction, int amount);
        public abstract void Cut(IntVector2 direction, int amount);

        public Space Build()
        {
            var rawSpace = BuildRaw();

            foreach (var modifierType in _modifiersApplied)
            {
                SpaceModifier modifier;
                switch (modifierType)
                {
                    case ModifierTypes.Cavernous:
                        modifier = new CavernousModifier(this);
                        break;

                    default: throw new System.ArgumentException($"Unknown modifier of type `{modifierType}`.  Cannot construct.");
                }


                modifier.Modify(rawSpace);
                rawSpace.AddModifier(modifierType);
            }

            return rawSpace;
        }

        protected abstract Space BuildRaw();

        private static readonly Log _log = new Log("SpaceBuilder");
    }
}