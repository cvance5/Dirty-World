using Data.Serialization;
using System;
using System.Collections.Generic;
using WorldObjects;

namespace Data
{
    public static class GameState
    {
        private static readonly Dictionary<IntVector2, Chunk>
             _dirtyChunks = new Dictionary<IntVector2, Chunk>();

        public static void Initialize()
        {
            Chunk.OnChunkChanged += LogDirty;
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
            var fileToWrite = new Dictionary<string, string>();

            try
            {
                foreach (var kvp in _dirtyChunks)
                {
                    var fileName = kvp.Key.ToString();
                    var data = new SerializableChunk(kvp.Value).Serialize();
                    fileToWrite.Add(fileName, data);
                }
            }
            catch (Exception e)
            {
                _log.Error($"Failed to serialize data during a save with exception {e.Message}.  Aborting.");
                return new Dictionary<string, string>();
            }

            return fileToWrite;
        }

        public static void Clear()
        {
            _dirtyChunks.Clear();
        }

        private static readonly Log _log = new Log("GameState");
    }
}