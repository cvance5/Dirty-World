using MathConcepts;
using WorldObjects.Features;

namespace WorldObjects.WorldGeneration.FeatureGeneration
{
    public abstract class FeatureBuilder
    {
        public abstract IntVector2 Position { get; protected set; }

        public abstract bool IsValid { get; }

        public abstract Feature Build();
    }
}