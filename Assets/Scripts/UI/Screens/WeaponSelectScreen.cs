using Economy;
using Items;
using Items.Unlocking;
using Items.Weapons;
using System.Collections.Generic;
using UI.Components.Weapons;
using UI.Effects;
using UnityEngine;

namespace UI.Screens
{
    public class WeaponSelectScreen : UIScreen
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private GameObject _weaponSelectFrameTemplate = null;
        [SerializeField]
        private Transform _contentAnchor = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private List<GameObject> _activeFrames = new List<GameObject>();

        public override void ActivateScreen()
        {
            _weaponSelectFrameTemplate.SetActive(false);

            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                ScreenGroup.Hide(),
                ScreenGroup.FadeTo(1f, .5f),
                ScreenGroup.SetInteractable(true)
            );

            sequence.Play(FillWeaponList);
        }

        public void OnRespawnClicked() => DeactivateScreen();

        private void FillWeaponList()
        {
            foreach (var activeFrame in _activeFrames)
            {
                Destroy(activeFrame);
            }

            var userInventory = GameManager.Character.Inventory;
            var userEquipment = GameManager.Character.Equipment;

            foreach (var weapon in WeaponSorter.Secondaries)
            {
                var weaponSelectGameobject = Instantiate(_weaponSelectFrameTemplate, _contentAnchor);

                var weaponSelectFrame = weaponSelectGameobject.GetComponent<WeaponDisplayFrame>();
                weaponSelectFrame.SetDisplay(WeaponDisplayDataProvider.GetDisplay(weapon));

                var weaponTransaction = _weaponOffers[weapon];

                var weaponStateToggle = weaponSelectGameobject.GetComponent<WeaponStateToggle>();
                weaponStateToggle.SetState(userInventory.HasUnlocked(weaponTransaction.Reward as UnlockItem));

                var unownedWeaponDisplay = weaponStateToggle.UnownedWeaponDisplay;
                unownedWeaponDisplay.SetTransaction(weaponTransaction, userInventory.CanAfford(weaponTransaction));

                unownedWeaponDisplay.OnRequestPurchase += OnRequestWeaponPurchase;

                var ownedWeaponDisplay = weaponStateToggle.OwnedWeaponDisplay;
                ownedWeaponDisplay.SetEquipmentState(weapon, userEquipment.EquippedSecondaryWeapon == weapon);

                ownedWeaponDisplay.OnRequestWeaponEquipmentToggle += OnRequestWeaponEquipmentToggle;

                _activeFrames.Add(weaponSelectGameobject);
                weaponSelectGameobject.SetActive(true);
            }
        }

        private void OnRequestWeaponPurchase(Transaction weaponTransaction)
        {
            if (Ledgerman.TryPurchase(weaponTransaction))
            {
                FillWeaponList();
            }
            else throw new System.ArgumentException($"User cannot purchase {weaponTransaction}.  This transaction should not have been available to them.");
        }

        private void OnRequestWeaponEquipmentToggle(WeaponTypes weapon, bool isEquipped)
        {
            var userEquipment = GameManager.Character.Equipment;

            if (isEquipped) userEquipment.EquipWeapon(weapon);
            else if (userEquipment.EquippedSecondaryWeapon == weapon) userEquipment.UnequipWeapon();

            FillWeaponList();
        }

        public override void DeactivateScreen()
        {
            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                ScreenGroup.FadeTo(0f, .5f)
            );

            sequence.Play(() =>
            {
                OnScreenDeactivated.Raise(this);
                Destroy(gameObject);
                gameObject.SetActive(false);
            });
        }

        private static readonly Dictionary<WeaponTypes, Transaction> _weaponOffers = new Dictionary<WeaponTypes, Transaction>()
        {
            { WeaponTypes.SeismicBomb, new Transaction(new Item(ItemCategories.Wealth, 1), new UnlockItem("Seismic Bomb", UnlockTypes.Weapon)) },
            { WeaponTypes.DeReconstructor, new Transaction(new Item(ItemCategories.Wealth, 25), new UnlockItem("DeReconstructor", UnlockTypes.Weapon)) },
            { WeaponTypes.DrillLauncher, new Transaction(new Item(ItemCategories.Wealth, 50), new UnlockItem("DrillLauncher", UnlockTypes.Weapon)) }
        };
    }
}