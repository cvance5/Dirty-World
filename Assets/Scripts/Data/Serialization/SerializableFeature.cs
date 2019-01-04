using Newtonsoft.Json;
using WorldObjects.Construction;
using WorldObjects.Features;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace Data.Serialization
{
    public class SerializableFeature : ISerializable<Feature>
    {
        [JsonProperty("type")]
        private readonly FeatureTypes _type;
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonConstructor]
        private SerializableFeature() { }

        public SerializableFeature(Feature feature)
        {
            _type = feature.Type;
            _position = feature.Position;
        }

        public Feature ToObject() => FeatureLoader.CreateFeature(_type, _position);
    }
}