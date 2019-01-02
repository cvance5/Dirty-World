using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
using Space = WorldObjects.Spaces.Space;

namespace Tools.SpaceCrafting
{
    public class LaboratoryCrafter : SpaceCrafter
    {
        public int MetalThickeness;

        public override bool IsValid => MetalThickeness >= 0 && BuildRegions().Count > 0;

        private int _minX;
        public override int MinX => _minX;

        private int _maxX;
        public override int MaxX => _maxX;

        private int _minY;
        public override int MinY => _minY;

        private int _maxY;
        public override int MaxY => _maxY;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Laboratory";

            MetalThickeness = 2;
        }

        protected override Space RawBuild() => new Laboratory(BuildRegions(), MetalThickeness);

        private List<Region> BuildRegions()
        {
            var regions = new List<Region>();

            foreach (var child in transform.GetAllChildren())
            {
                var mainShaft = child.GetComponent<ShaftCrafter>();

                var regionMinX = mainShaft.MinX;
                var regionMaxX = mainShaft.MaxX;

                var regionMinY = mainShaft.MinY;
                var regionMaxY = mainShaft.MaxY;

                var regionalSpaces = new List<Space>()
                {
                    mainShaft.Build()
                };

                foreach (var subcrafter in mainShaft.GetComponentsInChildren<SpaceCrafter>())
                {
                    regionalSpaces.Add(subcrafter.Build());

                    regionMinX = Mathf.Min(regionMinX, subcrafter.MinX);
                    regionMaxX = Mathf.Max(regionMaxX, subcrafter.MaxX);
                    regionMinY = Mathf.Min(regionMinY, subcrafter.MinY);
                    regionMaxY = Mathf.Max(regionMaxY, subcrafter.MaxY);
                }

                _minX = Mathf.Min(MinX, regionMinX);
                _maxX = Mathf.Max(MaxX, regionMaxX);
                _minY = Mathf.Min(MinY, regionMinY);
                _maxY = Mathf.Max(MaxY, regionMaxY);

                regions.Add(new Region(new IntVector2(MinX, MinY), new IntVector2(regionMaxX, MaxY), regionalSpaces));
            }

            return regions;
        }
    }
}