using System;
using System.Collections.Generic;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace WorldObjects.WorldGeneration
{
    public class SpacePicker
    {
        public List<SpaceBuilder> SelectedSpaces = new List<SpaceBuilder>();

        private bool _canPickSpaces = true;

        private readonly ChunkBuilder _chunk;

        public SpacePicker(ChunkBuilder cBuilder)
        {
            _chunk = cBuilder;

            if (_chunk.Depth <= GameManager.World.SurfaceDepth)
            {
                CheckForSpecialCasing();

                if (_canPickSpaces) RandomlySelect();
            }
        }

        private void CheckForSpecialCasing()
        {
            if (_chunk.Position == IntVector2.Zero)
            {
                var spaceBuilder = Activator.CreateInstance(typeof(LaboratoryBuilder), _chunk) as SpaceBuilder;
                SelectedSpaces.Add(spaceBuilder);
            }

            if (_chunk.Remoteness <= 4 || _chunk.Depth <= 4)
            {
                // Nothing but the initial laboratory here...
                _canPickSpaces = false;
            }
        }

        private void RandomlySelect()
        {
            var spaceBuilder = Activator.CreateInstance(_spaces.RandomItem(), _chunk) as SpaceBuilder;
            if (Chance.OneIn(4))
            {
                spaceBuilder.AddModifier(Spaces.ModifierTypes.Cavernous);
            }

            SelectedSpaces.Add(spaceBuilder);
        }

        private static readonly List<Type> _spaces = new List<Type>()
        {
            typeof(CorridorBuilder),
            typeof(ShaftBuilder),
            typeof(MonsterDenBuilder)
        };
    }
}