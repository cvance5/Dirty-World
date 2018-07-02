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
                .SetLength(15);

            var sBuilder2 = new CorridorBuilder(Vector2.zero + (Vector2.up), CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(3)
                .SetLength(5);

            var cBuilder = new ChunkBuilder(Vector2.zero)
                    .RemoveAtWorldPositions(Vector2.zero, Vector2.left, Vector2.right)
                    .AddSpace(sBuilder)
                    .AddSpace(sBuilder2);

            var initialChunk = cBuilder.Build();
            World.Instance.RegisterChunk(initialChunk);
        }
    }
}
