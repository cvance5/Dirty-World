using UnityEngine;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public static class WorldBuilder
    {
        public static void BuildInitialChunk()
        {
            var sBuilder = new CorridorBuilder(Vector2.zero, CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(1)
                .SetLength(100);

            var sBuilder2 = new CorridorBuilder(Vector2.zero + (Vector2.up), CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(3)
                .SetLength(5);

            var cBuilder = new ChunkBuilder(Vector2.zero)
                    .AddSpace(sBuilder)
                    .AddSpace(sBuilder2);

            World.Instance.RegisterChunk(cBuilder.Build());

            foreach (var dir in Directions.Compass)
            {
                var cBuilderTemp = new ChunkBuilder(new IntVector2(dir.X * World.ChunkSize, dir.Y * World.ChunkSize));

                World.Instance.RegisterChunk(cBuilderTemp.Build());
            }
        }

        public static void BuildChunk(IntVector2 position)
        {
            var cBuilder = new ChunkBuilder(position);
            World.Instance.RegisterChunk(cBuilder.Build());
        }
    }
}
