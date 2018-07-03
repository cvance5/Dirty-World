using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.WorldGeneration
{
    public class ChunkBuilder
    {
        public IntVector2 BottomLeft { get; private set; }
        public IntVector2 TopRight { get; private set; }

        private IntVector2 _chunkWorldCenterpoint;        
        private List<BlockBuilder> _blockBuilders = new List<BlockBuilder>();
        private List<SpaceBuilder> _spaceBuilders = new List<SpaceBuilder>();
        private List<IntVector2> _boundedDirections = new List<IntVector2>();

        private static int _chunkSize => World.ChunkSize;

        public ChunkBuilder(IntVector2 chunkWorldCenterpoint)
        {
            _chunkWorldCenterpoint = new IntVector2(chunkWorldCenterpoint);

            IntVector2 startingPoint = new IntVector2(-(_chunkSize / 2), -(_chunkSize / 2));

            BottomLeft = new IntVector2(startingPoint);
            TopRight = new IntVector2(startingPoint.X + _chunkSize, startingPoint.Y + _chunkSize);

            // Initialize all blocks in chunk to the default value
            for (int row = 0; row < _chunkSize; row++)
            {
                for (int column = 0; column < _chunkSize; column++)
                {
                    IntVector2 offset = new IntVector2(row, column);
                    IntVector2 positionInChunk = startingPoint + offset;
                    var blockBuilder = new BlockBuilder(_chunkWorldCenterpoint + positionInChunk);
                    _blockBuilders.Add(blockBuilder);
                }
            }

            foreach (var dir in Directions.Cardinals)
            {
                if (World.GetNeighborOfChunk(chunkWorldCenterpoint, dir) != null)
                {
                    _boundedDirections.Add(dir);
                }
            }
        }

        public ChunkBuilder AddSpace(SpaceBuilder spaceBuilder)
        {
            foreach (var boundedDir in _boundedDirections)
            {
                spaceBuilder.Clamp(boundedDir, _chunkSize);
            }

            _spaceBuilders.Add(spaceBuilder);
            return this;
        }

        public Chunk Build()
        {
            var chunk = new GameObject($"Chunk [{_chunkWorldCenterpoint.X}, {_chunkWorldCenterpoint.Y}]").AddComponent<Chunk>();
            chunk.transform.position = _chunkWorldCenterpoint;
            chunk.AssignExtents
            (
                new IntVector2(_chunkWorldCenterpoint.X - (_chunkSize / 2), _chunkWorldCenterpoint.Y - (_chunkSize / 2)),
                new IntVector2(_chunkWorldCenterpoint.X + (_chunkSize / 2), _chunkWorldCenterpoint.Y + (_chunkSize / 2))
            );

            var spaces = new List<Space>();
            foreach (var sBuilder in _spaceBuilders)
            {
                var space = sBuilder.Build();
                spaces.Add(space);
                chunk.Register(space);
            }

            foreach (var dir in Directions.Cardinals)
            {
                var neighbor = (World.GetNeighborOfChunk(_chunkWorldCenterpoint, dir));

                if (neighbor != null)
                {
                    foreach (var overlappingSpace in neighbor.GetSpacesReachingEdge(-dir))
                    {
                        spaces.Add(overlappingSpace);
                        chunk.Register(overlappingSpace);
                    }
                }
            }

            foreach (var builder in _blockBuilders)
            {
                Block block;

                var containingSpace = spaces.Find(space => space.Contains(builder.WorldPosition));

                if (containingSpace != null)
                {
                    block = containingSpace.GetBlock(builder.WorldPosition);
                }
                else
                {
                    block = builder.Build();
                }

                if (block != null)
                {
                    chunk.Register(block);
                }
            }

            _blockBuilders.Clear();
            _spaceBuilders.Clear();

            return chunk;
        }
    }
}