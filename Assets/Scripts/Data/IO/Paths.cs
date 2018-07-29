﻿using Data;
using System.IO;
using UnityEngine;

public static class Paths
{
    private const string DATAPATH = "Data";

    private const string SAVESPATH = "Saves";
    private const string OPTIONSPATH = "Options";

    public static string ToPath(DataTypes dataType)
    {
        switch (dataType)
        {
            case DataTypes.SaveGames:
                return Path.Combine(Application.persistentDataPath, DATAPATH, SAVESPATH);
            case DataTypes.CurrentGame:
                if (string.IsNullOrEmpty(GameSaves.CurrentGame)) throw new System.ArgumentException($"No save game is selected. Cannot access current Save Game Data.");
                return Path.Combine(Application.persistentDataPath, DATAPATH, SAVESPATH, GameSaves.CurrentGame);
            case DataTypes.Options:
                return Path.Combine(Application.persistentDataPath, DATAPATH, OPTIONSPATH);
            default: throw new System.ArgumentOutOfRangeException($"Unknown datatype of {dataType}.");
        }
    }

    public static string ToPath(string fileName, DataTypes dataType) => Path.Combine(ToPath(dataType), fileName);
}