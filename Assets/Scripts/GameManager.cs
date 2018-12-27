using Characters;
using Data;
using Metadata;
using Narrative;
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
    public Script Script;

    public static World World { get; private set; }
    public static WorldBuilder WorldBuilder { get; private set; }
    public static ChunkActivationController ChunkActivationController { get; private set; }

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

        if (!GameSaves.HasSavedData)
        {
            StartCoroutine(HandlePrologueScreen());
        }
        else InitializeScene();
    }

    private void InitializeScene()
    {
        SceneHelper.LoadScene(SceneHelper.Scenes.Gameplay);
        SceneHelper.OnSceneIsReady += InitializeWorld;
    }

    private void InitializeWorld()
    {
        SceneHelper.OnSceneIsReady -= InitializeWorld;

        var worldGameObject = new GameObject("World");
        World = worldGameObject.AddComponent<World>();
        WorldBuilder = new WorldBuilder(World);
        World.Register(WorldBuilder);

        GameState.Initialize();

        if (GameSaves.HasSavedData)
        {
            _log.Info("Loading saved data...");
            WorldBuilder.ActivateChunk(IntVector2.Zero);
        }
        else
        {
            _log.Info("Creating new save...");
            WorldBuilder.ActivateChunk(IntVector2.Zero);
            GameSaves.SaveDirty();

            User.RegisterGame("Default");
            UserSaves.SaveUser();
        }

        ChunkActivationController = new ChunkActivationController(World);

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

        ChunkActivationController.ListenTo(_player);
        Timekeeper.StartStopwatch("PlaySession");

        _player.OnActorDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath(ActorData playerData)
    {
        var elapsedPlayTime = Timekeeper.EndStopwatch("PlaySession");
        Character.Metadata.AddTimePlayed(elapsedPlayTime);

        WorldBuilder.Destroy();

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

        UserSaves.SaveUser();
        GameSaves.SaveDirty();

        SceneHelper.ReloadScene();
        scrim.FadeTo(0, .5f).Play(() => Destroy(scrim));

        SceneHelper.OnSceneIsReady += InitializeWorld;
    }

    private IEnumerator HandlePrologueScreen()
    {
        //var scrim = Scrimmer.ScrimOver(UIManager.BaseLayer);
        //scrim.Show().Play();

        //UIScreen activeScreen = UIManager.Get<PrologueScreen>();
        //while (activeScreen != null)
        //{
        //    yield return new WaitForObjectDestroyed(activeScreen);
        //    activeScreen = UIManager.ActiveScreen;
        //}

        //scrim.FadeTo(0, .5f).Play(() => Destroy(scrim));

        yield return null;
        InitializeScene();
    }

    private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("GameManager");
}