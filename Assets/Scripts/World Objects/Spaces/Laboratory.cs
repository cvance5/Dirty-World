using System.Collections.Generic;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace WorldObjects.Spaces
{
    public class Laboratory : ComplexSpace
    {
        public override string Name => $"Laboratory with Extents {Extents[0]}, {Extents[1]}, {Extents[2]}, {Extents[3]}";

        public readonly int MetalThickness;

        public Laboratory(List<Region> regions, int metalThickeness)
        {
            Regions = regions;
            MetalThickness = metalThickeness;

            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;

            foreach (var region in regions)
            {
                if (region.BottomLeftCorner.X < minX)
                {
                    minX = region.BottomLeftCorner.X;
                }
                if (region.TopRightCorner.X > maxX)
                {
                    maxX = region.TopRightCorner.X;
                }
                if (region.TopRightCorner.Y > maxY)
                {
                    maxY = region.TopRightCorner.Y;
                }
                if (region.BottomLeftCorner.Y < minY)
                {
                    minY = region.BottomLeftCorner.Y;
                }
            }

            Extents.Add(new IntVector2(minX, maxY));
            Extents.Add(new IntVector2(maxX, maxY));
            Extents.Add(new IntVector2(maxX, minY));
            Extents.Add(new IntVector2(minX, minY));
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

        public override Space GetContainingSpace(IntVector2 position)
        {
            var containingRegions = Regions.FindAll(region => region.Contains(position));

            foreach (var containingRegion in containingRegions)
            {
                foreach (var space in containingRegion.Spaces)
                {
                    if (space.Contains(position))
                    {
                        return space;
                    }
                }
            }

            return null;
        }

        public override FeatureTypes GetFeatureType(IntVector2 position)
        {
            var containingRegions = Regions.FindAll(region => region.Contains(position));
            foreach (var containingRegion in containingRegions)
            {
                var containingSpace = containingRegion.Spaces.Find(space => space.Contains(position));
                if (containingSpace != null)
                {
                    return containingSpace.GetFeatureType(position);
                }
            }

            return FeatureTypes.None;
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