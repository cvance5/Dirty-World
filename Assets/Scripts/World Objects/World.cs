using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration;

namespace WorldObjects
{
    public class World : MonoBehaviour
    {
        public int SurfaceDepth { get; private set; }
        public int ChunkSize { get; private set; }

        private readonly List<Chunk> _loadedChunks = new List<Chunk>();
        public List<Chunk> LoadedChunks => new List<Chunk>(_loadedChunks);

        private readonly List<ChunkBlueprint> _pendingBlueprints = new List<ChunkBlueprint>();
        public List<ChunkBlueprint> PendingBlueprints => new List<ChunkBlueprint>(_pendingBlueprints);

        private Dictionary<IntVector2, Chunk> _chunksByWorldPosition = new Dictionary<IntVector2, Chunk>();
        private Dictionary<IntVector2, ChunkBlueprint> _blueprintsByWorldPosition = new Dictionary<IntVector2, ChunkBlueprint>();

        private WorldBuilder _builder;

        public void Initialize(int surfaceDepth, int chunkSize)
        {
            SurfaceDepth = surfaceDepth;
            ChunkSize = chunkSize;

            Chunk.AssignWorld(this);
            ChunkBlueprint.AssignWorld(this);
        }

        public void Register(WorldBuilder worldBuilder)
        {
            if (_builder != null) throw new InvalidOperationException($"This world already has a registed builder.");
            else _builder = worldBuilder;
        }

        public void Register(Chunk chunk)
        {
            _loadedChunks.Add(chunk);
            _chunksByWorldPosition.Add(chunk.Position, chunk);
            chunk.transform.SetParent(transform);

            if (_blueprintsByWorldPosition.TryGetValue(chunk.Position, out var blueprint))
            {
                _pendingBlueprints.Remove(blueprint);
                _blueprintsByWorldPosition.Remove(chunk.Position);
            }
        }

        public void SetActiveChunks(List<IntVector2> activeChunkList)
        {
            foreach (var chunk in _loadedChunks)
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
                else _builder.ActivateChunk(activeChunkPosition);
            }
        }

        public Block GetBlock(IntVector2 position)
        {
            foreach (var chunk in _loadedChunks)
            {
                if (chunk.Contains(position))
                {
                    return chunk.GetBlockForPosition(position);
                }
            }

            throw new System.NotImplementedException($"Check inactive chunks too!");
        }

        public List<Block> GetNeighbors(Block block)
        {
            var neighbors = new List<Block>();

            foreach (var dir in Directions.Cardinals)
            {
                var neighborPos = block.Position + dir;
                var neighbor = GetBlock(neighborPos);

                if (neighbor != null) neighbors.Add(neighbor);
            }

            return neighbors;
        }

        public List<Chunk> GetNeighbors(Chunk chunk)
        {
            var neighbors = new List<Chunk>();

            foreach (var dir in Directions.Cardinals)
            {
                var neighbor = GetChunkNeighbor(chunk.Position, dir);
                if (neighbor != null) neighbors.Add(neighbor);
            }

            return neighbors;
        }

        public Chunk GetContainingChunk(IntVector2 position)
        {
            foreach (var chunk in _loadedChunks)
            {
                if (chunk.Contains(position))
                {
                    return chunk;
                }
            }
            return null;
        }

        public Chunk GetChunkNeighbor(IntVector2 chunkPosition, IntVector2 directionToCheck)
        {
            var neighborPosition = GetChunkPosition(chunkPosition, directionToCheck);
            return GetChunkAtPosition(neighborPosition);
        }

        public Chunk GetChunkAtPosition(IntVector2 chunkPosition)
        {
            _chunksByWorldPosition.TryGetValue(chunkPosition, out var chunk);
            return chunk;
        }

        public IntVector2 GetChunkPosition(IntVector2 chunkPosition, IntVector2 direction) =>
            chunkPosition + new IntVector2(direction.X * ChunkSize, direction.Y * ChunkSize);

        public ChunkBlueprint GetBlueprintNeighbor(IntVector2 chunkPosition, IntVector2 directionToCheck)
        {
            var neighborPosition = GetChunkPosition(chunkPosition, directionToCheck);
            return GetBlueprintForPosition(neighborPosition);
        }

        public ChunkBlueprint GetBlueprintForPosition(IntVector2 worldPosition)
        {
            ChunkBlueprint blueprint;

            if (_chunksByWorldPosition.ContainsKey(worldPosition))
            {
                throw new ArgumentException($"A chunk already exists at {worldPosition}.");
            }
            else if (!_blueprintsByWorldPosition.TryGetValue(worldPosition, out blueprint))
            {
                var bottomLeft = new IntVector2(worldPosition.X - (ChunkSize / 2), worldPosition.Y - (ChunkSize / 2));
                var topRight = new IntVector2(worldPosition.X + (ChunkSize / 2), worldPosition.Y + (ChunkSize / 2));

                blueprint = new ChunkBlueprint(worldPosition, bottomLeft, topRight);
                _pendingBlueprints.Add(blueprint);
                _blueprintsByWorldPosition[worldPosition] = blueprint;
            }

            return blueprint;
        }
    }
}