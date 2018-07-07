using UnityEngine;
using WorldObjects;
using WorldObjects.WorldGeneration;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;

    private void Start()
    {
        WorldBuilder.BuildInitialChunk();
        var playerObj = Instantiate(Settings.Player, Vector2.zero, Quaternion.identity);
        var playerData = playerObj.GetComponent<Actors.Player.PlayerData>();
        PositionTracker.BeginTracking(playerData);
        PositionTracker.Subscribe(playerData, CheckForGenerateChunk);
    }

    private void CheckForGenerateChunk(PositionData oldData, PositionData newData)
    {
        foreach (var dir in Directions.Compass)
        {
            var position = World.GetChunkPosition(newData.Chunk.GetPosition(), dir);

            if (World.GetChunkAtPosition(position) == null)
            {
                WorldBuilder.BuildChunk(position);
            }
        }
    }
}
