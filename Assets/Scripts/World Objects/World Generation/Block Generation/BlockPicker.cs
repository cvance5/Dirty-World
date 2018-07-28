using System.Collections.Generic;
using WorldObjects.Blocks;

namespace WorldObjects.WorldGeneration.BlockGeneration
{
    public static class BlockPicker
    {
        private static readonly Dictionary<BlockTypes, Range> _blockRanges = new Dictionary<BlockTypes, Range>()
        {
            { BlockTypes.Stone, new Range(int.MinValue, World.SurfaceDepth) },
            { BlockTypes.Copper, new Range(World.SurfaceDepth - 15, World.SurfaceDepth) },
            { BlockTypes.Silver,  new Range(World.SurfaceDepth - 30, World.SurfaceDepth - 10) },
            { BlockTypes.Gold,  new Range(World.SurfaceDepth - 45, World.SurfaceDepth - 25) },
            { BlockTypes.Platinum, new Range(int.MinValue, World.SurfaceDepth - 40) }
        };

        private static readonly Dictionary<BlockTypes, float> _blockScarcities = new Dictionary<BlockTypes, float>()
        {
            { BlockTypes.Silver, .75f },
            { BlockTypes.Gold, .6f },
            { BlockTypes.Platinum, .45f }
        };

        public static Dictionary<BlockTypes, int> Pick(ChunkBuilder cBuilder)
        {
            var blocksAvailableAtDepth = GetBlocksAtDepth(cBuilder.Depth);
            var blockCounts = GetBlockCountsByDepth(blocksAvailableAtDepth, cBuilder.Depth);
            var modifiedBlockCounts = ApplyBlockScarcity(blockCounts);

            return modifiedBlockCounts;
        }

        private static List<BlockTypes> GetBlocksAtDepth(int depth)
        {
            List<BlockTypes> availableBlocks = new List<BlockTypes>();

            foreach (var blockRange in _blockRanges)
            {
                var range = blockRange.Value;

                if (range.IsInRange(depth))
                {
                    availableBlocks.Add(blockRange.Key);
                }
            }

            return availableBlocks;
        }

        private static Dictionary<BlockTypes, int> GetBlockCountsByDepth(List<BlockTypes> blockTypes, int depth)
        {
            var results = new Dictionary<BlockTypes, int>();

            foreach (var block in blockTypes)
            {
                switch (block)
                {
                    // Blocks which have special casing that do not need 
                    // a count specified for them
                    case BlockTypes.None:
                    case BlockTypes.Dirt:
                        break;

                    // Blocks which increase in count the deeper you go
                    case BlockTypes.Stone:
                    case BlockTypes.Platinum:
                        var depthRange = _blockRanges[block];
                        results.Add(block, depthRange.Max - depth);
                        break;

                    // Blocks which are most dense near the center of 
                    // their respective ranges
                    case BlockTypes.Copper:
                    case BlockTypes.Silver:
                    case BlockTypes.Gold:
                        var targetRange = _blockRanges[block];
                        var maxSize = targetRange.Size;
                        var distance = targetRange.DistanceFromCenter(depth);
                        results.Add(block, maxSize - distance);
                        break;
                }
            }

            return results;
        }

        private static Dictionary<BlockTypes, int> ApplyBlockScarcity(Dictionary<BlockTypes, int> blockCounts)
        {
            var modifiedBlockCounts = new Dictionary<BlockTypes, int>(blockCounts);

            foreach (var blockType in blockCounts.Keys)
            {
                float scarcity;
                if (!_blockScarcities.TryGetValue(blockType, out scarcity)) scarcity = 1;
                modifiedBlockCounts[blockType] = (int)(blockCounts[blockType] * scarcity);
            }

            return modifiedBlockCounts;
        }
    }
}