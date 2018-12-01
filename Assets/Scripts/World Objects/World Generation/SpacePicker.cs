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
            typeof(ShaftBuilder),
            typeof(MonsterDenBuilder)
        };

        public static List<SpaceBuilder> Pick(ChunkBuilder cBuilder)
        {
            var spaceBuilders = new List<SpaceBuilder>();

            if (cBuilder.Depth <= GameManager.World.SurfaceDepth)
            {
                var spaceBuilder = Activator.CreateInstance(_spaces.RandomItem(), cBuilder) as SpaceBuilder;
                if(Chance.OneIn(4))
                {
                    spaceBuilder.AddModifier(Spaces.ModifierTypes.Cavernous);
                }

                spaceBuilders.Add(spaceBuilder);
            }

            return spaceBuilders;
        }
    }
}