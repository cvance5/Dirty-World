using Data.Saving.Serialization;
using System.Collections.Generic;
using System.IO;
using WorldObjects;

namespace Data.Saving
{
    public static class GameSave
    {
        public static string SaveName { get; private set; }

        private static readonly Dictionary<IntVector2, Chunk> _chunks = new Dictionary<IntVector2, Chunk>();
        public static void TrackChunk(Chunk chunk) => _chunks[chunk.Position] = chunk;

        public static void Create(string baseSavePath, string saveName)
        {
            SaveName = saveName;

            var path = Path.Combine(baseSavePath, saveName);
            Directory.CreateDirectory(path);
        }

        public static void PublishTo(string baseSavePath)
        {
            foreach (var kvp in _chunks)
            {
                var position = kvp.Key;
                var chunk = kvp.Value;

                var chunkFilePath = Path.Combine(baseSavePath, position.ToString() + JSON);
                var serializedChunk = new SerializableChunk(chunk).Serialize();
                File.WriteAllText(chunkFilePath, serializedChunk);
            }

            _chunks.Clear();
        }

        public static void ReadFrom(string baseSavePath, string saveName)
        {
            var fullSavePath = Path.Combine(baseSavePath, saveName);

            World.Clear();

            foreach (var chunkFilePath in Directory.EnumerateFiles(fullSavePath))
            {
                var serializedChunk = File.ReadAllText(chunkFilePath);
                var chunk = SerializableChunk.Deserialize(serializedChunk).ToObject();

                World.Register(chunk);
            }

            SaveName = saveName;
        }

        private const string JSON = ".json";
    }
}