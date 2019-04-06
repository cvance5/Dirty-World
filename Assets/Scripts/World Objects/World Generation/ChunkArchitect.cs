using Data;
using Data.IO;
using Data.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.BlockGeneration;
using WorldObjects.WorldGeneration.ChunkGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public class ChunkArchitect : MonoBehaviour
    {
        private BlockPicker _bPicker;
        private SpaceArchitect _spaceArchitect;

        private readonly List<Chunk> _chunkCache = new List<Chunk>();
        public List<Chunk> ChunkCache => new List<Chunk>(_chunkCache);

        private List<IntVector2> _activeChunksPositions;
        private Dictionary<IntVector2, Chunk> _chunksByWorldPosition = new Dictionary<IntVector2, Chunk>();

        private readonly List<ChunkBuilder> _builderCache = new List<ChunkBuilder>();
        public List<ChunkBuilder> BuilderCache => new List<ChunkBuilder>(_builderCache);
        private Dictionary<IntVector2, ChunkBuilder> _chunkBuildersByWorldPosition = new Dictionary<IntVector2, ChunkBuilder>();

        private readonly Queue<ChunkActivationCommand> _chunkActivationCommands = new Queue<ChunkActivationCommand>();
        private Coroutine _chunkConstructionCoroutine;

        private Dictionary<BlockTypes, Range> _fillRanges;

        public void Initialize(BlockPicker bPicker, SpaceArchitect spaceArchitect)
        {
            _spaceArchitect = spaceArchitect;

            _bPicker = bPicker;
            _fillRanges = new Dictionary<BlockTypes, Range>()
            {
                {BlockTypes.None, new Range(World.SURFACE_DEPTH, int.MaxValue) },
                {BlockTypes.Dirt, new Range(int.MinValue, World.SURFACE_DEPTH - 1) }
            };

            SpaceArchitect.OnNewSpaceRegistered += Register;
            SpaceArchitect.OnNewSpaceBuilderDeclared += GenerateChunkBuildersForSpace;
        }

        private void CreateChunk(IntVector2 worldPosition)
        {
            if (GetNearestChunkPosition(worldPosition) != worldPosition)
            {
                throw new InvalidOperationException($"Chunk cannot be spawned at {worldPosition}.");
            }

            if (_chunksByWorldPosition.ContainsKey(worldPosition)) return;
            else
            {
                Chunk chunk;

                if (GameSaves.HasGameData(worldPosition.ToString()))
                {
                    _log.Info($"Loading Chunk at {worldPosition}.");
                    chunk = LoadChunk(worldPosition);
                }
                else
                {
                    _log.Info($"Building Chunk at {worldPosition}.");
                    chunk = BuildChunk(worldPosition);
                }

                Register(chunk);

                if (_chunkConstructionCoroutine == null)
                {
                    _chunkConstructionCoroutine = StartCoroutine(ChunkConstructionCoroutine());
                }

                foreach (var direction in Directions.Cardinals)
                {
                    if (_chunkBuildersByWorldPosition.TryGetValue(worldPosition + direction, out var neighborBuilder))
                    {
                        _spaceArchitect.CheckForSpaces(neighborBuilder);
                    }
                }
            }
        }

        public void Register(Chunk chunk)
        {
            if (_chunkCache.Contains(chunk) || _chunksByWorldPosition.ContainsKey(chunk.Position))
            {
                throw new InvalidOperationException($"This world already has a chunk registered at {chunk.Position}.");
            }

            _chunkCache.Add(chunk);
            _chunksByWorldPosition.Add(chunk.Position, chunk);
            chunk.transform.SetParent(transform);

            if (_chunkBuildersByWorldPosition.TryGetValue(chunk.Position, out var builderUsed))
            {
                _builderCache.Remove(builderUsed);
                _chunkBuildersByWorldPosition.Remove(builderUsed.Position);
            }

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
            return GetChunkAtPosition(chunkPosition);
        }

        public Chunk GetChunkAtPosition(IntVector2 chunkPosition)
        {
            if (!_chunksByWorldPosition.TryGetValue(chunkPosition, out var chunk))
            {
                CreateChunk(chunkPosition);
                chunk = _chunksByWorldPosition[chunkPosition];
            }

            return chunk;
        }

        public void SetActiveChunks(List<IntVector2> activeChunkList)
        {
            _activeChunksPositions = activeChunkList;

            foreach (var chunk in _chunkCache)
            {
                if (!activeChunkList.Contains(chunk.Position) &&
                 chunk.State == Chunk.ChunkState.Active)
                {
                    chunk.SetState(Chunk.ChunkState.Inactive);
                }
            }

            foreach (var activeChunkPosition in activeChunkList)
            {
                if (_chunksByWorldPosition.TryGetValue(activeChunkPosition, out var chunk))
                {
                    if (chunk.State == Chunk.ChunkState.Inactive)
                    {
                        chunk.SetState(Chunk.ChunkState.Active);
                    }
                }
                else CreateChunk(activeChunkPosition);
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

            return chunkBuilder;
        }

        private void Register(ChunkBuilder chunkBuilder)
        {
            _builderCache.Add(chunkBuilder);
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

        private Chunk LoadChunk(IntVector2 worldPosition)
        {
            var serializedChunk = DataReader.Read(worldPosition.ToString(), DataTypes.CurrentGame);
            var serializableChunk = SerializableChunk.Deserialize(serializedChunk);

            var chunk = serializableChunk.ToObject();
            var chunkActivationCommand = new ChunkActivationCommand(chunk, serializableChunk.ReconstructCoroutine);
            _chunkActivationCommands.Enqueue(chunkActivationCommand);

            return chunk;
        }

        private Chunk BuildChunk(IntVector2 worldPosition)
        {
            var cBuilder = GetBuilderAtPosition(worldPosition);

            _spaceArchitect.CheckForSpaces(cBuilder);

            if (_bPicker != null)
            {
                var blocks = _bPicker.Pick(cBuilder);
                cBuilder.AddBlocks(blocks);
            }

            cBuilder.SetFill(GetFill(cBuilder.Depth));

            var chunk = cBuilder.Build();
            var chunkActivationCommand = new ChunkActivationCommand(chunk, cBuilder.BuildCoroutine);
            _chunkActivationCommands.Enqueue(chunkActivationCommand);

            return chunk;
        }

        private IEnumerator ChunkConstructionCoroutine()
        {
            while (_chunkActivationCommands.Count > 0)
            {
                var command = _chunkActivationCommands.Dequeue();

                yield return StartCoroutine(command.Invoke());

                if (_activeChunksPositions.Contains(command.Chunk.Position))
                {
                    command.Chunk.SetState(Chunk.ChunkState.Active);
                }
                else
                {
                    command.Chunk.SetState(Chunk.ChunkState.Inactive);
                }
            }

            _chunkConstructionCoroutine = null;
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
            _chunkActivationCommands.Clear();
            if (_chunkConstructionCoroutine != null)
            {
                StopCoroutine(_chunkConstructionCoroutine);
            }

            SpaceArchitect.OnNewSpaceRegistered -= Register;
            SpaceArchitect.OnNewSpaceBuilderDeclared -= GenerateChunkBuildersForSpace;
        }

        private static readonly Log _log = new Log("WorldBuilder");
    }
}