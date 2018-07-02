using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;

    void Start()
    {
        WorldObjects.WorldGeneration.WorldBuilder.BuildInitialChunk();
        Instantiate(Settings.Player, Vector2.zero, Quaternion.identity);
    }
}
