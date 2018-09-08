using Items.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components.Weapons
{
    public class WeaponDisplayFrame : UIComponent
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private Text _weaponNameLabel = null;
        [SerializeField]
        private Image _weaponImage = null;
        [SerializeField]
        private Text _weaponDescriptionBody = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public void SetDisplay(WeaponDisplayData displayForWeapon)
        {
            _weaponNameLabel.text = displayForWeapon.Name;
            _weaponImage.sprite = displayForWeapon.Image;
            _weaponDescriptionBody.text = displayForWeapon.Description;
        }
    }
}