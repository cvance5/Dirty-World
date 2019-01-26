using UnityEngine;
using WorldObjects.Actors.Player.Guns;

namespace WorldObjects.Actors.Player
{
    public class PlayerWeapons : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly for Unity Serialization
        [SerializeField]
        private PlayerHealth _health = null;
#pragma warning restore IDE0044 // Add readonly modifier
        private Gun _primary = null;
        private Gun _secondary = null;
        private ElectricalHands _hands = null;

        private WeaponMode _weaponMode = WeaponMode.Primary;

        private void Awake() => _health.OnActorDeath += OnPlayerDeath;

        private void Update()
        {
            if (_weaponMode == WeaponMode.Primary)
            {
                if (Input.GetButtonDown("Alternate Fire"))
                {
                    _primary.AlternateFire();
                }
                else if (Input.GetButtonDown("Fire"))
                {
                    _primary.Fire();
                }
            }
            else if (_weaponMode == WeaponMode.Secondary && _secondary != null)
            {
                if (Input.GetButtonDown("Alternate Fire"))
                {
                    _secondary.AlternateFire();
                }
                else if (Input.GetButtonDown("Fire"))
                {
                    _secondary.Fire();
                }
            }
            else if (_weaponMode == WeaponMode.Hands)
            {
                if (Input.GetButtonDown("Alternate Fire"))
                {
                    _hands.AlternateFire();
                }
                else if (Input.GetButtonDown("Fire"))
                {
                    _hands.Fire();
                }
            }

            if (Input.GetButtonDown("Switch Weapon"))
            {
                if (_weaponMode == WeaponMode.Primary && _secondary != null)
                {
                    _weaponMode = WeaponMode.Secondary;
                }
                else if (_weaponMode == WeaponMode.Hands)
                {
                    _weaponMode = WeaponMode.Primary;
                }
                else
                {
                    _weaponMode = WeaponMode.Hands;
                }
            }
        }

        public void EquipPrimary(Gun newPrimary)
        {
            if (_primary != null)
            {
                Destroy(_primary.gameObject);
            }

            _primary = newPrimary;
            _primary.transform.SetParent(transform);
        }

        public void EquipSecondary(Gun newSecondary)
        {
            if (_secondary != null)
            {
                Destroy(_secondary.gameObject);
            }

            _secondary = newSecondary;

            // You could be equipping nothing, i.e. unequipping
            if (newSecondary != null)
            {
                _secondary.transform.SetParent(transform);
            }
        }

        public void EquipHands(ElectricalHands hands)
        {
            if(_hands != null)
            {
                Destroy(_hands.gameObject);
            }

            _hands = hands;
            _hands.transform.SetParent(transform);
        }

        private void OnPlayerDeath(ActorHealth playerData)
        {
            playerData.OnActorDeath -= OnPlayerDeath;
            Destroy(this);
        }

        private enum WeaponMode
        {
            Primary,
            Secondary,
            Hands
        }
    }
}