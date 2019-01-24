using Characters;
using Items.Weapons;
using Newtonsoft.Json;

namespace Data.Serialization.SerializableCharacterProperties
{
    public class SerializableEquipment : ISerializable<Equipment>
    {
        [JsonProperty("secondary")]
        private readonly WeaponTypes? _equippedSecondaryWeapon;

        [JsonConstructor]
        public SerializableEquipment() { }

        public SerializableEquipment(Equipment equipment)
        {
            _equippedSecondaryWeapon = equipment.EquippedSecondaryWeapon;
        }

        public Equipment ToObject()
        {
            var newEquipment = new Equipment();
            if (_equippedSecondaryWeapon.HasValue) newEquipment.EquipSecondaryWeapon(_equippedSecondaryWeapon.Value);
            return newEquipment;
        }
    }
}