using Actors.Player.Guns;
using UnityEngine;

namespace Actors.Player
{
    public class PlayerWeapons : MonoBehaviour
    {
        [SerializeField]
        private Gun _primary = null;

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
    }
}