using Actors;
using Data;
using Data.IO;
using UI;
using UnityEngine;
using WorldObjects;
using WorldObjects.WorldGeneration;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameState.Initialize();

        GameSaves.Refresh();
        GameSaves.LoadGame("Default");

        if (!GameSaves.HasSavedData)
        {
            _log.Info("Creating new save...");
            WorldBuilder.BuildInitialChunk();
            GameSaves.SaveDirty();
        }
        else
        {
            _log.Info("Loading saved data...");
            var initialChunkPosition = new IntVector2(0, 0);
            WorldBuilder.LoadChunk(DataReader.Read(initialChunkPosition.ToString(), DataTypes.CurrentGame));
            CheckForGenerateChunk(initialChunkPosition);
        }

        InitializePlayer();
        _log.Info("Success.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameSaves.SaveDirty();
        }
    }

    private void InitializePlayer()
    {
        var playerObj = Instantiate(Settings.Player, Vector2.zero, Quaternion.identity);
        var playerData = playerObj.GetComponent<Actors.Player.PlayerData>();
        PositionTracker.BeginTracking(playerData);
        PositionTracker.Subscribe(playerData, OnPlayerTrackingUpdate);

        playerData.OnActorDeath += OnPlayerDeath;
    }

    private void OnPlayerTrackingUpdate(PositionData oldData, PositionData newData)
    {
        CheckForGenerateChunk(newData.Chunk.transform.position);
    }

    private void CheckForGenerateChunk(Vector2 currentChunkPosition)
    {
        foreach (var dir in Directions.Compass)
        {
            var newChunkPosition = World.GetChunkPosition(new IntVector2(currentChunkPosition), dir);

            if (World.GetChunkAtPosition(newChunkPosition) != null) continue;
            else if (GameSaves.HasGameData(newChunkPosition.ToString()))
            {
                WorldBuilder.LoadChunk(DataReader.Read(newChunkPosition.ToString(), DataTypes.CurrentGame));
            }
            else WorldBuilder.BuildChunk(newChunkPosition);
        }
    }

    private void OnPlayerDeath(ActorData playerData)
    {
        PositionTracker.StopTracking(playerData);
        var scrim = Scrimmer.ScrimOver(UIManager.BaseLayer);
        scrim.Hide();
        scrim.FadeTo(1f, 3f);
    }

    private static readonly Log _log = new Log("GameManager");
}