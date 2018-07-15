using System.Collections.Generic;
using UnityEngine;
using WorldObjects.WorldGeneration.BlockGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public static class WorldBuilder
    {
        private static readonly int _chunkSize = World.ChunkSize;

        private static readonly Dictionary<BlockTypes, Range> _fillRanges = new Dictionary<BlockTypes, Range>()
        {
            {BlockTypes.None, new Range(World.SurfaceDepth + 1, int.MaxValue) },
            {BlockTypes.Dirt, new Range(int.MinValue, World.SurfaceDepth) }
        };

        public static void BuildInitialChunk()
        {
            var cBuilder = new ChunkBuilder(Vector2.zero);

            var sBuilder = new CorridorBuilder(cBuilder)
                .SetStartingPoint(Vector2.zero, CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(1)
                .SetLength(100)
                .SetHazards(false);

            var sBuilder2 = new CorridorBuilder(cBuilder)
                .SetStartingPoint(Vector2.zero + (Vector2.up), CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(3)
                .SetLength(5)
                .SetHazards(false);

            cBuilder.AddSpace(sBuilder)
                    .AddSpace(sBuilder2);

            World.Instance.Register(cBuilder.Build());

            foreach (var dir in Directions.Compass)
            {
                BuildChunk(new IntVector2(dir.X * _chunkSize, dir.Y * _chunkSize));
            }
        }

        public static void BuildChunk(IntVector2 position)
        {
            var cBuilder = new ChunkBuilder(position);
            var sBuilder = SpacePicker.Pick(cBuilder);

            var blocks = BlockPicker.Pick(cBuilder);

            cBuilder.AddSpace(sBuilder)
                    .AddBlocks(blocks)
                    .SetFill(GetFill(cBuilder.Depth));

            World.Instance.Register(cBuilder.Build());
        }
        
        private static BlockTypes GetFill(int depth)
        {
            foreach (var fillRange in _fillRanges)
            {
                if (fillRange.Value.IsInRange(depth)) return fillRange.Key;
            }

            throw new System.ArgumentOutOfRangeException($"Unhandled depth {depth}.  No fill block found.");
        }
    }
}
