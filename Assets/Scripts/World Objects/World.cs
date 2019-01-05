using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration;

using Space = WorldObjects.Spaces.Space;

namespace WorldObjects
{
    public class World : MonoBehaviour
    {
        public int SurfaceDepth { get; private set; }
        public int ChunkSize { get; private set; }

        private readonly List<Space> _spaces = new List<Space>();
        public List<Space> Spaces => new List<Space>(_spaces);

        private readonly List<Chunk> _loadedChunks = new List<Chunk>();
        public List<Chunk> LoadedChunks => new List<Chunk>(_loadedChunks);
        private Dictionary<IntVector2, Chunk> _chunksByWorldPosition = new Dictionary<IntVector2, Chunk>();

        private WorldBuilder _builder;

        public void Initialize(int surfaceDepth, int chunkSize)
        {
            SurfaceDepth = surfaceDepth;
            ChunkSize = chunkSize;

            WorldSizer.SetChunkSize(chunkSize);
        }

        public void Register(WorldBuilder worldBuilder)
        {
            if (_builder != null) throw new InvalidOperationException($"This world already has a registed builder.");
            else _builder = worldBuilder;
        }

        public void Register(Chunk chunk)
        {
            if (_loadedChunks.Contains(chunk) || _chunksByWorldPosition.ContainsKey(chunk.Position))
            {
                throw new InvalidOperationException($"This world already has a chunk registered at {chunk.Position}.");
            }

            _loadedChunks.Add(chunk);
            _chunksByWorldPosition.Add(chunk.Position, chunk);
            chunk.transform.SetParent(transform);
        }

        public void Register(Space space)
        {
            if (_spaces.Contains(space))
            {
                throw new InvalidOperationException($"This world already has space `{space.Name}` registered to it.");
            }
            _spaces.Add(space);
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

            throw new NotImplementedException($"Check inactive chunks too!");
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

        public Chunk GetContainingChunk(IntVector2 position)
        {
            var chunkPosition = WorldSizer.GetNearestChunkPosition(position);
            if(!_chunksByWorldPosition.TryGetValue(chunkPosition, out var chunk))
            {
                // It may exist, unloaded, but we can't give you that data right now, so...uh???
            }
            return chunk;
        }

        public Space GetContainingSpace(IntVector2 position)
        {
            foreach (var space in _spaces)
            {
                if (space.Contains(position))
                {
                    return space;
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
    }
}