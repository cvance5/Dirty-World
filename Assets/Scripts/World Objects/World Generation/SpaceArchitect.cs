using Data.IO;
using Data.Serialization.SerializableSpaces;
using System;
using System.Collections.Generic;
using Utilities.Debug;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public class SpaceArchitect
    {
        public static SmartEvent<Space> OnNewSpaceRegistered = new SmartEvent<Space>();
        public static SmartEvent<SpaceBuilder> OnNewSpaceBuilderDeclared = new SmartEvent<SpaceBuilder>();

        private readonly SpacePicker _sPicker;

        private readonly List<Space> _activeSpaces = new List<Space>();
        public List<Space> ActiveSpaces => new List<Space>(_activeSpaces);
        private Dictionary<string, Space> _spacesByName = new Dictionary<string, Space>();

        public SpaceArchitect(SpacePicker sPicker = null)
        {
            _sPicker = sPicker;
            ChunkArchitect.OnNewChunkBuilderAdded += AddSpace;
        }

        public void Register(Space space)
        {
            if (_activeSpaces.Contains(space))
            {
                throw new InvalidOperationException($"This world already has space `{space.Name}` registered to it.");
            }
            _activeSpaces.Add(space);
            _spacesByName.Add(space.Name, space);
            OnNewSpaceRegistered.Raise(space);
        }

        public void SetActiveSpaces(List<Chunk> activeChunks)
        {
            foreach (var chunk in activeChunks)
            {
                foreach (var spaceName in chunk.SpacesUsed)
                {
                    if (!_spacesByName.ContainsKey(spaceName))
                    {
                        LoadSpace(spaceName);
                    }
                }
            }
        }

        public Space GetSpaceByName(string spaceName)
        {
            if (!_spacesByName.TryGetValue(spaceName, out var space))
            {
                LoadSpace(spaceName);

                space = _spacesByName[spaceName];

                if (space == null) throw new KeyNotFoundException($"Can't load a space by name {spaceName}.");
            }

            return space;
        }

        public Space GetContainingSpace(IntVector2 position)
        {
            foreach (var space in _activeSpaces)
            {
                if (space.Contains(position))
                {
                    return space;
                }
            }
            return null;
        }


        private void AddSpace(ChunkBuilder sourceChunkBuilder)
        {
            if (_sPicker != null)
            {
                var spaces = CheckForSpecialCasing(sourceChunkBuilder);

                if (spaces.Count > 0)
                {
                    foreach (var space in spaces)
                    {
                        Register(space);
                    }
                }
                else
                {
                    var spaceBuilders = _sPicker.Select(sourceChunkBuilder);

                    foreach (var spaceBuilder in spaceBuilders)
                    {
                        OnNewSpaceBuilderDeclared.Raise(spaceBuilder);
                        spaceBuilder.CheckForBoundaries();
                        Register(spaceBuilder.Build());
                    }
                }
            }
        }

        private List<Space> CheckForSpecialCasing(ChunkBuilder chunk)
        {
            var specialCaseSpaces = new List<Space>();

            if (chunk.Position == IntVector2.Zero)
            {
                var space = CustomSpaceLoader.Load("InitialLaboratory");
                if (space != null) specialCaseSpaces.Add(space);
            }

            return specialCaseSpaces;
        }

        private void LoadSpace(string spaceName)
        {
            var serializedSpace = DataReader.Read(spaceName, DataTypes.CurrentGame);
            var serializableSpace = SerializableSpace.Deserialize(serializedSpace);

            var space = serializableSpace.ToObject();

            _activeSpaces.Add(space);
            _spacesByName.Add(space.Name, space);
        }

        private static readonly Log _log = new Log("SpaceArchitect");
    }
}