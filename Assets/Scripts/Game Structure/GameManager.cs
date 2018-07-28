using UnityEngine;
using WorldObjects;
using WorldObjects.WorldGeneration;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;

    private void Awake()
    {
        Object.DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        DataSaver.InitialLoad();

        if (DataSaver.HasSavedGames)
        {
            DataSaver.LoadGame("Default");
        }
        else
        {
            DataSaver.CreateSaveGame("Default");
            WorldBuilder.BuildInitialChunk();
            DataSaver.SaveGame();
        }

        InitializePlayer();
    }

    private void InitializePlayer()
    {
        var playerObj = Instantiate(Settings.Player, Vector2.zero, Quaternion.identity);
        var playerData = playerObj.GetComponent<Actors.Player.PlayerData>();
        PositionTracker.BeginTracking(playerData);
        PositionTracker.Subscribe(playerData, CheckForGenerateChunk);
    }

    private void CheckForGenerateChunk(PositionData oldData, PositionData newData)
    {
        foreach (var dir in Directions.Compass)
        {
            var position = World.GetChunkPosition(new IntVector2(newData.Chunk.transform.position), dir);

            if (World.GetChunkAtPosition(position) == null)
            {
                WorldBuilder.BuildChunk(position);
            }
        }
    }
}