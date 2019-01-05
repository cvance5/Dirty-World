using System.Collections.Generic;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects
{
    public static class WorldSizer
    {
        public static int ChunkSize { get; private set; }

        public static void SetChunkSize(int size) => ChunkSize = size;

        public static IntVector2 GetNearestChunkPosition(IntVector2 worldPosition)
        {
            var chunksX = worldPosition.X / ChunkSize;
            var chunksY = worldPosition.Y / ChunkSize;

            return new IntVector2(chunksX * ChunkSize, chunksY * ChunkSize);
        }

        public static List<IntVector2> GetAffectedChunkPositions(SpaceBuilder spaceBuilder)
        {
            var minX = spaceBuilder.GetMaximalValue(Directions.Left);
            var maxX = spaceBuilder.GetMaximalValue(Directions.Right);

            var minY = spaceBuilder.GetMaximalValue(Directions.Down);
            var maxY = spaceBuilder.GetMaximalValue(Directions.Up);

            var minChunkX = minX / ChunkSize;
            var maxChunkX = maxX / ChunkSize;
            var minChunkY = minY / ChunkSize;
            var maxChunkY = maxY / ChunkSize;

            var affectedChunkPositions = new List<IntVector2>();

            for (var chunkX = minChunkX; chunkX <= maxChunkX; chunkX++)
            {
                for (var chunkY = minChunkY; chunkY <= maxChunkY; chunkY++)
                {
                    affectedChunkPositions.Add(new IntVector2(chunkX * ChunkSize, chunkY * ChunkSize));
                }
            }

            return affectedChunkPositions;
        }
    }
}