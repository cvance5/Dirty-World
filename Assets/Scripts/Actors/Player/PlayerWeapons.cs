using Actors.Player.Guns;
using UnityEngine;

namespace Actors.Player
{
    public class PlayerWeapons : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly for Unity Serialization
        [SerializeField]
        private Gun _primary = null;
        [SerializeField]
        private Gun _secondary = null;
        [SerializeField]
        private PlayerData _data = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private WeaponMode _weaponMode = WeaponMode.Primary;

        private void Awake() => _data.OnActorDeath += OnPlayerDeath;

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

            if (Input.GetButtonDown("Switch Weapon"))
            {
                if (_weaponMode == WeaponMode.Primary && _secondary != null)
                {
                    _weaponMode = WeaponMode.Secondary;
                }
                else if (_weaponMode == WeaponMode.Secondary)
                {
                    _weaponMode = WeaponMode.Primary;
                }
            }
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

        private void OnPlayerDeath(ActorData playerData)
        {
            playerData.OnActorDeath -= OnPlayerDeath;
            Destroy(this);
        }

        private enum WeaponMode
        {
            Primary,
            Secondary
        }
    }
}