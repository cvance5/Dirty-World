using Actors.Player.Guns;
using UnityEngine;

namespace Actors.Player
{
    public class PlayerWeapons : MonoBehaviour
    {
        [SerializeField]
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        private float _climbSpeed = 0;
#pragma warning restore IDE0044 // Add readonly modifier

        [SerializeField]
        private HarpoonGun _harpoonGun = null;

        private void Update()
        {
            if (Input.GetButtonDown("Alternate Fire"))
            {
                _harpoonGun.AlternateFire();
            }
            else if (Input.GetButtonDown("Fire"))
            {
                _harpoonGun.Fire();
            }

            if(_harpoonGun.IsAttached)
            {
                var ropeLengthChange = Input.GetAxis("Vertical");

                if(ropeLengthChange != 0)
                {
                    _harpoonGun.ChangeLength(ropeLengthChange * _climbSpeed * Time.deltaTime);
                }                
            }            
        }
    }
}