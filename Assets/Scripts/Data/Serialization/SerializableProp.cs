using Newtonsoft.Json;
using WorldObjects.Construction;
using WorldObjects.Props;
using WorldObjects.WorldGeneration.PropGeneration;

namespace Data.Serialization
{
    public class SerializableProp : ISerializable<Prop>
    {
        [JsonProperty("type")]
        private readonly PropTypes _type;
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonConstructor]
        private SerializableProp() { }

        public SerializableProp(Prop prop)
        {
            _type = prop.Type;
            _position = prop.Position;
        }

        public Prop ToObject() => PropLoader.CreateProp(_type, _position);
    }
}