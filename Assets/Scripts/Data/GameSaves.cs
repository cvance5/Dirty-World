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

        public static string CurrentGame { get; private set; }

        private static List<string> _currentGameData = new List<string>();
        public static bool HasGameData(string dataName) => _currentGameData.Contains(dataName);

        public static void Refresh()
        {
            _savedGames = DataReader.FindAllDirectories(DataTypes.SaveGames);
        }

        public static void SetCurrent(string currentGame)
        {
            CurrentGame = currentGame;

            if (_savedGames.Contains(CurrentGame))
            {
                _currentGameData.Clear();

                _currentGameData = DataReader.FindAllFiles(DataTypes.CurrentGame);
            }
        }

        public static void SaveDirty()
        {
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
                var writeLocation = Paths.ToPath(kvp.Key, DataTypes.CurrentGame);
                DataWriter.Write(writeLocation, kvp.Value);
            }

            _log.Info($"Save complete, updated {filesToWrite.Count} files.");

            GameState.Clear();

            Refresh();
        }

        private static readonly Log _log = new Log("GameSaves");
    }
}