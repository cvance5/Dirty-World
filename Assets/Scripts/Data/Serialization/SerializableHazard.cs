using Newtonsoft.Json;
using WorldObjects.Hazards;
using WorldObjects.WorldGeneration;

namespace Data.Serialization
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableHazard : ISerializable<Hazard>
    {
        [JsonProperty("hazardType")]
        private readonly HazardTypes _type;
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonConstructor]
        public SerializableHazard() { }

        public SerializableHazard(Hazard hazard)
        {
            _type = HazardLoader.ConvertToEnum(hazard.GetType());
            _position = hazard.Position;
        }

        public Hazard ToObject() => HazardLoader.CreateHazard(_type, _position);
    }
}