using System;
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
                spaces.AddRange(CheckForSpecialCasing(chunk));

                if (_canPickSpaces) spaces.AddRange(RandomlySelect(chunk));
            }

            return spaces;
        }

        public void AllowSpaces(List<Type> allowedSpaceTypes)
        {
            foreach (var type in allowedSpaceTypes)
            {
                if (type.IsAssignableFrom(typeof(SpaceBuilder)))
                {
                    _allowedSpaces.Add(type);
                }
                else _log.Warning($"{type.Name} is not a spacebuilder and cannot be allowed.");
            }
        }

        private List<SpaceBuilder> CheckForSpecialCasing(ChunkBuilder chunk)
        {
            var specialCaseSpaces = new List<SpaceBuilder>();

            if (chunk.Position == IntVector2.Zero)
            {
                var spaceBuilder = Activator.CreateInstance(typeof(LaboratoryBuilder), chunk) as SpaceBuilder;
                specialCaseSpaces.Add(spaceBuilder);
            }

            if (chunk.Remoteness <= 4 || chunk.Depth <= 4)
            {
                // Nothing but the initial laboratory here...
                _canPickSpaces = false;
            }

            return specialCaseSpaces;
        }

        private List<SpaceBuilder> RandomlySelect(ChunkBuilder chunk)
        {
            var randomSpaces = new List<SpaceBuilder>();

            var spaceBuilder = Activator.CreateInstance(_allowedSpaces.RandomItem(), chunk) as SpaceBuilder;
            if (Chance.OneIn(4))
            {
                spaceBuilder.AddModifier(ModifierTypes.Cavernous);
            }

            randomSpaces.Add(spaceBuilder);

            return randomSpaces;
        }

        private static readonly Log _log = new Log("SpacePicker");
    }
}