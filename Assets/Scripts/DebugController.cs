using Actors.Player;
using UnityEngine;

public class DebugController : Singleton<DebugController>
{
#if UNITY_EDITOR

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            FindObjectOfType<PlayerData>().ApplyDamage(10000);
        }
    }
#endif
}
