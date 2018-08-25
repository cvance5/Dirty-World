using UnityEngine.SceneManagement;

public static class SceneHelper
{
    public static SmartEvent OnSceneIsReady = new SmartEvent();
    public static SmartEvent OnSceneIsEnding = new SmartEvent();

    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnActiveSceneChangedCallback;

        OnSceneIsEnding.Raise();
    }

    public static void LoadScene(Scenes scene)
    {
        switch (scene)
        {
            case Scenes.Gameplay:
                SceneManager.LoadScene("Gameplay");
                break;
            default: throw new System.ArgumentException($"Unknown scene of type {scene}.");
        }

        SceneManager.sceneLoaded += OnActiveSceneChangedCallback;

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
        Gameplay
    }
}