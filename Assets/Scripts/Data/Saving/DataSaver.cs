using Data.Saving;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataSaver
{
    private static readonly string DATA = Path.Combine(Application.persistentDataPath, "Data");

    private static readonly string SAVES = Path.Combine(DATA, "Saves");
    private static readonly string OPTIONS = Path.Combine(DATA, "Options");

    private static List<string> _saves = new List<string>();

    public static bool HasSavedGames => _saves.Count > 0;

    public static void InitialLoad()
    {
        if (!Directory.Exists(DATA))
        {
            try
            {
                InitializeData();
            }
            catch
            {
                Application.Quit();
            }
        }

        FindSaves();
    }

    public static void CreateSaveGame(string saveName)
    {
        if (SaveExists(saveName)) throw new ArgumentException($"Cannot overwrite existing saves currently.");

        TryCreateDirectory(Path.Combine(SAVES, saveName));

        GameSave.Create(SAVES, saveName);
    }

    public static void LoadGame(string saveName)
    {
        GameSave.ReadFrom(SAVES, saveName);
    }

    public static void SaveGame()
    {
        var savePath = Path.Combine(SAVES, GameSave.SaveName);

        if (!Directory.Exists(savePath)) throw new ArgumentException($"Save can't be found at {savePath}.");

        GameSave.PublishTo(savePath);
    }

    private static void InitializeData()
    {
        try
        {
            TryCreateDirectory(DATA);
            TryCreateDirectory(SAVES);
            TryCreateDirectory(OPTIONS);
        }
        catch (Exception e)
        {
            _log.Error($"Initial data creation has failed.  Cleaning up, and then aborting.");

            if (Directory.Exists(DATA)) Directory.Delete(DATA);
            if (Directory.Exists(SAVES)) Directory.Delete(SAVES);
            if (Directory.Exists(OPTIONS)) Directory.Delete(OPTIONS);

            throw e;
        }
    }

    private static void FindSaves()
    {
        foreach (var dir in Directory.GetDirectories(SAVES))
        {
            _saves.Add(dir);
        }
    }

    private static bool SaveExists(string saveName) => Directory.Exists(SAVES + Path.DirectorySeparatorChar + saveName);

    private static void TryCreateDirectory(string path)
    {
        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception e)
        {
            _log.Error($"Directory creation failed for {path} with exception {e.Message}.");
            throw e;
        }
    }

    private static Log _log = new Log("GameData");
}