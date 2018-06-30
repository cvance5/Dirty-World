using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;

    void Start()
    {
        var builder = new WorldGeneration.ChunkBuilder(Vector2.zero)
            .RemoveAtWorldPositions(Vector2.zero, Vector2.left, Vector2.right);

        var initialChunk = builder.Build();
        World.Instance.RegisterChunk(initialChunk);

        Instantiate(Settings.Player, Vector2.zero, Quaternion.identity);
    }
}
