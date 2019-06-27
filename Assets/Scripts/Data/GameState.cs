﻿using Characters;
using Data.IO;
using Data.Serialization;
using MathConcepts;
using System.Collections.Generic;
using WorldObjects;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace Data
{
    public static class GameState
    {
        public static Character CurrentCharacter { get; private set; }

        private static readonly Dictionary<IntVector2, Chunk>
             _dirtyChunks = new Dictionary<IntVector2, Chunk>();

        private static readonly Dictionary<IntVector2, ChunkBuilder>
            _dirtyBuilders = new Dictionary<IntVector2, ChunkBuilder>();

        private static readonly Dictionary<string, Space>
            _dirtySpaces = new Dictionary<string, Space>();

        public static void Initialize()
        {
            _dirtyChunks.Clear();
            _dirtyBuilders.Clear();
            _dirtySpaces.Clear();

            Chunk.OnChunkChanged += LogDirty;
            ChunkBuilder.OnChunkBuilderChanged += LogDirty;
            SpaceArchitect.OnNewSpaceRegistered += LogDirty;

            LoadCharacter();
            LoadSpaceNamer();
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

        public static void LogDirty(Space dirtySpace)
        {
            if (!_dirtySpaces.ContainsKey(dirtySpace.Name))
            {
                _dirtySpaces.Add(dirtySpace.Name, dirtySpace);
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

            // TODO: How do we delete builders that have been built?
            foreach (var kvp in _dirtyBuilders)
            {
                var fileName = kvp.Key.ToString() + "B";
                var data = new SerializableChunkBuilder(kvp.Value).Serialize();
                filesToWrite.Add(fileName, data);
            }

            foreach (var kvp in _dirtySpaces)
            {
                var fileName = kvp.Key;
                var data = new SerializableSpace(kvp.Value);
                filesToWrite.Add(fileName, data.Serialize());
            }

            var characterFile = new SerializableCharacter(CurrentCharacter);
            filesToWrite.Add(Paths.CHARACTERFILE, characterFile.Serialize());

            filesToWrite.Add(Paths.SPACENAMERFILE, SpaceNamer.GetNext());

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

        private static void LoadSpaceNamer()
        {
            if (DataReader.Exists(DataTypes.CurrentSpaceNamer))
            {
                SpaceNamer.Load(int.Parse(DataReader.Read(DataTypes.CurrentSpaceNamer)));
            }
        }

        public static void Clear() => _dirtyChunks.Clear();

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("GameState");
    }
}