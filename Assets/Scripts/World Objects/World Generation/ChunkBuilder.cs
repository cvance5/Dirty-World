using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration
{
    public class ChunkBuilder
    {
        public IntVector2 BottomLeftCorner { get; private set; }
        public IntVector2 TopRightCorner { get; private set; }

        public int Depth { get; private set; }
        public int Remoteness { get; private set; }

        private IntVector2 _chunkWorldCenterpoint;
        private List<BlockBuilder> _blockBuilders = new List<BlockBuilder>();
        private List<SpaceBuilder> _spaceBuilders = new List<SpaceBuilder>();
        private List<IntVector2> _boundedDirections = new List<IntVector2>();

        private readonly Dictionary<IntVector2, BlockBuilder> _blockMap
                   = new Dictionary<IntVector2, BlockBuilder>();

        private readonly ChunkBlueprint _blueprint;

        private readonly int _chunkSize;
        private int _halfChunkSize => _chunkSize / 2;

        public BlockTypes FillBlock { get; private set; } = BlockTypes.Dirt;

        public ChunkBuilder(IntVector2 chunkWorldCenterpoint, int chunkSize, ChunkBlueprint blueprint = null)
        {
            _chunkSize = chunkSize;
            _chunkWorldCenterpoint = new IntVector2(chunkWorldCenterpoint);

            var startingPoint = new IntVector2(-_halfChunkSize, -_halfChunkSize);

            BottomLeftCorner = new IntVector2(chunkWorldCenterpoint.X - _halfChunkSize, chunkWorldCenterpoint.Y - _halfChunkSize);
            TopRightCorner = new IntVector2(chunkWorldCenterpoint.X + _halfChunkSize - 1, chunkWorldCenterpoint.Y + _halfChunkSize - 1);

            Depth = _chunkWorldCenterpoint.Y / _chunkSize;
            Remoteness = Mathf.Abs(_chunkWorldCenterpoint.X / _chunkSize) + Mathf.Abs(_chunkWorldCenterpoint.Y / _chunkSize);

            // Initialize all blocks in chunk to the default value
            for (var row = 0; row < _chunkSize; row++)
            {
                for (var column = 0; column < _chunkSize; column++)
                {
                    var offset = new IntVector2(row, column);
                    var positionInChunk = startingPoint + offset;
                    var blockBuilder = new BlockBuilder(_chunkWorldCenterpoint + positionInChunk);
                    _blockBuilders.Add(blockBuilder);
                    _blockMap.Add(blockBuilder.Position, blockBuilder);
                }
            }

            foreach (var dir in Directions.Cardinals)
            {
                if (GameManager.World?.GetChunkNeighbor(chunkWorldCenterpoint, dir) != null)
                {
                    _boundedDirections.Add(dir);
                }

                if (Depth == GameManager.World?.SurfaceDepth)
                {
                    _boundedDirections.Add(Directions.Up);
                }
            }

            _blueprint = blueprint;
        }

        public bool Contains(IntVector2 position) =>
                position.X >= BottomLeftCorner.X &&
                position.Y >= BottomLeftCorner.Y &&
                position.X <= TopRightCorner.X &&
                position.Y <= TopRightCorner.Y;

        public ChunkBuilder AddSpace(SpaceBuilder spaceBuilder)
        {
            foreach (var boundedDir in _boundedDirections)
            {
                // We subtract one so that the shared boundary block
                // is not included in the Space.  Otherwise, the space may
                // not be able to create the block it needs at the boundary, if the other
                // chunk has mandated a block.

                if (boundedDir == Directions.Up)
                {
                    spaceBuilder.AddBoundary(boundedDir, _chunkWorldCenterpoint.Y + _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Right)
                {
                    spaceBuilder.AddBoundary(boundedDir, _chunkWorldCenterpoint.X + _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Down)
                {
                    spaceBuilder.AddBoundary(boundedDir, _chunkWorldCenterpoint.Y - _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Left)
                {
                    spaceBuilder.AddBoundary(boundedDir, _chunkWorldCenterpoint.X - _halfChunkSize - 1);
                }
                else throw new System.ArgumentException($"Expected a cardinal direction. Cannot bound space by direction {boundedDir}.");
            }

            _spaceBuilders.Add(spaceBuilder);

            return this;
        }

        public ChunkBuilder AddBlocks(Dictionary<BlockTypes, int> blockAmounts)
        {
            var blocks = new List<BlockTypes>();

            foreach (var kvp in blockAmounts)
            {
                for (var amountAdded = 0; amountAdded < kvp.Value; amountAdded++)
                {
                    blocks.Add(kvp.Key);
                }
            }

            AddBlocks(blocks.ToArray());

            return this;
        }

        public ChunkBuilder AddBlocks(params BlockTypes[] blocksToAdd)
        {
            // Randomly order the blocks (without actually changing their order, 
            // because that would be unnecessarily expensive and may change other
            // effects.
            var selectionOrder = Chance.ExclusiveRandomOrder(_blockBuilders.Count);

            var addedBlocks = 0;

            for (var blockIndex = 0; blockIndex < _blockBuilders.Count; blockIndex++)
            {
                // Foreach block, if it the randomized order shows it as one of the
                // first blocks, assign it to the blockType specified in the matching
                // index of blocks to add.
                if (selectionOrder[blockIndex] < blocksToAdd.Length)
                {
                    _blockBuilders[blockIndex].SetType(blocksToAdd[selectionOrder[blockIndex]]);
                    addedBlocks++;

                    // If we have added all blocks, break.
                    if (addedBlocks >= blocksToAdd.Length) break;
                }
            }

            return this;
        }

        public ChunkBuilder SetFill(BlockTypes fillBlock)
        {
            FillBlock = fillBlock;

            return this;
        }

        public BlockTypes GetAnticipatedBlockType(IntVector2 position)
        {
            var builder = _blockMap[position];

            if (builder.IsFill) return FillBlock;
            else return builder.Type;
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
            var enemiesToAdd = new List<EnemySpawn>();

            foreach (var sBuilder in _spaceBuilders)
            {
                var space = sBuilder.Build();
                spaces.Add(space);
                chunk.Register(space);

                foreach (var enemySpawn in space.GetEnemySpawnsInChunk(chunk))
                {
                    enemiesToAdd.Add(enemySpawn);
                }
            }

            if (_blueprint != null)
            {
                foreach (var space in _blueprint.Spaces)
                {
                    spaces.Add(space);
                    chunk.Register(space);
                }
            }

            foreach (var builder in _blockBuilders)
            {
                var position = builder.Position;

                var block = BlockTypes.None;

                var containingSpace = spaces.Find(space => space.Contains(position));

                if (containingSpace != null)
                {
                    block = containingSpace.GetBlockType(position);
                }
                else
                {
                    if (builder.IsFill) builder.SetType(FillBlock);
                    block = builder.Build();
                }

                if (block != BlockTypes.None)
                {
                    chunk.Register(BlockLoader.CreateBlock(block, position));
                }
            }

            foreach (var space in chunk.Spaces)
            {
                foreach (var hazardSpawn in space.GetHazardBuildersInChunk(chunk))
                {
                    var hazard = hazardSpawn.Build(chunk, space);
                    if (hazard != null) chunk.Register(hazard);
                }
            }

            foreach (var enemyToAdd in enemiesToAdd)
            {
                var enemyData = EnemySpawner.SpawnEnemy(enemyToAdd.Type, enemyToAdd.Position);
                chunk.Register(enemyData);
            }

            if (_blueprint != null)
            {
                foreach (var enemy in _blueprint.Enemies)
                {
                    enemy.SetActive(true);
                    chunk.Register(enemy);
                }
            }

            _blockBuilders.Clear();
            _blockMap.Clear();
            _spaceBuilders.Clear();

            Chunk.OnChunkChanged.Raise(chunk);
            return chunk;
        }
    }
}