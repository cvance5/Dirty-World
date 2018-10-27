using System.IO;

namespace Data.IO
{
    public static class DataWriter
    {
        public static void Write(string filePath, string data)
        {
            try
            {
                File.WriteAllText(filePath, data);
            }
            catch (System.Exception e)
            {
                _log.Error($"Failed to write to {filePath} with exception {e.Message}");
            }
        }

        public static void Delete(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (System.Exception e)
            {
                _log.Error($"Failed to delete {filePath} with exception {e.Message}");
            }
        }

        public static void DeleteRecursive(string directoryPath)
        {
            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch (System.Exception e)
            {
                _log.Error($"Failed to delete {directoryPath} with exception {e.Message}");
            }
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("DataWriter");
    }
}