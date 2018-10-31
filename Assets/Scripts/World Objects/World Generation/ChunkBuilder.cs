using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.Hazards;
using WorldObjects.WorldGeneration.EnemyGeneration;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration
{
    public class ChunkBuilder
    {
        public IntVector2 BottomLeft { get; private set; }
        public IntVector2 TopRight { get; private set; }

        public int Depth { get; private set; }
        public int Remoteness { get; private set; }

        private IntVector2 _chunkWorldCenterpoint;
        private List<BlockBuilder> _blockBuilders = new List<BlockBuilder>();
        private List<SpaceBuilder> _spaceBuilders = new List<SpaceBuilder>();
        private List<IntVector2> _boundedDirections = new List<IntVector2>();

        private readonly ChunkBlueprint _blueprint;

        private static int _chunkSize = GameManager.Instance.Settings.ChunkSize;
        private static int _halfChunkSize = _chunkSize / 2;

        private static BlockTypes _fillBlock = BlockTypes.Dirt;

        public ChunkBuilder(IntVector2 chunkWorldCenterpoint, ChunkBlueprint blueprint = null)
        {
            _chunkWorldCenterpoint = new IntVector2(chunkWorldCenterpoint);

            var startingPoint = new IntVector2(-_halfChunkSize, -_halfChunkSize);

            BottomLeft = new IntVector2(chunkWorldCenterpoint.X - _halfChunkSize, chunkWorldCenterpoint.Y - _halfChunkSize);
            TopRight = new IntVector2(chunkWorldCenterpoint.X + _halfChunkSize, chunkWorldCenterpoint.Y + _halfChunkSize);

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
                }
            }

            foreach (var dir in Directions.Cardinals)
            {
                if (GameManager.World.GetChunkNeighbor(chunkWorldCenterpoint, dir) != null)
                {
                    _boundedDirections.Add(dir);
                }
            }

            _blueprint = blueprint;
        }

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
                    spaceBuilder.Clamp(boundedDir, _chunkWorldCenterpoint.Y + _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Right)
                {
                    spaceBuilder.Clamp(boundedDir, _chunkWorldCenterpoint.X + _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Down)
                {
                    spaceBuilder.Clamp(boundedDir, _chunkWorldCenterpoint.Y - _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Left)
                {
                    spaceBuilder.Clamp(boundedDir, _chunkWorldCenterpoint.X - _halfChunkSize - 1);
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
            _fillBlock = fillBlock;

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
            var enemiesToAdd = new Dictionary<IntVector2, EnemyTypes>();

            foreach (var sBuilder in _spaceBuilders)
            {
                var space = sBuilder.Build();
                spaces.Add(space);
                chunk.Register(space);

                foreach (var enemy in space.GetEnemySpawnsInChunk(chunk))
                {
                    enemiesToAdd.Add(enemy.Key, enemy.Value);
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

            var hazardsToAdd = new List<Hazard>();

            foreach (var builder in _blockBuilders)
            {
                var position = builder.WorldPosition;

                var blockToBuild = BlockTypes.None;

                var containingSpace = spaces.Find(space => space.Contains(position));

                if (containingSpace != null)
                {
                    blockToBuild = containingSpace.GetBlock(position);

                    if (containingSpace.IsHazardous)
                    {
                        var hazardType = containingSpace.GetHazard(position);

                        if (hazardType != HazardTypes.None)
                        {
                            hazardsToAdd.Add(HazardLoader.CreateHazard(hazardType, position));
                        }
                    }
                }
                else
                {
                    if (builder.IsFill) builder.SetType(_fillBlock);
                    blockToBuild = builder.Build();
                }

                if (blockToBuild != BlockTypes.None)
                {
                    chunk.Register(BlockLoader.CreateBlock(blockToBuild, position));
                }
            }

            foreach (var hazardToAdd in hazardsToAdd)
            {
                chunk.Register(hazardToAdd);
            }

            foreach (var enemyToAdd in enemiesToAdd)
            {
                var enemyData = EnemySpawner.SpawnEnemy(enemyToAdd.Value, enemyToAdd.Key);
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
            _spaceBuilders.Clear();

            Chunk.OnChunkChanged.Raise(chunk);
            return chunk;
        }
    }
}