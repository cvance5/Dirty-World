using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.WorldGeneration
{
    public class ChunkBuilder
    {
        private Vector2 _chunkWorldCenterpoint;
        private List<BlockBuilder> _blockBuilders = new List<BlockBuilder>();
        private List<SpaceBuilder> _spaceBuilders = new List<SpaceBuilder>();
        private Dictionary<Vector2, BlockBuilder> _blockByWorldPosition = new Dictionary<Vector2, BlockBuilder>();

        private Vector2 _chunkSize = GameManager.Instance.Settings.ChunkSize;

        public ChunkBuilder(Vector2 chunkWorldCenterpoint)
        {
            _chunkWorldCenterpoint = chunkWorldCenterpoint;
            Vector2 startingPoint = new Vector2(-(_chunkSize.x / 2), -(_chunkSize.y / 2));

            // Initialize all blocks in chunk to the default value
            for (int row = 0; row < _chunkSize.x; row++)
            {
                for (int column = 0; column < _chunkSize.y; column++)
                {
                    Vector2 offset = new Vector2(row, column);
                    Vector2 positionInChunk = startingPoint + offset;
                    var blockBuilder = new BlockBuilder(_chunkWorldCenterpoint + positionInChunk);
                    _blockBuilders.Add(blockBuilder);
                    _blockByWorldPosition[blockBuilder.WorldPosition] = blockBuilder;
                }
            }
        }

        public ChunkBuilder AddSpace(SpaceBuilder spaceBuilder)
        {
            _spaceBuilders.Add(spaceBuilder);
            return this;
        }

        public ChunkBuilder RemoveAtWorldPositions(params Vector2[] worldPositions)
        {
            foreach (var worldPos in worldPositions)
            {
                BlockBuilder blockBuilder;
                if (_blockByWorldPosition.TryGetValue(worldPos, out blockBuilder))
                {
                    blockBuilder.Remove();
                }
            }

            return this;
        }

        public Chunk Build()
        {
            var chunk = new GameObject($"Chunk [{_chunkWorldCenterpoint.x}, {_chunkWorldCenterpoint.y}]").AddComponent<Chunk>();

            var spaces = new List<Space>();
            foreach (var sBuilder in _spaceBuilders)
            {
                var space = sBuilder.Build();
                spaces.Add(space);
                chunk.Register(space);
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