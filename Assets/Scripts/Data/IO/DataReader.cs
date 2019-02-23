using System.Collections.Generic;
using System.IO;

namespace Data.IO
{
    public static class DataReader
    {
        public static string Read(DataTypes dataType)
        {
            var readLocation = Paths.ToPath(dataType);
            return File.ReadAllText(readLocation);
        }

        public static string Read(string fileName, DataTypes dataType)
        {
            var readLocation = Paths.ToPath(dataType, fileName);
            return File.ReadAllText(readLocation);
        }

        public static bool Exists(DataTypes dataType)
        {
            var readLocation = Paths.ToPath(dataType);
            return File.Exists(readLocation);
        }

        public static bool Exists(string fileName, DataTypes dataType)
        {
            var readLocation = Paths.ToPath(dataType, fileName);
            return File.Exists(readLocation);
        }

        public static List<string> FindAllFiles(DataTypes dataType)
        {
            var readLocation = Paths.ToPath(dataType);
            return FindAllFiles(readLocation);
        }

        public static List<string> FindAllFiles(string readLocation)
        {
            var allFiles = new List<string>();

            if (Directory.Exists(readLocation))
            {
                var allPaths = Directory.GetFiles(readLocation);

                foreach (var path in allPaths)
                {
                    allFiles.Add(Path.GetFileNameWithoutExtension(path));
                }
            }

            return allFiles;
        }

        public static List<string> FindAllDirectories(DataTypes dataType)
        {
            var readLocation = Paths.ToPath(dataType);
            return FindAllDirectories(readLocation);
        }

        public static List<string> FindAllDirectories(string readLocation)
        {
            var allFiles = new List<string>();

            if (Directory.Exists(readLocation))
            {
                var allPaths = Directory.GetDirectories(readLocation);

                foreach (var path in allPaths)
                {
                    allFiles.Add(Path.GetFileNameWithoutExtension(path));
                }
            }
            return allFiles;
        }
    }
}