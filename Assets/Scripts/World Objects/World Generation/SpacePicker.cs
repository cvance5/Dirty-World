using System;
using System.Collections.Generic;
using WorldObjects.Spaces;
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
                spaceBuilders.Add(Activator.CreateInstance(_spaces.RandomItem(), cBuilder) as SpaceBuilder);
            }

            return spaceBuilders;
        }

        public static Space ApplyModifier(Space space, ModifierTypes type)
        {
            switch (type)
            {
                case ModifierTypes.Cavernous: return new CavernousModifier(space);
                default: throw new ArgumentException($"Unknown modifier type: {type}.");
            }
        }
    }
}