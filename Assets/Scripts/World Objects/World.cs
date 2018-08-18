using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects
{
    public static class World
    {
        public static int SurfaceDepth => GameManager.Instance.Settings.SurfaceDepth;
        public static int ChunkSize => GameManager.Instance.Settings.ChunkSize;

        private static List<Chunk> _activeChunks = new List<Chunk>();
        private static Dictionary<IntVector2, Chunk> _chunksByWorldPosition = new Dictionary<IntVector2, Chunk>();

        private static readonly Transform _worldParent;

        static World()
        {
            var world = new GameObject("World");
            _worldParent = world.transform;
        }

        public static void Register(Chunk chunk)
        {
            _activeChunks.Add(chunk);
            _chunksByWorldPosition.Add(chunk.Position, chunk);
            chunk.transform.SetParent(_worldParent);
        }

        public static List<Block> GetNeighbors(Block block)
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

        public static List<Chunk> GetNeighbors(Chunk chunk)
        {
            var neighbors = new List<Chunk>();

            foreach (var dir in Directions.Cardinals)
            {
                var neighbor = GetNeighborOfChunk(chunk.Position, dir);
                if (neighbor != null) neighbors.Add(neighbor);
            }

            return neighbors;
        }

        public static Chunk GetContainingChunk(IntVector2 position)
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

        public static Chunk GetNeighborOfChunk(IntVector2 chunkPosition, IntVector2 directionToCheck)
        {
            var neighborPosition = GetChunkPosition(chunkPosition, directionToCheck);
            return GetChunkAtPosition(neighborPosition);
        }

        public static Chunk GetChunkAtPosition(IntVector2 chunkPosition)
        {
            Chunk chunk = null;
            _chunksByWorldPosition.TryGetValue(chunkPosition, out chunk);
            return chunk;
        }

        public static IntVector2 GetChunkPosition(IntVector2 chunkPosition, IntVector2 direction) =>
            chunkPosition + new IntVector2(direction.X * ChunkSize, direction.Y * ChunkSize);

        public static void Clear()
        {
            _activeChunks.Clear();
            _chunksByWorldPosition.Clear();
        }
    }
}