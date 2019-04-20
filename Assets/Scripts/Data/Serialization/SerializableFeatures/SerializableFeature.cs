using Newtonsoft.Json;
using WorldObjects.Features;

namespace Data.Serialization.SerializableFeatures
{
    public abstract class SerializableFeature : ISerializable<Feature>
    {
        [JsonProperty("position")]
        protected readonly IntVector2 _position;

        public SerializableFeature(Feature feature) => _position = feature.Position;

        public abstract Feature ToObject();
    }
}