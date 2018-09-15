using Items.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components.Weapons
{
    public class EquipWeaponDisplay : UIComponent
    {
        public SmartEvent<WeaponTypes, bool> OnRequestWeaponEquipmentToggle = new SmartEvent<WeaponTypes, bool>();

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private Text _isEquippedLabel = null;
        [SerializeField]
        private Button _equipToggleButton = null;
        [SerializeField]
        private Text _equipToggleButtonLabel = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private WeaponTypes _weapon;
        private bool _isEquipped;

        private void Awake()
        {
            _equipToggleButton.onClick.AddListener(OnClick);
        }

        public void SetEquipmentState(WeaponTypes weapon, bool isEquipped)
        {
            _weapon = weapon;
            _isEquipped = isEquipped;

            if (_isEquipped)
            {
                _isEquippedLabel.text = "Equipped";
                _equipToggleButtonLabel.text = "Unequip";
            }
            else
            {
                _isEquippedLabel.text = "Unequipped";
                _equipToggleButtonLabel.text = "Equip";
            }
        }

        private void OnClick() => OnRequestWeaponEquipmentToggle.Raise(_weapon, !_isEquipped);
    }
}