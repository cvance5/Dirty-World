using System.Collections.Generic;
using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class Laboratory : ComplexSpace
    {
        public override string Name => $"Laboratory";

        public readonly int MetalThickness;

        public Laboratory(List<Region> regions, int metalThickeness)
        {
            Regions = regions;
            MetalThickness = metalThickeness;

            int? maxX = null;
            int? minX = null;
            int? maxY = null;
            int? minY = null;

            foreach (var region in regions)
            {
                if (minX == null || region.BottomLeftCorner.X < minX)
                {
                    minX = region.BottomLeftCorner.X;
                }
                if (minY == null || region.BottomLeftCorner.Y < minY)
                {
                    minY = region.BottomLeftCorner.Y;
                }
                if (maxX == null || region.TopRightCorner.X > maxX)
                {
                    maxX = region.TopRightCorner.X;
                }
                if (maxY == null || region.TopRightCorner.Y > maxY)
                {
                    maxY = region.TopRightCorner.Y;
                }
            }

            Extents.Add(new IntVector2(minX.Value, maxY.Value));
            Extents.Add(new IntVector2(maxX.Value, maxY.Value));
            Extents.Add(new IntVector2(maxX.Value, minY.Value));
            Extents.Add(new IntVector2(minX.Value, minY.Value));
        }

        public override bool Contains(IntVector2 position)
        {
            var containingRegions = Regions.FindAll(region => region.Contains(position));

            foreach (var containingRegion in containingRegions)
            {
                foreach (var space in containingRegion.Spaces)
                {
                    if (space.Contains(position))
                    {
                        return true;
                    }
                    else
                    {
                        foreach (var direction in Directions.Compass)
                        {
                            if (space.Contains(position + (direction * MetalThickness)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override BlockTypes GetBlockType(IntVector2 position)
        {
            var containingRegions = Regions.FindAll(region => region.Contains(position));
            foreach (var containingRegion in containingRegions)
            {
                var containingSpace = containingRegion.Spaces.Find(space => space.Contains(position));
                if (containingSpace != null)
                {
                    return containingSpace.GetBlockType(position);
                }
            }

            return BlockTypes.SteelPlate;
        }

        public override IntVector2 GetRandomPosition() => Regions.RandomItem().Spaces.RandomItem().GetRandomPosition();
    }
}