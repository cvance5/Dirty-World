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

        public override void InitializeFromSpace(Space space)
        {
            var laboratory = space as Laboratory;
            foreach (var region in laboratory.Regions)
            {
                var mainShaft = region.Spaces[0];
                var mainShaftCrafter = SpaceCraftingManager.Instance.AddNewCrafter<ShaftCrafter>();

                mainShaftCrafter.transform.SetParent(transform);
                mainShaftCrafter.InitializeFromSpace(mainShaft);
                mainShaftCrafter.InitializeEnemySpawns(mainShaft.EnemySpawns);

                for (var spaceNumber = 1; spaceNumber < region.Spaces.Count; spaceNumber++)
                {
                    var regionSpace = region.Spaces[spaceNumber];
                    SpaceCrafter crafter;

                    if (regionSpace is Shaft)
                    {
                        crafter = SpaceCraftingManager.Instance.AddNewCrafter<ShaftCrafter>();
                    }
                    else if (regionSpace is Corridor)
                    {
                        crafter = SpaceCraftingManager.Instance.AddNewCrafter<CorridorCrafter>();
                    }
                    else if (regionSpace is Room)
                    {
                        crafter = SpaceCraftingManager.Instance.AddNewCrafter<RoomCrafter>();
                    }
                    else throw new System.ArgumentOutOfRangeException($"Unhandled space of type {region.GetType()}.");

                    crafter.transform.SetParent(mainShaftCrafter.transform);
                    crafter.InitializeFromSpace(regionSpace);
                    crafter.InitializeEnemySpawns(regionSpace.EnemySpawns);
                }
            }

            InitializeEnemySpawns(laboratory.EnemySpawns);
        }

        protected override Space RawBuild() => new Laboratory(BuildRegions(), MetalThickeness);

        private List<Region> BuildRegions()
        {
            var regions = new List<Region>();

            foreach (var child in transform.GetChildren())
            {
                var mainShaft = child.GetComponent<ShaftCrafter>();

                if (mainShaft != null)
                {
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
                        if (subcrafter != mainShaft)
                        {
                            regionalSpaces.Add(subcrafter.Build());

                            regionMinX = Mathf.Min(regionMinX, subcrafter.MinX);
                            regionMaxX = Mathf.Max(regionMaxX, subcrafter.MaxX);
                            regionMinY = Mathf.Min(regionMinY, subcrafter.MinY);
                            regionMaxY = Mathf.Max(regionMaxY, subcrafter.MaxY);
                        }
                    }

                    regionMinX -= MetalThickeness;
                    regionMaxX += MetalThickeness;
                    regionMinY -= MetalThickeness;
                    regionMaxY += MetalThickeness;

                    _minX = Mathf.Min(MinX, regionMinX);
                    _maxX = Mathf.Max(MaxX, regionMaxX);
                    _minY = Mathf.Min(MinY, regionMinY);
                    _maxY = Mathf.Max(MaxY, regionMaxY);

                    regions.Add(new Region(new IntVector2(MinX, MinY), new IntVector2(regionMaxX, MaxY), regionalSpaces));
                }
                else
                {
                    _log.Warning($"No main shaft found.  This laboratory is invalid.");
                }
            }

            return regions;
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("LaboratoryCrafter");
    }
}