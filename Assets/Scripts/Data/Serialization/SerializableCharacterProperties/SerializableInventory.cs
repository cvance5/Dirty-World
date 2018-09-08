using Items;
using Newtonsoft.Json;
using Characters;

namespace Data.Serialization.SerializableCharacterProperties
{
    public class SerializableInventory : ISerializable<Inventory>
    {
        [JsonProperty("wealth")]
        public uint Wealth;

        [JsonConstructor]
        public SerializableInventory() { }

        public SerializableInventory(Inventory inventory)
        {
            Wealth = inventory.Wealth;
        }

        public Inventory ToObject()
        {
            var inventory = new Inventory();
            inventory.Add(new Item(ItemCategories.Wealth, Wealth));
            return inventory;
        }
    }
}