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
                FindObjectOfType<PlayerData>().ApplyDamage(10000);
            }
        }
#endif
    }
}