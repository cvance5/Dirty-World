using UnityEngine;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public static class WorldBuilder
    {
        public static void BuildInitialChunk()
        {
            var cBuilder = new ChunkBuilder(Vector2.zero);

            var sBuilder = new CorridorBuilder(cBuilder)
                .SetStartingPoint(Vector2.zero, CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(1)
                .SetLength(100);

            var sBuilder2 = new CorridorBuilder(cBuilder)
                .SetStartingPoint(Vector2.zero + (Vector2.up), CorridorBuilder.CorridorAlignment.StartFromLeft)
                .SetHeight(3)
                .SetLength(5);

            cBuilder.AddSpace(sBuilder)
                    .AddSpace(sBuilder2);

            World.Instance.RegisterChunk(cBuilder.Build());

            foreach (var dir in Directions.Compass)
            {
                BuildChunk(new IntVector2(dir.X * World.ChunkSize, dir.Y * World.ChunkSize));
            }
        }

        public static void BuildChunk(IntVector2 position)
        {
            var cBuilder = new ChunkBuilder(position);
            var sBuilder = SpacePicker.Pick(cBuilder);

            cBuilder.AddSpace(sBuilder)
                    .AddBlocks(BlockTypes.Gold, BlockTypes.Gold, BlockTypes.Gold);

            World.Instance.RegisterChunk(cBuilder.Build());
        }
    }
}
