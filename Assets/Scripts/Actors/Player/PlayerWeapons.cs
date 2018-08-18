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
        private PlayerData _data = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private void Awake()
        {
            _data.OnActorDeath += OnPlayerDeath;
        }

        private void Update()
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

        private void OnPlayerDeath(ActorData playerData)
        {
            playerData.OnActorDeath -= OnPlayerDeath;
            Destroy(this);
        }
    }
}