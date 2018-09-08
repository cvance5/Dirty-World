using Items.Weapons;
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
            foreach (var weapon in WeaponSorter.Secondaries)
            {
                var weaponSelectGameobject = Instantiate(_weaponSelectFrameTemplate, _contentAnchor);

                var weaponSelectFrame = weaponSelectGameobject.GetComponent<WeaponDisplayFrame>();
                weaponSelectFrame.SetDisplay(WeaponDisplayData.GetDisplay(weapon));

                var weaponStateToggle = weaponSelectGameobject.GetComponent<WeaponStateToggle>();
                weaponStateToggle.SetState(false, false);

                weaponSelectGameobject.SetActive(true);
            }
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
    }
}