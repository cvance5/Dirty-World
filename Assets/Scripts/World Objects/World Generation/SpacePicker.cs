using System;
using System.Collections.Generic;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public static class SpacePicker
    {
        public static List<Type> SpaceBuilders = new List<Type>()
        {
            typeof(CorridorBuilder),
            typeof(ShaftBuilder)
        };

        public static SpaceBuilder Pick(ChunkBuilder chunkBuilder) => Activator.CreateInstance(SpaceBuilders.RandomItem(), chunkBuilder) as SpaceBuilder;
    }
}