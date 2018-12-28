using System.Collections.Generic;
using System.Linq;
using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class Laboratory : ComplexSpace
    {
        public override string Name => $"Laboratory";

        public readonly int MetalThickness;

        public Laboratory(List<Space> containedSpaces, int metalThickeness)
        {
            ContainedSpaces = containedSpaces;
            MetalThickness = metalThickeness;

            int? maxX = null;
            int? minX = null;
            int? maxY = null;
            int? minY = null;

            foreach (var space in containedSpaces)
            {
                foreach (var extent in space.Extents)
                {
                    if (maxX == null || extent.X > maxX.Value)
                    {
                        maxX = extent.X;
                    }
                    if (minX == null || extent.X < minX.Value)
                    {
                        minX = extent.X;
                    }
                    if (maxY == null || extent.Y > maxY.Value)
                    {
                        maxY = extent.Y;
                    }
                    if (minY == null || extent.Y < minY.Value)
                    {
                        minY = extent.Y;
                    }
                }
            }

            Extents.Add(new IntVector2(minX.Value, maxY.Value));
            Extents.Add(new IntVector2(maxX.Value, maxY.Value));
            Extents.Add(new IntVector2(maxX.Value, minY.Value));
            Extents.Add(new IntVector2(minX.Value, minY.Value));
        }

        public override bool Contains(IntVector2 position)
        {
            var doesContain = false;

            if (ContainedSpaces.Any(space => space.Contains(position)))
            {
                doesContain = true;
            }
            else
            {
                foreach (var direction in Directions.Compass)
                {
                    if (ContainedSpaces.Any(space => space.Contains(position + (direction * MetalThickness))))
                    {
                        doesContain = true;
                        break;
                    }
                }
            }

            return doesContain;
        }

        public override BlockTypes GetBlockType(IntVector2 position)
        {
            var containingSpace = ContainedSpaces.Find(space => space.Contains(position));

            if (containingSpace != null)
            {
                return containingSpace.GetBlockType(position);
            }
            else return BlockTypes.SteelPlate;
        }

        public override IntVector2 GetRandomPosition() => ContainedSpaces.RandomItem().GetRandomPosition();
    }
}