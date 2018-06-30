using UnityEditor;

public class CustomMenu
{
    [MenuItem("Assets/Create/Settings")]
    public static void CreateGameSettings()
    {
        ScriptableObjectUtility.CreateAsset<Settings>();
    }
}
