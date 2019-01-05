using Data;
using Data.IO;
using Data.Serialization;
using Data.Serialization.SerializableSpaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Debug;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.BlockGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;
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

            if (GameSaves.HasGameData(Paths.SPACESFILE))
            {
                var serializedSpaces = DataReader.Read(Paths.SPACESFILE, DataTypes.CurrentGame);
                var spaces = JsonConvert.DeserializeObject<List<SerializableSpace>>(serializedSpaces, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                foreach (var space in spaces)
                {
                    _world.Register(space.ToObject());
                }
            }

            var initialBuilder = AddBuilder(IntVector2.Zero);
            AddSpace(initialBuilder);
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
                if (GameSaves.HasGameData(worldPosition.ToString() + BUILDER_IDENTIFIER))
                {
                    LoadBuilder(worldPosition);
                }
                else if (!GameSaves.HasGameData(worldPosition.ToString()))
                {
                    AddBuilder(worldPosition);
                }
            }

            return builder;
        }

        public ChunkBuilder GetContainingBuilder(IntVector2 position)
        {
            var chunkPosition = WorldSizer.GetNearestChunkPosition(position);
            return GetBuilderAtPosition(chunkPosition);
        }

        private void LoadBuilder(IntVector2 worldPosition)
        {
            var serializedBuilder = DataReader.Read(worldPosition.ToString() + BUILDER_IDENTIFIER, DataTypes.CurrentGame);
            var serializableBuilder = SerializableChunkBuilder.Deserialize(serializedBuilder);

            var deserializedChunkBuilder = serializableBuilder.ToObject();
            _chunkBuilders.Add(deserializedChunkBuilder);
            _chunkBuildersByWorldPosition[worldPosition] = deserializedChunkBuilder;
        }

        private ChunkBuilder AddBuilder(IntVector2 worldPosition)
        {
            var cBuilder = new ChunkBuilder(worldPosition, _chunkSize);
            _chunkBuilders.Add(cBuilder);
            _chunkBuildersByWorldPosition[worldPosition] = cBuilder;

            return cBuilder;
        }

        private void AddSpace(ChunkBuilder sourceChunkBuilder)
        {
            if (_sPicker != null)
            {
                var spaceBuilders = _sPicker.Select(sourceChunkBuilder);

                foreach (var spaceBuilder in spaceBuilders)
                {
                    var space = spaceBuilder.Build();

                    foreach (var affectedChunkPosition in WorldSizer.GetAffectedChunkPositions(spaceBuilder))
                    {
                        var otherBuilder = GetBuilderAtPosition(affectedChunkPosition);

                        if (otherBuilder != null)
                        {
                            otherBuilder.AddSpace(space);
                        }
                        else _log.Warning($"Tried to build a space on an existing chunk.  Stop doing that.");
                    }
                    sourceChunkBuilder.AddSpace(space);
                    _world.Register(space);
                }
            }

        }

        private void LoadChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

            var serializedChunk = DataReader.Read(worldPosition.ToString(), DataTypes.CurrentGame);
            var serializableChunk = SerializableChunk.Deserialize(serializedChunk);

            // Calls OnChunkReadyToActivate when finished
            _chunkActivationCoroutine = CoroutineHandler.StartCoroutine(serializableChunk.ToObject);
        }

        private void BuildChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

            var cBuilder = GetBuilderAtPosition(worldPosition);

            if (cBuilder == null) cBuilder = AddBuilder(worldPosition);

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

        private const string BUILDER_IDENTIFIER = "[BUILDER]";

        private static readonly Log _log = new Log("WorldBuilder");
    }
}
