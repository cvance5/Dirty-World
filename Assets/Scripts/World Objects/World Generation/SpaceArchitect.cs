using Data.IO;
using Data.Serialization.SerializableSpaces;
using MathConcepts;
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

        public SpaceArchitect(SpacePicker sPicker = null) => _sPicker = sPicker;

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
                if (space.Extents.Contains(position))
                {
                    return space;
                }
            }
            return null;
        }

        public void CheckForSpaces(ChunkBuilder sourceChunkBuilder)
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

                while (_sPicker.NeedsMoreSpaces(sourceChunkBuilder))
                {
                    var spaceBuilder = _sPicker.Select(sourceChunkBuilder);

                    OnNewSpaceBuilderDeclared.Raise(spaceBuilder);

                    FindSpaceBoundaries(spaceBuilder, sourceChunkBuilder);

                    if (spaceBuilder.IsValid)
                    {
                        var space = spaceBuilder.Build();

                        Register(space);
                        sourceChunkBuilder.AddSpace(space);
                    }
                }
            }
        }

        private void FindSpaceBoundaries(SpaceBuilder spaceBuilder, ChunkBuilder sourceChunkBuilder)
        {
            var chunksToCheck = new Queue<ChunkBuilder>();
            var checkedChunks = new List<ChunkBuilder>();

            chunksToCheck.Enqueue(sourceChunkBuilder);

            while (chunksToCheck.Count > 0 && spaceBuilder.IsValid)
            {
                var nextChunkToCheck = chunksToCheck.Dequeue();

                if (checkedChunks.Contains(nextChunkToCheck))
                {
                    continue;
                }
                else checkedChunks.Add(nextChunkToCheck);

                foreach (var direction in Directions.Cardinals)
                {
                    var maximalValue = nextChunkToCheck.GetMaximalValue(direction);

                    var amount = spaceBuilder.PassesBy(direction, maximalValue);

                    if (amount > 0)
                    {
                        if (nextChunkToCheck.TryGetNeighborBuilder(direction, out var neighbor))
                        {
                            // Add every neighbor affected to be checked.
                            chunksToCheck.Enqueue(neighbor);
                        }
                        else
                        {
                            // If we discovered a boundary, this greatly changes the space.  Check everything again.
                            spaceBuilder.AddBoundary(direction, maximalValue);
                            chunksToCheck.Clear();
                            chunksToCheck.Enqueue(sourceChunkBuilder);
                            checkedChunks.Clear();
                            break;
                        }
                    }
                }
            }
        }

        private List<Space> CheckForSpecialCasing(ChunkBuilder chunk)
        {
            var specialCaseSpaces = new List<Space>();

            //if (chunk.Position == IntVector2.Zero)
            //{
            //    var space = CustomSpaceLoader.Load("InitialLaboratory");
            //    if (space != null) specialCaseSpaces.Add(space);
            //}

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