using Data;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration;

namespace WorldObjects
{
    public class World : MonoBehaviour
    {
        public const int SURFACE_DEPTH = 0;
        public const int CHUNK_SIZE = 16;

        public int ChunkActivationDepth { get; set; } = 2;

        public ChunkArchitect ChunkArchitect { get; private set; }
        public SpaceArchitect SpaceArchitect { get; private set; }

        private ITrackable _activationCatalyst;

        public void Initialize(ChunkArchitect chunkArchitect, SpaceArchitect spaceArchitect)
        {
            ChunkArchitect = chunkArchitect;
            SpaceArchitect = spaceArchitect;
        }

        public void ListenTo(ITrackable activationCatalyst)
        {
            if (_activationCatalyst != null)
            {
                throw new System.InvalidOperationException($"World cannot listen to multiple catalysts at a time.");
            }
            _activationCatalyst = activationCatalyst;

            PositionTracker.Subscribe(activationCatalyst, OnPositionUpdate);
            var position = PositionTracker.GetCurrentPosition(activationCatalyst);

            OnPositionUpdate(activationCatalyst, null, position);
        }

        private void OnPositionUpdate(ITrackable trackable, PositionData oldPosition, PositionData newPosition)
        {
            if (newPosition.Chunk != null)
            {
                var activeChunkList = DetermineActiveChunks(newPosition.Chunk.Position);
                ChunkArchitect.SetActiveChunks(activeChunkList);
                SpaceArchitect.SetActiveSpaces(ChunkArchitect.ActiveChunks);
            }
        }

        private List<IntVector2> DetermineActiveChunks(IntVector2 currentChunkPosition)
        {
            var activeChunks = new List<IntVector2>();

            var minChunkX = (currentChunkPosition.X / CHUNK_SIZE) - ChunkActivationDepth;
            var maxChunkX = minChunkX + (ChunkActivationDepth * 2);

            var minChunkY = (currentChunkPosition.Y / CHUNK_SIZE) - ChunkActivationDepth;
            var maxChunkY = minChunkY + (ChunkActivationDepth * 2);

            for (var chunkX = minChunkX; chunkX <= maxChunkX; chunkX++)
            {
                for (var chunkY = minChunkY; chunkY <= maxChunkY; chunkY++)
                {
                    activeChunks.Add(new IntVector2(chunkX * CHUNK_SIZE, chunkY * CHUNK_SIZE));
                }
            }

            return activeChunks;
        }

        public Block GetBlock(IntVector2 position)
        {
            var containingCHunk = ChunkArchitect.GetContainingChunk(position);
            return containingCHunk.GetBlockForPosition(position);
        }

        public List<Block> GetNeighbors(Block block)
        {
            var neighbors = new List<Block>();

            foreach (var dir in Directions.Cardinals)
            {
                var neighborPos = block.Position + dir;
                var neighbor = GetBlock(neighborPos);

                if (neighbor != null) neighbors.Add(neighbor);
            }

            return neighbors;
        }

        public void Destroy()
        {
            if (_activationCatalyst != null)
            {
                PositionTracker.Unsubscribe(_activationCatalyst, OnPositionUpdate);
                _activationCatalyst = null;
            }

            ChunkArchitect.Destroy();
        }
    }
}