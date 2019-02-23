using Data;
using Data.IO;
using Data.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.BlockGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public class ChunkArchitect : MonoBehaviour
    {
        public static SmartEvent<ChunkBuilder> OnNewChunkBuilderAdded { get; set; } = new SmartEvent<ChunkBuilder>();

        private BlockPicker _bPicker;

        private readonly List<Chunk> _activeChunks = new List<Chunk>();
        public List<Chunk> ActiveChunks => new List<Chunk>(_activeChunks);
        private Dictionary<IntVector2, Chunk> _chunksByWorldPosition = new Dictionary<IntVector2, Chunk>();

        private readonly List<ChunkBuilder> _chunkBuilders = new List<ChunkBuilder>();
        public List<ChunkBuilder> ActiveBuilders => new List<ChunkBuilder>(_chunkBuilders);
        private Dictionary<IntVector2, ChunkBuilder> _chunkBuildersByWorldPosition = new Dictionary<IntVector2, ChunkBuilder>();

        private IntVector2 _chunkBeingActivated;
        private readonly Queue<IntVector2> _chunksToActivate = new Queue<IntVector2>();
        private Coroutine _chunkActivationCoroutine;

        private Dictionary<BlockTypes, Range> _fillRanges;

        public void Initialize(BlockPicker bPicker)
        {
            _bPicker = bPicker;
            _fillRanges = new Dictionary<BlockTypes, Range>()
            {
                {BlockTypes.None, new Range(World.SURFACE_DEPTH + 1, int.MaxValue) },
                {BlockTypes.Dirt, new Range(int.MinValue, World.SURFACE_DEPTH) }
            };

            ChunkBuilder.OnChunkBuilt += OnChunkReadyToActivate;
            SerializableChunk.OnChunkLoaded += OnChunkReadyToActivate;

            SpaceArchitect.OnNewSpaceRegistered += Register;
            SpaceArchitect.OnNewSpaceBuilderDeclared += GenerateChunkBuildersForSpace;
        }

        public void ActivateChunk(IntVector2 worldPosition)
        {
            if (GetChunkAtPosition(worldPosition) != null) return;
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

        public void Register(Chunk chunk)
        {
            if (_activeChunks.Contains(chunk) || _chunksByWorldPosition.ContainsKey(chunk.Position))
            {
                throw new InvalidOperationException($"This world already has a chunk registered at {chunk.Position}.");
            }

            _activeChunks.Add(chunk);
            _chunksByWorldPosition.Add(chunk.Position, chunk);
            chunk.transform.SetParent(transform);

            foreach (var direction in Directions.Cardinals)
            {
                var neighborPos = chunk.Position + (direction * World.CHUNK_SIZE);

                if (_chunkBuildersByWorldPosition.TryGetValue(neighborPos, out var neighborBuilder))
                {
                    neighborBuilder.AddNeighbor(chunk, -direction);
                }
            }
        }

        public void Register(Spaces.Space space)
        {

            var min = GetNearestChunkPosition(new IntVector2(space.GetMaximalValue(Directions.Left),
                                                                  space.GetMaximalValue(Directions.Down)));

            var max = GetNearestChunkPosition(new IntVector2(space.GetMaximalValue(Directions.Right),
                                                                  space.GetMaximalValue(Directions.Up)));

            for (var chunkX = min.X; chunkX <= max.X; chunkX += World.CHUNK_SIZE)
            {
                for (var chunkY = min.Y; chunkY <= max.Y; chunkY += World.CHUNK_SIZE)
                {
                    var pos = new IntVector2(chunkX, chunkY);
                    if (_chunkBuildersByWorldPosition.TryGetValue(pos, out var builder))
                    {
                        builder.AddSpace(space);
                    }
                    else if (!_chunksByWorldPosition.ContainsKey(pos))
                    {
                        var newBuilder = GetBuilderAtPosition(pos);
                        newBuilder.AddSpace(space);
                    }
                }
            }
        }

        public void GenerateChunkBuildersForSpace(SpaceBuilder space)
        {
            var min = GetNearestChunkPosition(new IntVector2(space.GetMaximalValue(Directions.Left),
                                                                  space.GetMaximalValue(Directions.Down)));

            var max = GetNearestChunkPosition(new IntVector2(space.GetMaximalValue(Directions.Right),
                                                                  space.GetMaximalValue(Directions.Up)));

            for (var chunkX = min.X; chunkX <= max.X; chunkX += World.CHUNK_SIZE)
            {
                for (var chunkY = min.Y; chunkY <= max.Y; chunkY += World.CHUNK_SIZE)
                {
                    var pos = new IntVector2(chunkX, chunkY);
                    // Space has not yet validated it does not overlap existing chunks
                    if (!_chunksByWorldPosition.ContainsKey(pos))
                    {
                        GetBuilderAtPosition(pos);
                    }
                }
            }
        }

        public ChunkBuilder GetBuilderAtPosition(IntVector2 worldPosition)
        {
            if (!_chunkBuildersByWorldPosition.TryGetValue(worldPosition, out var builder))
            {
                if (GameSaves.HasGameData(worldPosition.ToString() + "B"))
                {
                    builder = LoadBuilder(worldPosition);
                }
                else if (!GameSaves.HasGameData(worldPosition.ToString()))
                {
                    builder = AddBuilder(worldPosition);
                }
            }

            return builder;
        }

        public ChunkBuilder GetContainingBuilder(IntVector2 position)
        {
            var chunkPosition = GetNearestChunkPosition(position);
            return GetBuilderAtPosition(chunkPosition);
        }

        public Chunk GetContainingChunk(IntVector2 position)
        {
            var chunkPosition = GetNearestChunkPosition(position);
            if (!_chunksByWorldPosition.TryGetValue(chunkPosition, out var chunk))
            {
                // It may exist, unloaded, but we can't give you that data right now, so...uh???
            }
            return chunk;
        }

        public Chunk GetChunkAtPosition(IntVector2 chunkPosition)
        {
            _chunksByWorldPosition.TryGetValue(chunkPosition, out var chunk);
            return chunk;
        }

        public void SetActiveChunks(List<IntVector2> activeChunkList)
        {
            foreach (var chunk in _activeChunks)
            {
                if (!activeChunkList.Contains(chunk.Position))
                {
                    chunk.SetActive(false);
                }
            }

            foreach (var activeChunkPosition in activeChunkList)
            {
                if (_chunksByWorldPosition.TryGetValue(activeChunkPosition, out var chunk))
                {
                    chunk.SetActive(true);
                }
                else ActivateChunk(activeChunkPosition);
            }
        }

        public IntVector2 GetNearestChunkPosition(IntVector2 worldPosition)
        {
            var nearestX =
                    (int)Math.Round((worldPosition.X / (double)World.CHUNK_SIZE),
                         MidpointRounding.AwayFromZero
                     ) * World.CHUNK_SIZE;

            var nearestY =
                    (int)Math.Round((worldPosition.Y / (double)World.CHUNK_SIZE),
                         MidpointRounding.AwayFromZero
                     ) * World.CHUNK_SIZE;

            return new IntVector2(nearestX, nearestY);
        }

        private ChunkBuilder LoadBuilder(IntVector2 worldPosition)
        {
            var serializedBuilder = DataReader.Read(worldPosition.ToString() + "B", DataTypes.CurrentGame);
            var serializableBuilder = SerializableChunkBuilder.Deserialize(serializedBuilder);

            var chunkBuilder = serializableBuilder.ToObject();
            Register(chunkBuilder);

            return chunkBuilder;
        }

        private ChunkBuilder AddBuilder(IntVector2 worldPosition)
        {
            var chunkBuilder = new ChunkBuilder(worldPosition, World.CHUNK_SIZE);

            if (_chunkBuildersByWorldPosition.ContainsKey(worldPosition) ||
                _chunksByWorldPosition.ContainsKey(worldPosition))
            {
                throw new InvalidOperationException($"This world already has a chunk registered at {worldPosition}.");
            }

            Register(chunkBuilder);

            OnNewChunkBuilderAdded.Raise(chunkBuilder);

            return chunkBuilder;
        }

        private void Register(ChunkBuilder chunkBuilder)
        {
            _chunkBuilders.Add(chunkBuilder);
            _chunkBuildersByWorldPosition[chunkBuilder.Position] = chunkBuilder;

            foreach (var direction in Directions.Cardinals)
            {
                var neighborPos = chunkBuilder.Position + (direction * World.CHUNK_SIZE);

                if (_chunkBuildersByWorldPosition.TryGetValue(neighborPos, out var neighborBuilder))
                {
                    chunkBuilder.AddNeighbor(neighborBuilder, direction);
                    neighborBuilder.AddNeighbor(chunkBuilder, -direction);
                }
                else if (_chunksByWorldPosition.TryGetValue(neighborPos, out var neighborChunk))
                {
                    chunkBuilder.AddNeighbor(neighborChunk, direction);
                }
            }
        }

        private void LoadChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

            var serializedChunk = DataReader.Read(worldPosition.ToString(), DataTypes.CurrentGame);
            var serializableChunk = SerializableChunk.Deserialize(serializedChunk);

            // Calls OnChunkReadyToActivate when finished
            _chunkActivationCoroutine = StartCoroutine(serializableChunk.ToObject());
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
            _chunkActivationCoroutine = StartCoroutine(cBuilder.Build());
        }

        private void OnChunkReadyToActivate(Chunk chunk)
        {
            Register(chunk);
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

            throw new ArgumentOutOfRangeException($"Unhandled depth {depth}.  No fill block found.");
        }

        public void Destroy()
        {
            _chunksToActivate.Clear();
            _chunkBeingActivated = null;
            if (_chunkActivationCoroutine != null)
            {
                StopCoroutine(_chunkActivationCoroutine);
            }

            ChunkBuilder.OnChunkBuilt -= OnChunkReadyToActivate;
            SerializableChunk.OnChunkLoaded -= OnChunkReadyToActivate;

            SpaceArchitect.OnNewSpaceRegistered -= Register;
            SpaceArchitect.OnNewSpaceBuilderDeclared -= GenerateChunkBuildersForSpace;
        }

        private static readonly Log _log = new Log("WorldBuilder");
    }
}