using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Debug;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class SpacePicker
    {
        private List<Type> _allowedSpaces = new List<Type>();

        public SpacePicker(List<Type> initialAllowedSpaces = null)
        {
            if (initialAllowedSpaces != null)
            {
                AllowSpaces(initialAllowedSpaces);
            }
        }

        public bool NeedsMoreSpaces(ChunkBuilder sourceChunkBuilder)
        {
            var needsMoreSpaces = _allowedSpaces.Count > 0;

            if (needsMoreSpaces)
            {
                needsMoreSpaces = sourceChunkBuilder.Depth <= World.SURFACE_DEPTH;
            }

            if (needsMoreSpaces)
            {
                var numBlocksAffectedBySpaces = sourceChunkBuilder.BlockBuilders.Count(bBuilder => bBuilder.Space != null);
                var percentAffected = numBlocksAffectedBySpaces / sourceChunkBuilder.BlockBuilders.Count;
                needsMoreSpaces = percentAffected < Chance.Percent;
            }

            if (needsMoreSpaces)
            {
                needsMoreSpaces = Chance.OneIn(3);
            }

            return needsMoreSpaces;
        }

        public SpaceBuilder Select(ChunkBuilder chunk) => RandomlySelect(chunk);

        public void AllowSpaces(List<Type> allowedSpaceTypes)
        {
            foreach (var allowedType in allowedSpaceTypes)
            {
                if (typeof(SpaceBuilder).IsAssignableFrom(allowedType))
                {
                    _allowedSpaces.Add(allowedType);
                }
                else _log.Warning($"{allowedType.Name} is not a spacebuilder and cannot be allowed.");
            }
        }

        private SpaceBuilder RandomlySelect(ChunkBuilder cBuilder)
        {
            var spaceBuilder = Activator.CreateInstance(_allowedSpaces.RandomItem(), cBuilder) as SpaceBuilder;

            if (Chance.OneIn(4))
            {
                spaceBuilder.AddModifier(ModifierTypes.Cavernous);
            }

            return spaceBuilder;
        }

        private static readonly Log _log = new Log("SpacePicker");
    }
}