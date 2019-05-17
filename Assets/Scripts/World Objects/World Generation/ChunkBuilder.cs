using Data;
using MathConcepts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;
using WorldObjects.Actors.Enemies;
using WorldObjects.Blocks;
using WorldObjects.Construction;
using WorldObjects.WorldGeneration.EnemyGeneration;
using WorldObjects.WorldGeneration.PropGeneration;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration
{
    public class ChunkBuilder
    {
        public static SmartEvent<ChunkBuilder> OnChunkBuilderChanged { get; set; } = new SmartEvent<ChunkBuilder>();

        public IntVector2 Position { get; }

        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Depth { get; }
        public int Remoteness { get; }

        private readonly List<Space> _spaces = new List<Space>();
        public List<Space> Spaces => new List<Space>(_spaces);

        private readonly Dictionary<IntVector2, Chunk> _neighborChunks = new Dictionary<IntVector2, Chunk>();
        private readonly Dictionary<IntVector2, ChunkBuilder> _neighborBuilders = new Dictionary<IntVector2, ChunkBuilder>();

        private readonly List<EnemyHealth> _enemies = new List<EnemyHealth>();
        public List<EnemyHealth> Enemies => new List<EnemyHealth>(_enemies);

        private readonly List<BlockBuilder> _blockBuilders = new List<BlockBuilder>();
        public List<BlockBuilder> BlockBuilders => new List<BlockBuilder>(_blockBuilders);

        private readonly Dictionary<IntVector2, BlockBuilder> _blockMap
                   = new Dictionary<IntVector2, BlockBuilder>();

        public int ChunkSize { get; }
        private int _halfChunkSize => ChunkSize / 2;

        public BlockTypes FillBlock { get; private set; } = BlockTypes.Dirt;

        public ChunkBuilder(IntVector2 chunkWorldCenterpoint, int chunkSize)
        {
            ChunkSize = chunkSize;
            Position = new IntVector2(chunkWorldCenterpoint);

            BottomLeftCorner = new IntVector2(chunkWorldCenterpoint.X - _halfChunkSize, chunkWorldCenterpoint.Y - _halfChunkSize);
            TopRightCorner = new IntVector2(chunkWorldCenterpoint.X + _halfChunkSize - 1, chunkWorldCenterpoint.Y + _halfChunkSize - 1);

            Depth = Position.Y / ChunkSize;
            Remoteness = Mathf.Abs(Position.X / ChunkSize) + Mathf.Abs(Position.Y / ChunkSize);

            // Initialize all blocks in chunk to the default value
            for (var row = 0; row < ChunkSize; row++)
            {
                for (var column = 0; column < ChunkSize; column++)
                {
                    var offset = new IntVector2(row, column);
                    var blockBuilder = new BlockBuilder(BottomLeftCorner + offset);
                    _blockBuilders.Add(blockBuilder);
                    _blockMap.Add(blockBuilder.Position, blockBuilder);
                }
            }

            OnChunkBuilderChanged.Raise(this);
        }

        public bool Contains(IntVector2 position) =>
                position.X >= BottomLeftCorner.X &&
                position.Y >= BottomLeftCorner.Y &&
                position.X <= TopRightCorner.X &&
                position.Y <= TopRightCorner.Y;

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

            OnChunkBuilderChanged.Raise(this);

            return this;
        }

        public ChunkBuilder AddSpace(Space spaceToAdd)
        {
            _spaces.Add(spaceToAdd);

            foreach (var bBuilder in _blockBuilders)
            {
                if (spaceToAdd.Extents.Contains(bBuilder.Position))
                {
                    bBuilder.SetSpace(spaceToAdd);
                }
            }

            OnChunkBuilderChanged.Raise(this);

            return this;
        }

        public ChunkBuilder AddEnemy(EnemyHealth enemy)
        {
            _enemies.Add(enemy);

            enemy.gameObject.SetActive(false);
            OnChunkBuilderChanged.Raise(this);

            return this;
        }

        public ChunkBuilder SetFill(BlockTypes fillBlock)
        {
            FillBlock = fillBlock;

            OnChunkBuilderChanged.Raise(this);

            return this;
        }

        public void AddNeighbor(Chunk neighbor, IntVector2 direction)
        {
            _neighborChunks[direction] = neighbor;
            _neighborBuilders.Remove(direction);
        }

        public void AddNeighbor(ChunkBuilder neighbor, IntVector2 direction) => _neighborBuilders[direction] = neighbor;
        public bool TryGetNeighborBuilder(IntVector2 direction, out ChunkBuilder neighbor) => _neighborBuilders.TryGetValue(direction, out neighbor);

        public int GetMaximalValue(IntVector2 direction)
        {
            if (direction == Directions.Up)
            {
                return Position.Y + (_halfChunkSize - 1);
            }
            else if (direction == Directions.Right)
            {
                return Position.X + (_halfChunkSize - 1);
            }
            else if (direction == Directions.Down)
            {
                return Position.Y - _halfChunkSize;
            }
            else if (direction == Directions.Left)
            {
                return Position.X - _halfChunkSize;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public BlockTypes GetAnticipatedBlockType(IntVector2 position)
        {
            var builder = _blockMap[position];

            if (builder.IsFill) return FillBlock;
            else return builder.GetBlock();
        }

        public Chunk Build()
        {
            var chunk = new GameObject($"Chunk [{Position.X}, {Position.Y}]").AddComponent<Chunk>();
            chunk.transform.position = Position;
            chunk.AssignExtents(BottomLeftCorner, TopRightCorner);
            chunk.SetState(Chunk.ChunkState.Constructing);

            return chunk;
        }

        public IEnumerator BuildCoroutine(Chunk chunk)
        {
            // Never use more than 1/3 of a frame
            var yieldTimer = new IncrementalTimer(Time.realtimeSinceStartup, 1f / 180f);

            yield return BuildBlocks(chunk, yieldTimer);
            yield return BuildFromBuilders(chunk, yieldTimer);

            foreach (var space in _spaces)
            {
                chunk.Register(space.Name);
            }

            chunk.SetState(Chunk.ChunkState.Ready);
            Chunk.OnChunkChanged.Raise(chunk);
        }

        private IEnumerator BuildBlocks(Chunk chunk, IncrementalTimer timer)
        {
            foreach (var builder in _blockBuilders)
            {
                var position = builder.Position;

                var blockType = BlockTypes.None;
                var propToBuild = PropTypes.None;

                if (builder.IsFill) builder.SetType(FillBlock);

                blockType = builder.GetBlock();
                propToBuild = builder.GetProp(); ;

                Block block = null;
                if (blockType != BlockTypes.None)
                {
                    block = BlockLoader.CreateBlock(blockType, position);
                    chunk.Register(block);
                }

                if (propToBuild != PropTypes.None)
                {
                    var prop = PropLoader.CreateProp(propToBuild, position);
                    chunk.Register(prop);

                    if (block != null)
                    {
                        prop.Assign(block);
                    }
                    prop.Assign(builder.Space);

                    prop.Initialize();
                }

                if (timer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    timer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }
        }

        private IEnumerator BuildFromBuilders(Chunk chunk, IncrementalTimer timer)
        {
            foreach (var space in _spaces)
            {
                // Register enemies as we spawn them
                foreach (var enemySpawn in space.GetEnemySpawnsInChunk(chunk))
                {
                    var enemyData = EnemySpawner.SpawnEnemy(enemySpawn.Type, enemySpawn.Position);
                    chunk.Register(enemyData);
                }

                // Now register any already-spawned enemies
                foreach (var enemy in _enemies)
                {
                    chunk.Register(enemy);
                }

                foreach (var hazardSpawn in space.GetHazardBuildersInChunk(chunk))
                {
                    var hazard = hazardSpawn.Build(chunk, space);
                    if (hazard != null) chunk.Register(hazard);
                }

                foreach (var featureSpawn in space.GetFeatureBuildersInChunk(chunk))
                {
                    var feature = featureSpawn.Build();
                    if (feature != null) chunk.Register(feature);
                }

                if (timer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    timer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }
        }

        private static readonly Log _log = new Log("ChunkBuilder");
    }
}