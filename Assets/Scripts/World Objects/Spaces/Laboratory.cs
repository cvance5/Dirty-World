using System.Collections.Generic;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.PropGeneration;

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

            //Extents.Add(new IntVector2(minX, maxY));
            //Extents.Add(new IntVector2(maxX, maxY));
            //Extents.Add(new IntVector2(maxX, minY));
            //Extents.Add(new IntVector2(minX, minY));
        }

        public override Space GetContainingSpace(IntVector2 position)
        {
            var containingRegions = Regions.FindAll(region => region.Contains(position));

            foreach (var containingRegion in containingRegions)
            {
                foreach (var space in containingRegion.Spaces)
                {
                    if (space.Extents.Contains(position))
                    {
                        return space;
                    }
                }
            }

            return null;
        }

        public override PropTypes GetProp(IntVector2 position)
        {
            var containingRegions = Regions.FindAll(region => region.Contains(position));
            foreach (var containingRegion in containingRegions)
            {
                var containingSpace = containingRegion.Spaces.Find(space => space.Extents.Contains(position));
                if (containingSpace != null)
                {
                    return containingSpace.GetProp(position);
                }
            }

            return PropTypes.None;
        }

        public override BlockTypes GetBlockType(IntVector2 position)
        {
            if (!Extents.Contains(position)) throw new System.ArgumentOutOfRangeException($"{Name} does not contain {position}.  Cannot get block.");
            else if (_blockOverride.TryGetValue(position, out var overrideType))
            {
                return overrideType;
            }
            else
            {
                var containingRegions = Regions.FindAll(region => region.Contains(position));
                foreach (var containingRegion in containingRegions)
                {
                    var containingSpace = containingRegion.Spaces.Find(space => space.Extents.Contains(position));
                    if (containingSpace != null)
                    {
                        return containingSpace.GetBlockType(position);
                    }
                }
                return BlockTypes.SteelPlate;
            }
        }

        public override IntVector2 GetRandomPosition() => Regions.RandomItem().Spaces.RandomItem().GetRandomPosition();
    }
}