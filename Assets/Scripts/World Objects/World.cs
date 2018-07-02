using System.Collections.Generic;

namespace WorldObjects
{
    public class World : Singleton<World>
    {
        public static int ChunkSize => GameManager.Instance.Settings.ChunkSize;

        private List<Chunk> _activeChunks = new List<Chunk>();
        private Dictionary<IntVector2, Chunk> _chunksByWorldPosition = new Dictionary<IntVector2, Chunk>();

        public void RegisterChunk(Chunk chunk)
        {
            chunk.transform.SetParent(transform);

            _activeChunks.Add(chunk);
            _chunksByWorldPosition.Add(new IntVector2(chunk.transform.position), chunk);
        }

        public static Chunk GetContainingChunk(IntVector2 position)
        {
            foreach (var chunk in Instance._activeChunks)
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
            Instance._chunksByWorldPosition.TryGetValue(chunkPosition, out chunk);

            // No chunk has been made yet here
            return chunk;
        }

        public static IntVector2 GetChunkPosition(IntVector2 chunkPosition, IntVector2 direction) =>
            chunkPosition + new IntVector2(direction.X * ChunkSize, direction.Y * ChunkSize);
    }
}
