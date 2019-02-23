using Characters;
using Data;
using GizmoDrawers;
using Metadata;
using Narrative;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Effects;
using UI.Screens;
using UnityEngine;
using Utilities.CustomYieldInstructions;
using WorldObjects;
using WorldObjects.Actors;
using WorldObjects.Actors.Player;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;
using WorldObjects.WorldGeneration.BlockGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

public class GameManager : Singleton<GameManager>
{
    public Settings Settings;
    public Script Script;

    public static World World { get; private set; }

    public static User User { get; private set; }
    public static Character Character { get; private set; }

    private PlayerHealth _player;

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
        GameSaves.RefreshSavedGames();
        GameSaves.LoadGame("Default");

        if (!GameSaves.HasSavedData)
        {
            StartCoroutine(HandlePrologueScreen());
        }

        InitializeScene();
    }

    private void InitializeScene()
    {
        SceneHelper.LoadScene(SceneHelper.Scenes.World);
        SceneHelper.OnSceneIsReady += InitializeWorld;
    }

    private void InitializeWorld()
    {
        SceneHelper.OnSceneIsReady -= InitializeWorld;

        var worldGameObject = new GameObject("World");
        World = worldGameObject.AddComponent<World>();

        var bPicker = new BlockPicker();
        var chunkArchitect = worldGameObject.AddComponent<ChunkArchitect>();
        chunkArchitect.Initialize(bPicker);

        var sPicker = new SpacePicker(new List<Type>()
        {
            typeof(ShaftBuilder),
            typeof(CorridorBuilder),
            typeof(MonsterDenBuilder),
            typeof(RoomBuilder)
        });
        var spaceArchitect = new SpaceArchitect(sPicker);

        World.Initialize(chunkArchitect, spaceArchitect);

        ChunkGizmoDrawer.SetWorldToDraw(World);
        PositionTracker.SetWorldToTrack(World);

        GameState.Initialize();

        if (GameSaves.HasSavedData)
        {
            _log.Info("Loading saved data...");
            chunkArchitect.ActivateChunk(IntVector2.Zero);
        }
        else
        {
            _log.Info("Creating new save...");
            chunkArchitect.ActivateChunk(IntVector2.Zero);
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
        _player = playerObj.GetComponent<PlayerHealth>();
        _player.AssignCharacter(Character);

        PositionTracker.Subscribe(_player, OnPlayerPositionUpdate);
        World.ListenTo(_player);
        Timekeeper.StartStopwatch("PlaySession");

        _player.OnActorDeath += OnPlayerDeath;
    }

    private void OnPlayerPositionUpdate(ITrackable player, PositionData oldData, PositionData newData)
    {
        if (newData.Chunk == null)
        {
            Timekeeper.TogglePause(true);
        }
        else if (Timekeeper.IsPaused)
        {
            Timekeeper.TogglePause(false);
        }
    }

    private void OnPlayerDeath(ActorHealth playerData)
    {
        var elapsedPlayTime = Timekeeper.EndStopwatch("PlaySession");
        Character.Metadata.AddTimePlayed(elapsedPlayTime);

        PositionTracker.Unsubscribe(_player, OnPlayerPositionUpdate);

        World.Destroy();

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
        var scrim = Scrimmer.ScrimOver(UIManager.BaseLayer);
        scrim.Show().Play();

        UIScreen activeScreen = UIManager.Get<PrologueScreen>();
        while (activeScreen != null)
        {
            yield return new WaitForObjectDestroyed(activeScreen);
            activeScreen = UIManager.ActiveScreen;
        }

        scrim.FadeTo(0, .5f).Play(() => Destroy(scrim));

        yield return null;
    }

    private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("GameManager");
}