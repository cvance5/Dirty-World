﻿using Data;
using UnityEditor;

public class CustomToolbarMenu
{
#if UNITY_EDITOR
    [MenuItem("Data/Clear User Data")]
    public static void ClearUserData()
    {
        GameSaves.RefreshSavedGames();

        if (GameSaves.SaveExists("Default"))
        {
            GameSaves.UnloadCurrent();
            GameSaves.DeleteSave("Default");

            var user = UserSaves.LoadUser("Default");
            user.UnregisterGame("Default");

            UserSaves.SaveUser();            
        }
        else _log.Warning("No save data to delete.");
    }

    private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("CustomToolbar");
#endif
}