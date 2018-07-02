using System.Collections.Generic;

namespace WorldObjects
{
    public class World : Singleton<World>
    {
        private List<Chunk> _activeChunks = new List<Chunk>();

        public void RegisterChunk(Chunk chunk)
        {
            _activeChunks.Add(chunk);
            chunk.transform.SetParent(transform);
        }
    }
}
