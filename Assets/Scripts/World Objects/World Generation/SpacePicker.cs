using System;
using System.Collections.Generic;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public static class SpacePicker
    {
        private static readonly List<Type> _spaces = new List<Type>()
        {
            typeof(CorridorBuilder),
            typeof(ShaftBuilder)//,            typeof(MonsterDenBuilder)
        };

        public static SpaceBuilder Pick(ChunkBuilder cBuilder) => Activator.CreateInstance(_spaces.RandomItem(), cBuilder) as SpaceBuilder;
    }
}