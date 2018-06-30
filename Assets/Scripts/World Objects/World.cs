using System.Collections.Generic;

public class World : Singleton<World>
{
    private List<Chunk> _activeChunks = new List<Chunk>();
    private List<Space> _activeSpaces = new List<Space>();

    public void RegisterChunk(Chunk chunk)
    {
        _activeChunks.Add(chunk);
        chunk.transform.SetParent(transform);
    }
}