using Items.Weapons;

namespace Characters
{
    public class Equipment
    {
        public WeaponTypes? EquippedSecondaryWeapon { get; private set; }

        public void EquipWeapon(WeaponTypes newWeapon)
        {
            EquippedSecondaryWeapon = newWeapon;
        }

        public void UnequipWeapon()
        {
            EquippedSecondaryWeapon = null;
        }
    }
}