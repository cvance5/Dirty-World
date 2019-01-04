using WorldObjects.WorldGeneration.FeatureGeneration;

namespace WorldObjects.Features
{
    public class WallLight : Feature, IPowerable
    {
        public bool IsPowered { get; private set; }
        public override string ObjectName => $"Wall Light at {Position}";

        public override FeatureTypes Type { get; } = FeatureTypes.WallLight;
    }
}