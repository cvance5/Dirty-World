using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.BlockGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public static class WorldBuilder
    {
        private static readonly int _chunkSize = GameManager.Instance.Settings.ChunkSize;

        private static readonly Dictionary<BlockTypes, Range> _fillRanges = new Dictionary<BlockTypes, Range>()
        {
            {BlockTypes.None, new Range(GameManager.World.SurfaceDepth + 1, int.MaxValue) },
            {BlockTypes.Dirt, new Range(int.MinValue, GameManager.World.SurfaceDepth) }
        };

        public static void BuildInitialChunk()
        {
            var cBuilder = new ChunkBuilder(Vector2.zero, _chunkSize);

            var sBuilder = new CorridorBuilder(cBuilder)
                .SetStartingPoint(Vector2.zero, CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(1)
                .SetLength(100)
                .SetAllowEnemies(false)
                .SetHazards(false)
                .AddModifier(ModifierTypes.Cavernous);

            var sBuilder2 = new CorridorBuilder(cBuilder)
                .SetStartingPoint(Vector2.zero + (Vector2.up), CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(3)
                .SetLength(5)
                .SetAllowEnemies(false)
                .SetHazards(false)
                .AddModifier(ModifierTypes.Cavernous);

            cBuilder.AddSpace(sBuilder)
                    .AddSpace(sBuilder2);

            GameManager.World.Register(cBuilder.Build());

            foreach (var dir in Directions.Compass)
            {
                BuildChunk(new IntVector2(dir.X * _chunkSize, dir.Y * _chunkSize));
            }
        }

        public static void LoadChunk(string serializedChunk)
        {
            var serializableChunk = Data.Serialization.SerializableChunk.Deserialize(serializedChunk);
            var chunk = serializableChunk.ToObject();
            GameManager.World.Register(chunk);
        }

        public static void BuildChunk(IntVector2 worldPosition)
        {
            var cBuilder = new ChunkBuilder(worldPosition, _chunkSize, GameManager.World.GetBlueprintForPosition(worldPosition));
            var sBuilders = SpacePicker.Pick(cBuilder);

            var blocks = BlockPicker.Pick(cBuilder);

            foreach (var sBuilder in sBuilders)
            {
                cBuilder.AddSpace(sBuilder);
            }

            cBuilder.AddBlocks(blocks)
                    .SetFill(GetFill(cBuilder.Depth));

            GameManager.World.Register(cBuilder.Build());
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
