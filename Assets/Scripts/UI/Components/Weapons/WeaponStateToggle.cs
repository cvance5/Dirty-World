using UnityEngine;

namespace UI.Components.Weapons
{
    public class WeaponStateToggle : UIComponent
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private UnownedWeaponDisplay _unownedWeaponDisplay = null;
        [SerializeField]
        private OwnedWeaponDisplay _ownedWeaponDisplay = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public void SetState(bool isOwned, bool isEquipped)
        {
            _unownedWeaponDisplay.SetActive(!isOwned);
            _ownedWeaponDisplay.SetActive(isOwned);

            _ownedWeaponDisplay.SetEquipped(isEquipped);
        }        
    }
}