using Data.IO;
using Data.Serialization;
using Characters;
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

        public static void Initialize()
        {
            _dirtyChunks.Clear();
            Chunk.OnChunkChanged += LogDirty;
            LoadCharacter();
        }

        public static void LogDirty(Chunk dirtyChunk)
        {
            if (!_dirtyChunks.ContainsKey(dirtyChunk.Position))
            {
                _dirtyChunks.Add(dirtyChunk.Position, dirtyChunk);
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

        public static void Clear()
        {
            _dirtyChunks.Clear();
        }

        private static readonly Log _log = new Log("GameState");
    }
}