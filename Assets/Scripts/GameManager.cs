using Actors;
using Actors.Player;
using Data;
using Data.IO;
using Metadata;
using System.Collections;
using UI;
using UI.Effects;
using UI.UIScreens;
using UnityEngine;
using Utilities.CustomYieldInstructions;
using WorldObjects;
using WorldObjects.WorldGeneration;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;

    public static World World { get; private set; }
    public static User User { get; private set; }

    private PlayerData _player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UIManager.Instance.gameObject);

        FindUser();

        StartGame();
    }

    private void FindUser()
    {
        UserSaves.Refresh();

        if (!UserSaves.HasSavedData)
        {
            User = new User("default");
            UserSaves.SaveUser();
        }

        User = UserSaves.LoadUser("default");
    }

    private void StartGame()
    {
        GameState.Initialize();

        GameSaves.Refresh();
        GameSaves.LoadGame("Default");

        User.AssignCharacter(GameSaves.LoadCharacter());

        SceneHelper.LoadScene(SceneHelper.Scenes.Gameplay);
        SceneHelper.OnSceneIsReady += InitializeWorld;
    }

    private void InitializeWorld()
    {
        SceneHelper.OnSceneIsReady -= InitializeWorld;

        if (World != null) throw new System.InvalidOperationException($"Cannot initialize world while one already exists.");

        var worldGameObject = new GameObject("World");
        World = worldGameObject.AddComponent<World>();

        if (GameSaves.HasSavedData)
        {
            _log.Info("Loading saved data...");
            LoadInitialChunk();
        }
        else
        {
            _log.Info("Creating new save...");
            WorldBuilder.BuildInitialChunk();
            GameSaves.SaveDirty();
        }

        _log.Info("Success.");
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (_player != null)
        {
            DestroyImmediate(_player.gameObject);
        }

        var playerObj = Instantiate(Settings.Player, Vector2.zero, Quaternion.identity);
        _player = playerObj.GetComponent<PlayerData>();
        PositionTracker.Subscribe(_player, OnPlayerTrackingUpdate);

        _player.OnActorDeath += OnPlayerDeath;
    }

    private void LoadInitialChunk()
    {
        var initialChunkPosition = new IntVector2(0, 0);
        WorldBuilder.LoadChunk(DataReader.Read(initialChunkPosition.ToString(), DataTypes.CurrentGame));
        CheckForGenerateChunk(initialChunkPosition);
    }

    private void OnPlayerTrackingUpdate(PositionData oldData, PositionData newData)
    {
        CheckForGenerateChunk(newData.Chunk.transform.position);
    }

    private void CheckForGenerateChunk(Vector2 currentChunkPosition)
    {
        foreach (var dir in Directions.Compass)
        {
            var newChunkPosition = GameManager.World.GetChunkPosition(new IntVector2(currentChunkPosition), dir);

            if (GameManager.World.GetChunkAtPosition(newChunkPosition) != null) continue;
            else if (GameSaves.HasGameData(newChunkPosition.ToString()))
            {
                WorldBuilder.LoadChunk(DataReader.Read(newChunkPosition.ToString(), DataTypes.CurrentGame));
            }
            else WorldBuilder.BuildChunk(newChunkPosition);
        }
    }

    private void OnPlayerDeath(ActorData playerData)
    {
        GameSaves.SaveDirty();
        StartCoroutine(HandleGameOverScreen());
    }

    private IEnumerator HandleGameOverScreen()
    {
        var scrim = Scrimmer.ScrimOver(UIManager.BaseLayer);
        var activateScrimSequence = new SequenceEffect
        (
            scrim.Hide(),
            scrim.FadeTo(0.85f, 3f)
        );

        var wfcc = new WaitForCustomCallback();
        activateScrimSequence.Play(wfcc.Callback);
        yield return wfcc;

        UIScreen activeScreen = UIManager.Get<GameOverScreen>();
        while (activeScreen != null)
        {
            yield return new WaitForObjectDestroyed(activeScreen);
            activeScreen = UIManager.ActiveScreen;
        }

        wfcc = new WaitForCustomCallback();
        scrim.FadeTo(1f, .1f).Play(wfcc.Callback);
        yield return wfcc;

        SceneHelper.ReloadScene();
        scrim.FadeTo(0, .5f).Play(() => Destroy(scrim));

        World = null;
        SceneHelper.OnSceneIsReady += InitializeWorld;
    }

    private static readonly Log _log = new Log("GameManager");
}