using Data;
using System.IO;
using UnityEngine;

public static class Paths
{
    private const string DATAPATH = "Data";

    private const string SAVESPATH = "Saves";
    private const string OPTIONSPATH = "Options";
    private const string USERPATH = "Users";

    public const string CHARACTERFILE = "Char";
    public const string SPACESFILE = "Spaces";
    public const string SPACENAMERFILE = "SpaceNamer";

    public static string ToPath(DataTypes dataType)
    {
        switch (dataType)
        {
            case DataTypes.SavedGames:
                return Path.Combine(Application.persistentDataPath, DATAPATH, SAVESPATH);
            case DataTypes.CurrentGame:
                if (string.IsNullOrEmpty(GameSaves.CurrentGame)) throw new System.InvalidOperationException($"No save game is selected. Cannot access current Save Game Data.");
                return Path.Combine(Application.persistentDataPath, DATAPATH, SAVESPATH, GameSaves.CurrentGame);
            case DataTypes.CurrentCharacter:
                if (string.IsNullOrEmpty(GameSaves.CurrentGame)) throw new System.InvalidOperationException($"No save game is selected. Cannot access current Save Game Character.");
                return Path.Combine(Application.persistentDataPath, DATAPATH, SAVESPATH, GameSaves.CurrentGame, CHARACTERFILE);
            case DataTypes.CurrentSpaceNamer:
                if (string.IsNullOrEmpty(GameSaves.CurrentGame)) throw new System.InvalidOperationException($"No save game is selected. Cannot access current Save Game Space Namer.");
                return Path.Combine(Application.persistentDataPath, DATAPATH, SAVESPATH, GameSaves.CurrentGame, SPACENAMERFILE);
            case DataTypes.Options:
                return Path.Combine(Application.persistentDataPath, DATAPATH, OPTIONSPATH);
            case DataTypes.Users:
                return Path.Combine(Application.persistentDataPath, DATAPATH, USERPATH);
            default: throw new System.ArgumentOutOfRangeException($"Unknown datatype of {dataType}.");
        }
    }

    public static string ToPath(DataTypes dataType, params string[] subDirectory)
    {
        var path = ToPath(dataType);

        foreach (var fileName in subDirectory)
        {
            path = Path.Combine(path, fileName);
        }

        return path;
    }
}