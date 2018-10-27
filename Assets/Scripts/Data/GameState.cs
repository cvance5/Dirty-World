using Characters;
using Data.IO;
using Data.Serialization;
using System;
using System.Collections.Generic;
using WorldObjects;

namespace Data
{
    public static class GameState
    {
        public static Character CurrentCharacter { get; private set; }

        private static readonly Dictionary<IntVector2, Chunk>
             _dirtyChunks = new Dictionary<IntVector2, Chunk>();

        private static readonly Dictionary<IntVector2, ChunkBlueprint>
            _dirtyBlueprints = new Dictionary<IntVector2, ChunkBlueprint>();

        public static void Initialize()
        {
            _dirtyChunks.Clear();
            Chunk.OnChunkChanged += LogDirty;
            ChunkBlueprint.OnBlueprintChanged += LogDirty;
            LoadCharacter();
        }

        public static void LogDirty(Chunk dirtyChunk)
        {
            if (!_dirtyChunks.ContainsKey(dirtyChunk.Position))
            {
                _dirtyChunks.Add(dirtyChunk.Position, dirtyChunk);
            }
        }

        public static void LogDirty(ChunkBlueprint dirtyBlueprint)
        {
            if (_dirtyBlueprints.ContainsKey(dirtyBlueprint.Position))
            {
                _dirtyBlueprints.Add(dirtyBlueprint.Position, dirtyBlueprint);
            }
        }

        public static Dictionary<string, string> SerializeAll()
        {
            var filesToWrite = new Dictionary<string, string>();

            try
            {
                foreach (var kvp in _dirtyChunks)
                {
                    var fileName = kvp.Key.ToString();
                    var data = new SerializableChunk(kvp.Value).Serialize();
                    filesToWrite.Add(fileName, data);
                }

                foreach (var kvp in _dirtyBlueprints)
                {
                    var fileName = kvp.Key.ToString();
                    var data = new SerializableChunkBlueprint(kvp.Value).Serialize();
                    filesToWrite.Add(fileName, data);
                }

                var characterFile = new SerializableCharacter(CurrentCharacter);
                filesToWrite.Add(Paths.CHARACTERFILE, characterFile.Serialize());
            }
            catch (Exception e)
            {
                _log.Error($"Failed to serialize data during a save with exception {e.Message}.  Aborting.");
                return new Dictionary<string, string>();
            }

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