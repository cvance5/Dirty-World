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

        private static readonly Log _log = new Log("DataWriter");
    }
}