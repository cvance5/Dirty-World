using UI.Components.Economy;
using UnityEngine;

namespace UI.Components.Weapons
{
    public class WeaponStateToggle : UIComponent
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private PurchaseItemDisplay _unownedWeaponDisplay = null;
        public PurchaseItemDisplay UnownedWeaponDisplay => _unownedWeaponDisplay;
        [SerializeField]
        private EquipWeaponDisplay _ownedWeaponDisplay = null;
        public EquipWeaponDisplay OwnedWeaponDisplay => _ownedWeaponDisplay;
#pragma warning restore IDE0044 // Add readonly modifier

        public void SetState(bool isOwned)
        {
            _unownedWeaponDisplay.SetActive(!isOwned);
            _ownedWeaponDisplay.SetActive(isOwned);
        }
    }
}