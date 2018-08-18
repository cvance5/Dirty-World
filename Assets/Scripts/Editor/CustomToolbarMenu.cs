using Data;
using UnityEditor;

public class CustomToolbarMenu
{
    [MenuItem("Data/Clear User Data")]
    public static void ClearUserData()
    {
        GameSaves.Refresh();

        if (GameSaves.SaveExists("Default"))
        {
            GameSaves.UnloadCurrent();
            GameSaves.DeleteSave("Default");
        }
        else _log.Warning("No save data to delete.");
    }

    private static readonly Log _log = new Log("CustomToolbar");
}