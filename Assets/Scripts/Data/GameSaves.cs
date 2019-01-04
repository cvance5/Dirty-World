using Data.IO;
using System.Collections.Generic;
using System.IO;

namespace Data
{
    public static class GameSaves
    {
        private static List<string> _savedGames;
        public static List<string> SavedGames => new List<string>(_savedGames);
        public static bool HasSavedData => _savedGames.Count > 0;
        public static bool SaveExists(string gameName) => _savedGames.Contains(gameName);

        public static string CurrentGame { get; private set; }

        private static List<string> _currentGameData = new List<string>();
        public static bool HasGameData(string dataName) => _currentGameData.Contains(dataName);

        public static void RefreshSavedGames()
        {
            _savedGames = DataReader.FindAllDirectories(DataTypes.SavedGames);
        }

        public static void RefreshCurrentGameData()
        {
            _currentGameData.Clear();

            _currentGameData = DataReader.FindAllFiles(DataTypes.CurrentGame);
        }

        public static void LoadGame(string gameToLoad)
        {
            CurrentGame = gameToLoad;

            if (_savedGames.Contains(CurrentGame))
            {
                RefreshCurrentGameData();
            }

            GameState.Initialize();
        }

        public static void UnloadCurrent()
        {
            CurrentGame = null;

            if (_currentGameData != null)
            {
                _currentGameData.Clear();
            }
        }

        public static void SaveDirty()
        {
            _log.Info($"Saving {CurrentGame}...");

            if (!_savedGames.Contains(CurrentGame))
            {
                Directory.CreateDirectory(Paths.ToPath(DataTypes.CurrentGame));
            }

            var filesToWrite = GameState.SerializeAll();

            if (filesToWrite.Count == 0)
            {
                _log.Info("No dirty data to save.");
                return;
            }

            foreach (var kvp in filesToWrite)
            {
                _log.Info($"Updating {kvp.Key}...");
                var writeLocation = Paths.ToPath(DataTypes.CurrentGame, kvp.Key);
                DataWriter.Write(writeLocation, kvp.Value);
            }

            _log.Info($"Save complete, updated {filesToWrite.Count} files.");

            GameState.Clear();

            RefreshSavedGames();
            RefreshCurrentGameData();
        }

        public static void DeleteSave(string gameName)
        {
            _log.Info($"Deleting {gameName}...");

            if (gameName == CurrentGame) throw new System.InvalidOperationException($"The currently loaded game cannot be deleted.");

            var deleteLocation = Paths.ToPath(DataTypes.SavedGames, gameName);
            DataWriter.DeleteRecursive(deleteLocation);

            _log.Info($"Delete complete.");
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("GameSaves");
    }
}