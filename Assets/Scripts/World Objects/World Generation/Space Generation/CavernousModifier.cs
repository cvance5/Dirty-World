using MathConcepts;
using System.Collections.Generic;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.HazardGeneration;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class CavernousModifier : SpaceModifier
    {
        public override ModifierTypes Type => ModifierTypes.Cavernous;

        private Space _modifiedSpace;

        private readonly double _stalagDensity;

        public CavernousModifier(SpaceBuilder spaceBuilder)
            : base(spaceBuilder) => _stalagDensity = Chance.Range(0.05f, 0.15f);

        public override void Modify(Space modifiedSpace)
        {
            _modifiedSpace = modifiedSpace;

            var stalagBuilders = new List<HazardBuilder>();

            var totalTestedPositions = 0;

            foreach (var shape in _modifiedSpace.Extents.Shapes)
            {
                foreach (var segment in shape.Segments)
                {
                    var direction = (segment.End - segment.Start);
                    var unitPerStep = direction.Normalized;

                    for (var step = 0; step < direction.Magnitude; step++)
                    {
                        totalTestedPositions++;
                        var testPosition = new IntVector2(segment.Start + (unitPerStep * step));

                        // If this block is not already filled, check to see if we are at the top or bottom 
                        // edge of the space, and tell the StalagBuilder to go the other way.
                        if (!modifiedSpace.Extents.Contains(testPosition) || modifiedSpace.GetBlockType(testPosition) != BlockTypes.None) continue;
                        else if (modifiedSpace.Extents.Contains(testPosition + Directions.Down)) stalagBuilders.Add(new StalagBuilder(testPosition, Directions.Down, modifiedSpace));
                        else if (modifiedSpace.Extents.Contains(testPosition + Directions.Up)) stalagBuilders.Add(new StalagBuilder(testPosition, Directions.Up, modifiedSpace));
                    }
                }
            }

            var maxStalagNumber = totalTestedPositions * _stalagDensity;

            while (stalagBuilders.Count > maxStalagNumber)
            {
                stalagBuilders.RemoveAtRandom();
            }

            modifiedSpace.AddHazardBuilders(stalagBuilders);
        }
    }
}