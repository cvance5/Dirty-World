using UnityEngine;
using WorldObjects.Actors.Player;

namespace Utilities.Debug
{
    public class DebugController : Singleton<DebugController>
    {
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                FindObjectOfType<PlayerHealth>().ApplyDamage(10000);
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                FindObjectOfType<PlayerHealth>().ApplyDamage(30);
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                FindObjectOfType<PlayerHealth>().ApplyHealing(30);
            }
        }
#endif
    }
}