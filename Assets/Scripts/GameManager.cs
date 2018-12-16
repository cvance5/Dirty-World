using Characters;
using Data;
using Metadata;
using System.Collections;
using UI;
using UI.Effects;
using UI.Screens;
using UnityEngine;
using Utilities.CustomYieldInstructions;
using WorldObjects;
using WorldObjects.Actors;
using WorldObjects.Actors.Player;
using WorldObjects.WorldGeneration;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;

    public static World World { get; private set; }
    public static WorldBuilder WorldBuilder { get; private set; }
    public static User User { get; private set; }
    public static Character Character { get; private set; }

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
            User = UserSaves.CreateUser("Default");
        }

        User = UserSaves.LoadUser("Default");
    }

    private void StartGame()
    {
        GameSaves.Refresh();
        GameSaves.LoadGame("Default");

        SceneHelper.LoadScene(SceneHelper.Scenes.Gameplay);
        SceneHelper.OnSceneIsReady += InitializeWorld;
    }

    private void InitializeWorld()
    {
        SceneHelper.OnSceneIsReady -= InitializeWorld;

        if (World != null) throw new System.InvalidOperationException($"Cannot initialize world while one already exists.");

        var worldGameObject = new GameObject("World");
        World = worldGameObject.AddComponent<World>();
        WorldBuilder = new WorldBuilder(World);

        if (GameSaves.HasSavedData)
        {
            _log.Info("Loading saved data...");
            var initialChunkPosition = new IntVector2(0, 0);
            WorldBuilder.LoadChunk(initialChunkPosition);
            CheckForGenerateChunk(initialChunkPosition);
        }
        else
        {
            _log.Info("Creating new save...");
            WorldBuilder.BuildInitialChunk();
            GameSaves.SaveDirty();

            User.RegisterGame("Default");
            UserSaves.SaveUser();
        }

        Character = GameState.CurrentCharacter;

        _log.Info("Success.");
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (_player != null)
        {
            DestroyImmediate(_player.gameObject);
        }

        var playerObj = PlayerSpawner.SpawnPlayer(Character.Equipment);
        _player = playerObj.GetComponent<PlayerData>();
        _player.AssignCharacter(Character);

        PositionTracker.Subscribe(_player, OnPlayerTrackingUpdate);
        Timekeeper.StartStopwatch("PlaySession");

        _player.OnActorDeath += OnPlayerDeath;
    }
    
    private void OnPlayerTrackingUpdate(ITrackable player, PositionData oldData, PositionData newData) => CheckForGenerateChunk(newData.Chunk.transform.position);

    private void CheckForGenerateChunk(Vector2 currentChunkPosition)
    {
        foreach (var dir in Directions.Compass)
        {
            var newChunkPosition = World.GetChunkPosition(new IntVector2(currentChunkPosition), dir);
            WorldBuilder.LoadOrBuildChunk(newChunkPosition);
        }
    }

    private void OnPlayerDeath(ActorData playerData)
    {
        var elapsedPlayTime = Timekeeper.EndStopwatch("PlaySession");
        Character.Metadata.AddTimePlayed(elapsedPlayTime);

        GameSaves.SaveDirty();
        UserSaves.SaveUser();
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

    private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("GameManager");
}