using System.Collections.Generic;

namespace WorldObjects
{
    public class World : Singleton<World>
    {
        public static int ChunkSize => GameManager.Instance.Settings.ChunkSize;

        private static List<Chunk> _activeChunks = new List<Chunk>();
        private static Dictionary<IntVector2, Chunk> _chunksByWorldPosition = new Dictionary<IntVector2, Chunk>();

        public void RegisterChunk(Chunk chunk)
        {
            chunk.transform.SetParent(transform);

            _activeChunks.Add(chunk);
            _chunksByWorldPosition.Add(new IntVector2(chunk.transform.position), chunk);
        }

        public static List<Block> GetNeighbors(Block block)
        {
            var neighbors = new List<Block>();

            foreach (var dir in Directions.Cardinals)
            {
                Block neighbor = null;
                var neighborPos = block.GetPosition() + dir;

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
                var neighbor = GetNeighborOfChunk(chunk.GetPosition(), dir);
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
    }
}
