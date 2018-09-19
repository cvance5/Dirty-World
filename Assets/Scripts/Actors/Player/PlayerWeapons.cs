using Actors.Player.GunActors;
using UnityEngine;

namespace Actors.Player
{
    public class PlayerWeapons : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly for Unity Serialization
        [SerializeField]
        private GunActor _primary = null;
        [SerializeField]
        private GunActor _secondary = null;
        [SerializeField]
        private PlayerData _data = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private WeaponMode _weaponMode = WeaponMode.Primary;

        private void Awake()
        {
            _data.OnActorDeath += OnPlayerDeath;
        }

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
            else if (_weaponMode == WeaponMode.Secondary)
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