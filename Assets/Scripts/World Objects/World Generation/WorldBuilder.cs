using Data;
using Data.IO;
using Data.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Debug;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.BlockGeneration;

using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration
{
    public class WorldBuilder
    {
        private int _chunkSize => _world.ChunkSize;

        private readonly World _world;
        private readonly SpacePicker _sPicker;
        private readonly BlockPicker _bPicker;

        private readonly List<ChunkBuilder> _chunkBuilders = new List<ChunkBuilder>();
        public List<ChunkBuilder> ChunkBuilders => new List<ChunkBuilder>(_chunkBuilders);
        private Dictionary<IntVector2, ChunkBuilder> _chunkBuildersByWorldPosition = new Dictionary<IntVector2, ChunkBuilder>();

        private IntVector2 _chunkBeingActivated;
        private readonly Queue<IntVector2> _chunksToActivate = new Queue<IntVector2>();
        private Coroutine _chunkActivationCoroutine;

        private readonly Dictionary<BlockTypes, Range> _fillRanges;

        public WorldBuilder(World world, SpacePicker sPicker = null, BlockPicker bPicker = null)
        {
            _world = world;
            _sPicker = sPicker;
            _bPicker = bPicker;
            _fillRanges = new Dictionary<BlockTypes, Range>()
            {
                {BlockTypes.None, new Range(_world.SurfaceDepth + 1, int.MaxValue) },
                {BlockTypes.Dirt, new Range(int.MinValue, _world.SurfaceDepth) }
            };

            ChunkBuilder.OnChunkBuilt += OnChunkReadyToActivate;
            SerializableChunk.OnChunkLoaded += OnChunkReadyToActivate;

            if (GameSaves.HasGameData("spaces"))
            {
                var serializedSpaces = DataReader.Read("spaces", DataTypes.CurrentGame);
                var spaces = JsonConvert.DeserializeObject<List<Space>>(serializedSpaces, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                foreach (var space in spaces)
                {
                    _world.Register(space);
                }
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
                    _log.Info($"Loading Chunk at {worldPosition}.");
                    LoadChunk(worldPosition);
                }
                else
                {
                    _log.Info($"Building Chunk at {worldPosition}.");
                    BuildChunk(worldPosition);
                }
            }
            else _chunksToActivate.Enqueue(worldPosition);
        }

        public ChunkBuilder GetBuilderNeighbor(IntVector2 chunkPosition, IntVector2 directionToCheck)
        {
            var neighborPosition = _world.GetChunkPosition(chunkPosition, directionToCheck);
            return GetBuilderAtPosition(neighborPosition);
        }

        public ChunkBuilder GetBuilderAtPosition(IntVector2 worldPosition)
        {
            if (!_chunkBuildersByWorldPosition.TryGetValue(worldPosition, out var builder))
            {
                if (GameSaves.HasGameData(worldPosition.ToString()))
                {
                    LoadBuilder(worldPosition);
                }
                else
                {
                    builder = new ChunkBuilder(worldPosition, _chunkSize);
                    _chunkBuilders.Add(builder);
                    _chunkBuildersByWorldPosition[worldPosition] = builder;
                }
            }

            return builder;
        }

        public ChunkBuilder GetContainingBuilder(IntVector2 position)
        {
            foreach (var builder in _chunkBuilders)
            {
                if (builder.Contains(position))
                {
                    return builder;
                }
            }
            return null;
        }

        private void LoadChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

            var serializedChunk = DataReader.Read(worldPosition.ToString(), DataTypes.CurrentGame);
            var serializableChunk = SerializableChunk.Deserialize(serializedChunk);

            // Calls OnChunkReadyToActivate when finished
            _chunkActivationCoroutine = CoroutineHandler.StartCoroutine(serializableChunk.ToObject);
        }

        private void LoadBuilder(IntVector2 worldPosition)
        {
            var serializedBuilder = DataReader.Read(worldPosition.ToString(), DataTypes.CurrentGame);
            var serializableBuilder = SerializableChunkBuilder.Deserialize(serializedBuilder);

            var deserializedChunkBuilder = serializableBuilder.ToObject();
            _chunkBuilders.Add(deserializedChunkBuilder);
            _chunkBuildersByWorldPosition[worldPosition] = deserializedChunkBuilder;
        }

        private void BuildChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

            var cBuilder = GetBuilderAtPosition(worldPosition);

            if (_bPicker != null)
            {
                var blocks = _bPicker.Pick(cBuilder);
                cBuilder.AddBlocks(blocks);
            }

            cBuilder.SetFill(GetFill(cBuilder.Depth));

            // Calls OnChunkReadyToActivate when finished
            _chunkActivationCoroutine = CoroutineHandler.StartCoroutine(cBuilder.Build);
        }

        private void OnChunkReadyToActivate(Chunk chunk)
        {
            _world.Register(chunk);
            _chunkBeingActivated = null;
            _chunkActivationCoroutine = null;

            if (_chunkBuildersByWorldPosition.TryGetValue(chunk.Position, out var builderUsed))
            {
                _chunkBuilders.Remove(builderUsed);
                _chunkBuildersByWorldPosition.Remove(builderUsed.Position);
            }

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

        public void Destroy()
        {
            _chunksToActivate.Clear();
            _chunkBeingActivated = null;
            if (_chunkActivationCoroutine != null)
            {
                CoroutineHandler.AbortCoroutine(_chunkActivationCoroutine);
            }

            ChunkBuilder.OnChunkBuilt -= OnChunkReadyToActivate;
            SerializableChunk.OnChunkLoaded -= OnChunkReadyToActivate;
        }

        private static readonly Log _log = new Log("WorldBuilder");
    }
}
