using Data;
using Items.Weapons;
using UnityEditor;
using Utilities.Editor;

public class CustomCreateMenu
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Settings")]
    public static void CreateGameSettings()
    {
        ScriptableObjectUtility.CreateAsset<Settings>();
    }

    [MenuItem("Assets/Create/Weapon Display")]
    public static void CreateWeaponDisplay()
    {
        ScriptableObjectUtility.CreateAsset<WeaponDisplayData>();
    }
#endif
}