using System.Collections.Generic;
using System.IO;

namespace Data.IO
{
    public static class DataReader
    {
        public static string Read(string fileName, DataTypes dataTypes)
        {
            var readLocation = Paths.ToPath(fileName, dataTypes);
            return File.ReadAllText(readLocation);
        }

        public static List<string> FindAllFiles(DataTypes dataType)
        {
            var readLocation = Paths.ToPath(dataType);
            var allPaths = Directory.GetFiles(readLocation);

            var allFiles = new List<string>();

            foreach (var path in allPaths)
            {
                allFiles.Add(Path.GetFileNameWithoutExtension(path));
            }

            return allFiles;
        }

        public static List<string> FindAllDirectories(DataTypes dataType)
        {
            var readLocation = Paths.ToPath(dataType);
            var allPaths = Directory.GetDirectories(readLocation);

            var allFiles = new List<string>();

            foreach (var path in allPaths)
            {
                allFiles.Add(Path.GetFileNameWithoutExtension(path));
            }

            return allFiles;
        }
    }
}