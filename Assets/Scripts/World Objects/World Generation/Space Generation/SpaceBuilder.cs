using MathConcepts;
using System.Collections.Generic;
using System.Linq;
using Utilities.Debug;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public abstract class SpaceBuilder : IBoundary
    {
        public SmartEvent<SpaceBuilder> OnSpaceBuilderChanged = new SmartEvent<SpaceBuilder>();

        public abstract bool IsValid { get; }
        public string Name { get; } = SpaceNamer.GetNext();

        protected ChunkBuilder _chunkBuilder { get; private set; }

        public IntVector2 Origin { get; protected set; }

        protected Dictionary<IntVector2, int> _boundedDirections { get; private set; } =
              new Dictionary<IntVector2, int>();

        protected Dictionary<IntVector2, int> _maximalValues { get; private set; } =
              new Dictionary<IntVector2, int>();

        public Dictionary<IntVector2, int> MaximalValues => new Dictionary<IntVector2, int>(_maximalValues);

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

                // We are bounded in both sides and can't just shift that way
                if (_boundedDirections.ContainsKey(-direction))
                {
                    do
                    {
                        foreach (var boundedDirection in _boundedDirections)
                        {
                            Squash(boundedDirection.Key, boundedDirection.Value);
                        }
                    } while (IsValid && _boundedDirections.Any(boundedDir => DistanceFrom(boundedDir.Key, boundedDir.Value) > 0));
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

        public void Shift(IntVector2 shift)
        {
            Origin += shift;
            Recalculate();
        }

        public bool IntersectsWith(SpaceBuilder other) =>
               !(other is null) &&
               // Can't intersect with yourself!
               this != other &&
             // If any of my edges is beyond your opposite edge, we do not intersect
             // Otherwise, we do.
             // This method only works if we don't have spaces with gaps in the middle
             !(_maximalValues[Directions.Up] <= other._maximalValues[Directions.Down] ||
               _maximalValues[Directions.Right] <= other._maximalValues[Directions.Left] ||
               _maximalValues[Directions.Down] >= other._maximalValues[Directions.Up] ||
               _maximalValues[Directions.Left] >= other._maximalValues[Directions.Right]);

        public int DistanceFrom(IntVector2 direction, int amount)
        {
            if (!_maximalValues.TryGetValue(direction, out var maximalValue))
            {
                throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
            }

            var difference = 0;
            if (direction == Directions.Up || direction == Directions.Right)
            {
                difference = maximalValue - amount;
            }
            else if (direction == Directions.Down || direction == Directions.Left)
            {
                difference = amount - maximalValue;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            return difference;
        }

        public SpaceBuilder Align(IntVector2 direction, int amount)
        {
            var difference = DistanceFrom(direction, amount);
            Shift(direction * -difference);

            return this;
        }

        public void Clamp(IntVector2 direction, int amount)
        {
            var difference = DistanceFrom(direction, amount);

            if (difference > 0)
            {
                Shift(direction * -difference);
            }
        }

        public Space Build()
        {
            if (!IsValid) throw new System.InvalidOperationException($"SpaceBuilder {Name} is not valid and should not be built.");

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

        public abstract bool Contains(IntVector2 point);
        public abstract void Squash(IntVector2 direction, int amount);

        protected abstract Space BuildRaw();
        protected abstract void Recalculate();

        private static readonly Log _log = new Log("SpaceBuilder");
    }
}