using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;

namespace WorldObjects
{
    public class World : MonoBehaviour
    {
        public int SurfaceDepth => GameManager.Instance.Settings.SurfaceDepth;
        public int ChunkSize => GameManager.Instance.Settings.ChunkSize;

        private readonly List<Chunk> _activeChunks = new List<Chunk>();
        public List<Chunk> ActiveChunks => new List<Chunk>(_activeChunks);

        private readonly List<ChunkBlueprint> _pendingBlueprints = new List<ChunkBlueprint>();
        public List<ChunkBlueprint> PendingBlueprints => new List<ChunkBlueprint>(_pendingBlueprints);

        private Dictionary<IntVector2, Chunk> _chunksByWorldPosition = new Dictionary<IntVector2, Chunk>();
        private Dictionary<IntVector2, ChunkBlueprint> _blueprintsByWorldPosition = new Dictionary<IntVector2, ChunkBlueprint>();

        public void Register(Chunk chunk)
        {
            _activeChunks.Add(chunk);
            _chunksByWorldPosition.Add(chunk.Position, chunk);
            chunk.transform.SetParent(transform);

            if (_blueprintsByWorldPosition.TryGetValue(chunk.Position, out var blueprint))
            {
                _pendingBlueprints.Remove(blueprint);
                _blueprintsByWorldPosition.Remove(chunk.Position);
            }
        }

        public List<Block> GetNeighbors(Block block)
        {
            var neighbors = new List<Block>();

            foreach (var dir in Directions.Cardinals)
            {
                Block neighbor = null;
                var neighborPos = block.Position + dir;

                foreach (var chunk in _activeChunks)
                {
                    if (chunk.Contains(neighborPos))
                    {
                        neighbor = chunk.GetBlockForPosition(neighborPos);
                    }
                }

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
            foreach (var chunk in _activeChunks)
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
                throw new System.ArgumentException($"A chunk already exists at {worldPosition}.");
            }
            else if (!_blueprintsByWorldPosition.TryGetValue(worldPosition, out blueprint))
            {
                blueprint = new ChunkBlueprint(worldPosition);
                _pendingBlueprints.Add(blueprint);
                _blueprintsByWorldPosition[worldPosition] = blueprint;
            }

            return blueprint;
        }
    }
}