using Data;
using UnityEditor;

public class CustomToolbarMenu
{
#if UNITY_EDITOR
    [MenuItem("Data/Clear User Data")]
    public static void ClearUserData()
    {
        if (EditorApplication.isPlaying)
        {
            _log.Error($"Cannot delete during play mode.");
        }
        else
        {
            GameSaves.RefreshSavedGames();

            if (GameSaves.SaveExists("Default"))
            {
                GameSaves.UnloadCurrent();
                GameSaves.DeleteSave("Default");
            }
            else _log.Warning("No save data to delete.");

            var user = UserSaves.LoadUser("Default");
            user.UnregisterGame("Default");

            UserSaves.SaveUser();
        }
    }

    private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("CustomToolbar");
#endif
}