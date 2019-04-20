using WorldObjects.Features;

namespace Data.Serialization.SerializableFeatures
{
    public static class SerializableFeatureHelper
    {
        public static SerializableFeature ToSerializableFeature(Feature feature)
        {
            if (feature is Elevator)
            {
                return new SerializableElevator(feature as Elevator);
            }
            else throw new System.Exception($"Unknown feature type: {feature.GetType().Name}. Cannot serialize.");
        }
    }
}