using Characters;
using Items;
using Newtonsoft.Json;

namespace Data.Serialization.SerializableCharacterProperties
{
    public class SerializableInventory : ISerializable<Inventory>
    {
        [JsonProperty("wealth")]
        private readonly uint _wealth;

        [JsonConstructor]
        public SerializableInventory() { }

        public SerializableInventory(Inventory inventory)
        {
            _wealth = inventory.Wealth;
        }

        public Inventory ToObject()
        {
            var inventory = new Inventory();
            inventory.Add(new Item(ItemCategories.Wealth, _wealth));
            return inventory;
        }
    }
}