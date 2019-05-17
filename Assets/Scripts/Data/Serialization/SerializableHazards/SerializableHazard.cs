using MathConcepts;
using Newtonsoft.Json;
using WorldObjects.Hazards;

namespace Data.Serialization.SerializableHazards
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class SerializableHazard : ISerializable<Hazard>
    {
        [JsonProperty("hazardType")]
        private readonly HazardTypes _type;
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        public abstract Hazard ToObject();
    }
}