using Items.Weapons;

namespace Characters
{
    public class Equipment
    {
        public WeaponTypes EquippedPrimaryWeapon { get; private set; } = WeaponTypes.HarpoonGun;

        public WeaponTypes? EquippedSecondaryWeapon { get; private set; }

        public void EquipPrimaryWeapon(WeaponTypes newWeapon) => EquippedPrimaryWeapon = newWeapon;

        public void EquipSecondaryWeapon(WeaponTypes newWeapon) => EquippedSecondaryWeapon = newWeapon;
        public void UnequipSecondaryWeapon() => EquippedSecondaryWeapon = null;
    }
}