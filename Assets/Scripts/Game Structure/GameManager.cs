using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;

    void Start()
    {
        WorldObjects.WorldGeneration.WorldBuilder.BuildInitialChunk();
        var playerObj = Instantiate(Settings.Player, Vector2.zero, Quaternion.identity);
        var playerData = playerObj.GetComponent<Player.PlayerData>();
        PositionTracker.BeginTracking(playerData);
    }
}
