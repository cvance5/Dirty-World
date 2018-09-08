using UnityEngine;
using UnityEngine.UI;

namespace UI.Components.Weapons
{
    public class UnownedWeaponDisplay : UIComponent
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private Text _purchaseCostLabel = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public void SetPurchaseOffer()
        {

        }
    }
}