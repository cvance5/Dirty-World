﻿using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.Hazards;

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

        private static int _chunkSize = GameManager.World.ChunkSize;
        private static int _halfChunkSize = _chunkSize / 2;

        private static BlockTypes _fillBlock = BlockTypes.Dirt;

        public ChunkBuilder(IntVector2 chunkWorldCenterpoint)
        {
            _chunkWorldCenterpoint = new IntVector2(chunkWorldCenterpoint);

            IntVector2 startingPoint = new IntVector2(-_halfChunkSize, -_halfChunkSize);

            BottomLeft = new IntVector2(chunkWorldCenterpoint.X - _halfChunkSize, chunkWorldCenterpoint.Y - _halfChunkSize);
            TopRight = new IntVector2(chunkWorldCenterpoint.X + _halfChunkSize, chunkWorldCenterpoint.Y + _halfChunkSize);

            Depth = _chunkWorldCenterpoint.Y / _chunkSize;
            Remoteness = Mathf.Abs(_chunkWorldCenterpoint.X / _chunkSize) + Mathf.Abs(_chunkWorldCenterpoint.Y / _chunkSize);

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
                if (GameManager.World.GetNeighborOfChunk(chunkWorldCenterpoint, dir) != null)
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

        public ChunkBuilder AddBlocks(Dictionary<BlockTypes, int> blockAmounts)
        {
            var blocks = new List<BlockTypes>();

            foreach (var kvp in blockAmounts)
            {
                for (int amountAdded = 0; amountAdded < kvp.Value; amountAdded++)
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

            int addedBlocks = 0;

            for (int blockIndex = 0; blockIndex < _blockBuilders.Count; blockIndex++)
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
            foreach (var sBuilder in _spaceBuilders)
            {
                var space = sBuilder.Build();
                spaces.Add(space);
                chunk.Register(space);
            }

            foreach (var dir in Directions.Cardinals)
            {
                var neighbor = (GameManager.World.GetNeighborOfChunk(_chunkWorldCenterpoint, dir));

                if (neighbor != null)
                {
                    foreach (var overlappingSpace in neighbor.GetSpacesReachingEdge(-dir))
                    {
                        spaces.Add(overlappingSpace);
                        chunk.Register(overlappingSpace);
                    }
                }
            }

            List<Hazard> hazardsToAdd = new List<Hazard>();

            foreach (var builder in _blockBuilders)
            {
                var position = builder.WorldPosition;

                BlockTypes blockToBuild = BlockTypes.None;

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

            _blockBuilders.Clear();
            _spaceBuilders.Clear();

            Chunk.OnChunkChanged.Raise(chunk);
            return chunk;
        }
    }
}