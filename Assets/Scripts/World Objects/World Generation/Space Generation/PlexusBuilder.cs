using MathConcepts;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class PlexusBuilder : SpaceBuilder
    {
        public override bool IsValid => throw new System.NotImplementedException();

        private PlexusOrientation _orientation;

        IntVector2[,] _grid;

        public PlexusBuilder(ChunkBuilder chunkBuilder) 
            : base(chunkBuilder)
        {
            _orientation = Enum<PlexusOrientation>.Random;
        }
        
        public PlexusBuilder SetOrientation(PlexusOrientation orientation)
        {
            _orientation = orientation;
            return this;
        }

        public override SpaceBuilder Align(IntVector2 direction, int amount) => throw new System.NotImplementedException();
        public override void Clamp(IntVector2 direction, int amount) => throw new System.NotImplementedException();
        public override bool Contains(IntVector2 point) => throw new System.NotImplementedException();
        public override void Cut(IntVector2 direction, int amount) => throw new System.NotImplementedException();
        public override int GetMaximalValue(IntVector2 direction) => throw new System.NotImplementedException();
        public override IntVector2 GetRandomPoint() => throw new System.NotImplementedException();
        public override int PassesBy(IntVector2 direction, int amount) => throw new System.NotImplementedException();
        public override void Shift(IntVector2 shift) => throw new System.NotImplementedException();
        protected override Spaces.Space BuildRaw() => throw new System.NotImplementedException();

        public enum PlexusOrientation
        {
            Horizontal,
            Vertical
        }
    }
}