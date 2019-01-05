﻿using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;
using WorldObjects.Actors.Enemies;
using WorldObjects.Blocks;
using WorldObjects.Construction;
using WorldObjects.WorldGeneration.EnemyGeneration;
using WorldObjects.WorldGeneration.FeatureGeneration;

using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration
{
    public class ChunkBuilder
    {
        public static SmartEvent<ChunkBuilder> OnChunkBuilderChanged { get; set; } = new SmartEvent<ChunkBuilder>();
        public static SmartEvent<Chunk> OnChunkBuilt { get; set; } = new SmartEvent<Chunk>();

        public IntVector2 Position { get; }

        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Depth { get; }
        public int Remoteness { get; }

        private readonly List<Space> _spaces = new List<Space>();
        public List<Space> Spaces => new List<Space>(_spaces);

        private readonly List<EnemyData> _enemies = new List<EnemyData>();
        public List<EnemyData> Enemies = new List<EnemyData>();

        private readonly List<BlockBuilder> _blockBuilders = new List<BlockBuilder>();
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

            OnChunkBuilderChanged.Raise(this);

            return this;
        }

        public ChunkBuilder AddSpaces(List<Space> spacesToAdd)
        {
            _spaces.AddRange(spacesToAdd);

            OnChunkBuilderChanged.Raise(this);
            
            return this;
        }

        public ChunkBuilder AddEnemy(EnemyData enemy)
        {
            _enemies.Add(enemy);

            OnChunkBuilderChanged.Raise(this);

            return this;
        }

        public ChunkBuilder SetFill(BlockTypes fillBlock)
        {
            FillBlock = fillBlock;

            OnChunkBuilderChanged.Raise(this);

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
            chunk.AssignExtents(BottomLeftCorner, TopRightCorner);

            // Never use more than 1/3 of a frame
            var yieldTimer = new IncrementalTimer(Time.realtimeSinceStartup, 1f / 180f);

            yield return BuildBlocks(chunk, yieldTimer);
            yield return BuildHazardsAndEnemies(chunk, yieldTimer);

            OnChunkBuilt.Raise(chunk);
            Chunk.OnChunkChanged.Raise(chunk);
        }

        private IEnumerator BuildBlocks(Chunk chunk, IncrementalTimer timer)
        {
            foreach (var builder in _blockBuilders)
            {
                var position = builder.Position;

                var block = BlockTypes.None;
                var feature = FeatureTypes.None;

                var containingSpace = _spaces.Find(space => space.Contains(position));

                if (containingSpace != null)
                {
                    block = containingSpace.GetBlockType(position);
                    feature = containingSpace.GetFeatureType(position);
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

                if (feature != FeatureTypes.None)
                {
                    chunk.Register(FeatureLoader.CreateFeature(feature, position));
                }

                if (timer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    timer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }
        }

        private IEnumerator BuildHazardsAndEnemies(Chunk chunk, IncrementalTimer timer)
        {
            foreach (var space in _spaces)
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

                foreach (var enemy in _enemies)
                {
                    chunk.Register(enemy);
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