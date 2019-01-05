using Characters;
using Data.IO;
using Data.Serialization;
using System.Collections.Generic;
using WorldObjects;
using WorldObjects.WorldGeneration;

namespace Data
{
    public static class GameState
    {
        public static Character CurrentCharacter { get; private set; }

        private static readonly Dictionary<IntVector2, Chunk>
             _dirtyChunks = new Dictionary<IntVector2, Chunk>();

        private static readonly Dictionary<IntVector2, ChunkBuilder>
            _dirtyBuilders = new Dictionary<IntVector2, ChunkBuilder>();

        public static void Initialize()
        {
            _dirtyChunks.Clear();
            Chunk.OnChunkChanged += LogDirty;
            ChunkBuilder.OnChunkBuilderChanged += LogDirty;
            LoadCharacter();
        }

        public static void LogDirty(Chunk dirtyChunk)
        {
            if (!_dirtyChunks.ContainsKey(dirtyChunk.Position))
            {
                _dirtyChunks.Add(dirtyChunk.Position, dirtyChunk);
            }

            _dirtyBuilders.Remove(dirtyChunk.Position);
        }

        public static void LogDirty(ChunkBuilder dirtyBuilder)
        {
            if (!_dirtyBuilders.ContainsKey(dirtyBuilder.Position))
            {
                _dirtyBuilders.Add(dirtyBuilder.Position, dirtyBuilder);
            }
        }

        public static Dictionary<string, string> SerializeAll()
        {
            var filesToWrite = new Dictionary<string, string>();

            foreach (var kvp in _dirtyChunks)
            {
                var fileName = kvp.Key.ToString();
                var data = new SerializableChunk(kvp.Value).Serialize();
                filesToWrite.Add(fileName, data);
            }

            foreach (var kvp in _dirtyBuilders)
            {
                var fileName = kvp.Key.ToString();
                var data = new SerializableChunkBuilder(kvp.Value).Serialize();
                filesToWrite.Add(fileName, data);
            }

            var characterFile = new SerializableCharacter(CurrentCharacter);
            filesToWrite.Add(Paths.CHARACTERFILE, characterFile.Serialize());

            return filesToWrite;
        }

        private static void LoadCharacter()
        {
            Character character;

            if (DataReader.Exists(DataTypes.CurrentCharacter))
            {
                var characterJson = DataReader.Read(DataTypes.CurrentCharacter);
                character = SerializableCharacter.Deserialize(characterJson).ToObject();
            }
            else character = new Character();

            CurrentCharacter = character;
        }

        public static void Clear() => _dirtyChunks.Clear();

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("GameState");
    }
}