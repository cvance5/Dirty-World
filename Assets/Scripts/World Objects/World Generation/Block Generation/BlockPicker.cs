using System.Collections.Generic;

namespace WorldObjects.WorldGeneration.BlockGeneration
{
    public static class BlockPicker
    {
        private static readonly Dictionary<BlockTypes, List<Range>> _blockRanges = new Dictionary<BlockTypes, List<Range>>()
        {
            { BlockTypes.Stone, new List<Range>()
                {
                    new Range(int.MinValue, World.SurfaceDepth)
                }
            },
            { BlockTypes.Gold, new List<Range>()
                {
                    new Range(World.SurfaceDepth - 5, World.SurfaceDepth),
                    new Range(int.MinValue, World.SurfaceDepth - 10)
                }
            },
        };

        public static Dictionary<BlockTypes, int> Pick(ChunkBuilder cBuilder)
        {
            var blocksAvailableAtDepth = GetBlocksAtDepth(cBuilder.Depth);

            var blockCounts = new Dictionary<BlockTypes, int>();

            foreach (var block in blocksAvailableAtDepth)
            {
                switch (block)
                {
                    // Blocks which have special casing that do not need 
                    // a count specified for them.
                    case BlockTypes.None:
                    case BlockTypes.Dirt:
                        break;

                    // Blocks which increase in count the deeper you go
                    case BlockTypes.Stone:
                    case BlockTypes.Gold:
                        blockCounts.Add(block, -cBuilder.Depth);
                        break;
                }
            }

            return blockCounts;
        }

        private static List<BlockTypes> GetBlocksAtDepth(int depth)
        {
            List<BlockTypes> availableBlocks = new List<BlockTypes>();

            foreach (var blockRange in _blockRanges)
            {
                foreach (var range in blockRange.Value)
                {
                    if (range.IsInRange(depth))
                    {
                        availableBlocks.Add(blockRange.Key);
                        break;
                    }
                }
            }

            return availableBlocks;
        }
    }
}