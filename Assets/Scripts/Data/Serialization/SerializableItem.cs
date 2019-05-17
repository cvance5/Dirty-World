using Items.ItemActors;
using MathConcepts;
using Newtonsoft.Json;

namespace Data.Serialization
{
    public class SerializableItem : ISerializable<ItemActor>
    {
        [JsonProperty("type")]
        private readonly ItemActorTypes _type;
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonConstructor]
        private SerializableItem() { }

        public SerializableItem(ItemActor item)
        {
            _type = item.Type;
            _position = item.Position;
        }

        public ItemActor ToObject()
        {
            var item = ItemLoader.CreateItem(_type, _position);
            return item;
        }
    }
}