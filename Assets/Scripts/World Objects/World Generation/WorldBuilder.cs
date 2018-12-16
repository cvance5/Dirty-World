using Data;
using Data.IO;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.BlockGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public class WorldBuilder
    {
        private static readonly int _chunkSize = GameManager.Instance.Settings.ChunkSize;
        private readonly World _world;

        private IntVector2 _chunkBeingBuilt;
        private readonly Queue<IntVector2> _chunksToBuild = new Queue<IntVector2>();

        private readonly Dictionary<BlockTypes, Range> _fillRanges;

        public WorldBuilder(World world)
        {
            _world = world;
            _fillRanges = new Dictionary<BlockTypes, Range>()
            {
                {BlockTypes.None, new Range(_world.SurfaceDepth + 1, int.MaxValue) },
                {BlockTypes.Dirt, new Range(int.MinValue, _world.SurfaceDepth) }
            };

            ChunkBuilder.OnChunkBuilt += OnChunkReadyToActivate;
        }

        public void BuildInitialChunk()
        {
            var cBuilder = new ChunkBuilder(Vector2.zero, _chunkSize);

            var sBuilder = new CorridorBuilder(cBuilder)
                .SetStartingPoint(Vector2.zero, CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(1)
                .SetLength(100)
                .SetAllowEnemies(false)
                .AddModifier(ModifierTypes.Cavernous);

            var sBuilder2 = new CorridorBuilder(cBuilder)
                .SetStartingPoint(Vector2.zero + (Vector2.up), CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(3)
                .SetLength(5)
                .SetAllowEnemies(false)
                .AddModifier(ModifierTypes.Cavernous);

            cBuilder.AddSpace(sBuilder)
                    .AddSpace(sBuilder2);

            _chunkBeingBuilt = new IntVector2(0, 0);
            CoroutineHandler.StartCoroutine(cBuilder.Build);

            foreach (var dir in Directions.Compass)
            {
                ActivateChunk(new IntVector2(dir.X * _chunkSize, dir.Y * _chunkSize));
            }
        }

        public void ActivateChunk(IntVector2 worldPosition)
        {
            if (_world.GetChunkAtPosition(worldPosition) != null) return;
            else if (_chunkBeingBuilt == worldPosition) return;
            else if (_chunksToBuild.Contains(worldPosition)) return;
            else if (GameSaves.HasGameData(worldPosition.ToString()))
            {
                LoadChunk(worldPosition);
            }
            else
            {
                if (_chunkBeingBuilt == null)
                {
                    BuildChunk(worldPosition);
                }
                else _chunksToBuild.Enqueue(worldPosition);
            }
        }

        private void LoadChunk(IntVector2 worldPosition)
        {
            var serializedChunk = DataReader.Read(worldPosition.ToString(), DataTypes.CurrentGame);
            var serializableChunk = Data.Serialization.SerializableChunk.Deserialize(serializedChunk);

            var chunk = serializableChunk.ToObject();

            _world.Register(chunk);
        }

        private void BuildChunk(IntVector2 worldPosition)
        {
            _chunkBeingBuilt = worldPosition;

            var cBuilder = new ChunkBuilder(worldPosition, _chunkSize, _world.GetBlueprintForPosition(worldPosition));
            var sBuilders = SpacePicker.Pick(cBuilder);

            var blocks = BlockPicker.Pick(cBuilder);

            foreach (var sBuilder in sBuilders)
            {
                cBuilder.AddSpace(sBuilder);
            }

            cBuilder.AddBlocks(blocks)
                    .SetFill(GetFill(cBuilder.Depth));

            // Calls OnChunkReadyToActivate when finished
            CoroutineHandler.StartCoroutine(cBuilder.Build);
        }

        private void OnChunkReadyToActivate(Chunk chunk)
        {
            _world.Register(chunk);
            _chunkBeingBuilt = null;

            if(_chunksToBuild.Count > 0)
            {
                BuildChunk(_chunksToBuild.Dequeue());
            }
        }

        private BlockTypes GetFill(int depth)
        {
            foreach (var fillRange in _fillRanges)
            {
                if (fillRange.Value.IsInRange(depth)) return fillRange.Key;
            }

            throw new System.ArgumentOutOfRangeException($"Unhandled depth {depth}.  No fill block found.");
        }
    }
}
