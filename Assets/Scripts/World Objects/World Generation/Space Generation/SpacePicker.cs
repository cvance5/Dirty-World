﻿using System;
using System.Collections.Generic;
using Utilities.Debug;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class SpacePicker
    {
        private bool _canPickSpaces = true;
        private List<Type> _allowedSpaces = new List<Type>();

        public SpacePicker(List<Type> initialAllowedSpaces = null)
        {
            if (initialAllowedSpaces != null)
            {
                AllowSpaces(initialAllowedSpaces);
            }
        }

        public List<SpaceBuilder> Select(ChunkBuilder chunk)
        {
            var spaces = new List<SpaceBuilder>();

            _canPickSpaces = _allowedSpaces.Count > 0;

            if (chunk.Depth <= World.SURFACE_DEPTH)
            {
                if (_canPickSpaces) spaces.AddRange(RandomlySelect(chunk));
            }

            return spaces;
        }

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

        private List<SpaceBuilder> RandomlySelect(ChunkBuilder chunk)
        {
            var randomSpaces = new List<SpaceBuilder>();

            if (Chance.OneIn(3))
            {
                var spaceBuilder = Activator.CreateInstance(_allowedSpaces.RandomItem(), chunk) as SpaceBuilder;

                if (Chance.OneIn(4))
                {
                    spaceBuilder.AddModifier(ModifierTypes.Cavernous);
                }

                randomSpaces.Add(spaceBuilder);
            }

            return randomSpaces;
        }

        private static readonly Log _log = new Log("SpacePicker");
    }
}