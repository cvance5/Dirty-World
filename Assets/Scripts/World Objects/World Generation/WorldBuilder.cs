﻿using Data;
using Data.IO;
using Data.Serialization;
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

        private IntVector2 _chunkBeingActivated;
        private readonly Queue<IntVector2> _chunksToActivate = new Queue<IntVector2>();

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
            SerializableChunk.OnChunkLoaded += OnChunkReadyToActivate;
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

            _chunkBeingActivated = new IntVector2(0, 0);
            CoroutineHandler.StartCoroutine(cBuilder.Build);

            foreach (var dir in Directions.Compass)
            {
                ActivateChunk(new IntVector2(dir.X * _chunkSize, dir.Y * _chunkSize));
            }
        }

        public void ActivateChunk(IntVector2 worldPosition)
        {
            if (_world.GetChunkAtPosition(worldPosition) != null) return;
            else if (_chunkBeingActivated == worldPosition) return;
            else if (_chunksToActivate.Contains(worldPosition)) return;
            else if (_chunkBeingActivated == null)
            {
                if (GameSaves.HasGameData(worldPosition.ToString()))
                {
                    LoadChunk(worldPosition);
                }
                else
                {
                    BuildChunk(worldPosition);
                }
            }
            else _chunksToActivate.Enqueue(worldPosition);
        }

        private void LoadChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

            var serializedChunk = DataReader.Read(worldPosition.ToString(), DataTypes.CurrentGame);
            var serializableChunk = SerializableChunk.Deserialize(serializedChunk);

            CoroutineHandler.StartCoroutine(serializableChunk.ToObject);
        }

        private void BuildChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

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
            _chunkBeingActivated = null;

            if (_chunksToActivate.Count > 0)
            {
                ActivateChunk(_chunksToActivate.Dequeue());
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
