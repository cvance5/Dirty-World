using Data;
using UnityEditor;

public class CustomCreateMenu
{
    [MenuItem("Assets/Create/Settings")]
    public static void CreateGameSettings()
    {
        ScriptableObjectUtility.CreateAsset<Settings>();
    }
}
