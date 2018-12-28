using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.Construction;
using WorldObjects.WorldGeneration.EnemyGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration
{
    public class ChunkBuilder
    {
        public static SmartEvent<Chunk> OnChunkBuilt = new SmartEvent<Chunk>();

        public IntVector2 Position { get; }

        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Depth { get; }
        public int Remoteness { get; }

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
            Position = new IntVector2(chunkWorldCenterpoint);

            var startingPoint = new IntVector2(-_halfChunkSize, -_halfChunkSize);

            BottomLeftCorner = new IntVector2(chunkWorldCenterpoint.X - _halfChunkSize, chunkWorldCenterpoint.Y - _halfChunkSize);
            TopRightCorner = new IntVector2(chunkWorldCenterpoint.X + _halfChunkSize - 1, chunkWorldCenterpoint.Y + _halfChunkSize - 1);

            Depth = Position.Y / _chunkSize;
            Remoteness = Mathf.Abs(Position.X / _chunkSize) + Mathf.Abs(Position.Y / _chunkSize);

            // Initialize all blocks in chunk to the default value
            for (var row = 0; row < _chunkSize; row++)
            {
                for (var column = 0; column < _chunkSize; column++)
                {
                    var offset = new IntVector2(row, column);
                    var positionInChunk = startingPoint + offset;
                    var blockBuilder = new BlockBuilder(Position + positionInChunk);
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
            }

            if (Depth == GameManager.World?.SurfaceDepth)
            {
                _boundedDirections.Add(Directions.Up);
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
                    spaceBuilder.AddBoundary(boundedDir, Position.Y + _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Right)
                {
                    spaceBuilder.AddBoundary(boundedDir, Position.X + _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Down)
                {
                    spaceBuilder.AddBoundary(boundedDir, Position.Y - _halfChunkSize - 1);
                }
                else if (boundedDir == Directions.Left)
                {
                    spaceBuilder.AddBoundary(boundedDir, Position.X - _halfChunkSize - 1);
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

        public IEnumerator Build()
        {
            var chunk = new GameObject($"Chunk [{Position.X}, {Position.Y}]").AddComponent<Chunk>();
            chunk.transform.position = Position;
            chunk.AssignExtents
            (
                new IntVector2(Position.X - (_chunkSize / 2), Position.Y - (_chunkSize / 2)),
                new IntVector2(Position.X + (_chunkSize / 2), Position.Y + (_chunkSize / 2))
            );

            var spaces = new List<Space>();

            // Never use more than 1/3 of a frame
            var yieldTimer = new IncrementalTimer(Time.realtimeSinceStartup, 1f / 180f);

            yield return BuildSpaces(chunk, spaces, yieldTimer);
            yield return BuildBlocks(chunk, spaces, yieldTimer);
            yield return BuildHazardsAndEnemies(chunk, spaces, yieldTimer);

            OnChunkBuilt.Raise(chunk);
            Chunk.OnChunkChanged.Raise(chunk);
        }

        private IEnumerator BuildSpaces(Chunk chunk, List<Space> spaces, IncrementalTimer timer)
        {
            // Build and register the spaces, as the chunk may realize they are bounded
            // and need to change before any other work happens
            foreach (var sBuilder in _spaceBuilders)
            {
                if (sBuilder.IsValid)
                {
                    var space = sBuilder.Build();
                    spaces.Add(space);
                    chunk.Register(space);
                }

                if (timer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    timer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }

            if (_blueprint != null)
            {
                foreach (var space in _blueprint.Spaces)
                {
                    spaces.Add(space);
                    chunk.Register(space);

                    if (timer.CheckIncrement(Time.realtimeSinceStartup))
                    {
                        yield return null;
                        timer.AdvanceIncrement(Time.realtimeSinceStartup);
                    }
                }
            }
        }

        private IEnumerator BuildBlocks(Chunk chunk, List<Space> spaces, IncrementalTimer timer)
        {
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

                if (timer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    timer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }
        }

        private IEnumerator BuildHazardsAndEnemies(Chunk chunk, List<Space> spaces, IncrementalTimer timer)
        {
            foreach (var space in chunk.Spaces)
            {
                foreach (var hazardSpawn in space.GetHazardBuildersInChunk(chunk))
                {
                    var hazard = hazardSpawn.Build(chunk, space);
                    if (hazard != null) chunk.Register(hazard);
                }

                foreach (var enemySpawn in space.GetEnemySpawnsInChunk(chunk))
                {
                    var enemyData = EnemySpawner.SpawnEnemy(enemySpawn.Type, enemySpawn.Position);
                    chunk.Register(enemyData);
                }

                if (timer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    timer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }

            if (_blueprint != null)
            {
                foreach (var enemy in _blueprint.Enemies)
                {
                    enemy.SetActive(true);
                    chunk.Register(enemy);
                }
            }
        }
    }
}