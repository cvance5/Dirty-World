using Data;
using UnityEngine.SceneManagement;

public static class SceneHelper
{
    public static SmartEvent OnSceneIsReady = new SmartEvent();
    public static SmartEvent OnSceneIsEnding = new SmartEvent();

    public static void ReloadScene()
    {
        SceneManager.sceneLoaded += OnActiveSceneChangedCallback;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);

        OnSceneIsEnding.Raise();
    }

    public static void LoadScene(Scenes scene)
    {
        SceneManager.sceneLoaded += OnActiveSceneChangedCallback;

        switch (scene)
        {
            case Scenes.World:
                SceneManager.LoadScene("World");
                break;
            default: throw new System.ArgumentException($"Unknown scene of type {scene}.");
        }

        OnSceneIsEnding.Raise();
    }

    // Scene is completely active 1 from after the callback, so we will have our callback fire 1 frame after theirs
    private static void OnActiveSceneChangedCallback(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnActiveSceneChangedCallback;
        Timekeeper.SetTimer(1, OnSceneIsReady.Raise);
    }

    public enum Scenes
    {
        World
    }
}