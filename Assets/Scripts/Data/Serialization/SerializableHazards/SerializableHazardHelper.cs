using WorldObjects.Hazards;

namespace Data.Serialization.SerializableHazards
{
    public static class SerializableHazardHelper
    {
        public static SerializableHazard ToSerializableHazard(Hazard hazard)
        {
            if (hazard is StalagHazard)
            {
                return new SerializableStalag(hazard as StalagHazard);
            }
            else throw new System.Exception($"Unknown hazard type: {hazard.GetType().Name}.  Cannot serialize.");
        }
    }
}
