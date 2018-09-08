using Data;
using Items.Weapons;
using UnityEditor;

public class CustomCreateMenu
{
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
}
