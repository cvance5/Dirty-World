using UnityEngine;

namespace Player
{
    public class PlayerData : MonoBehaviour, ITrackable
    {
        public IntVector2 GetPosition() => new IntVector2(transform.position);
    }
}