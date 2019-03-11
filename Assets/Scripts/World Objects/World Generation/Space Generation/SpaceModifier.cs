using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public abstract class SpaceModifier
    {
        protected SpaceBuilder _spaceBuilder { get; private set; }

        public abstract ModifierTypes Type { get; }

        public SpaceModifier(SpaceBuilder spaceBuilder) => _spaceBuilder = spaceBuilder;

        public abstract void Modify(Space target);
    }
}