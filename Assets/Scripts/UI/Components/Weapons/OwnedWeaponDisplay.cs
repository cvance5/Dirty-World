using UnityEngine;
using UnityEngine.UI;

namespace UI.Components.Weapons
{
    public class OwnedWeaponDisplay : UIComponent
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private Text _isEquippedLabel = null;
        [SerializeField]
        private Text _equipmentToggleLabel = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public void SetEquipped(bool isEquipped)
        {
            if (isEquipped)
            {
                _isEquippedLabel.text = "Equipped";
                _equipmentToggleLabel.text = "Unequip";
            }
            else
            {
                _isEquippedLabel.text = "Unequipped";
                _equipmentToggleLabel.text = "Equip";
            }
        }
    }
}