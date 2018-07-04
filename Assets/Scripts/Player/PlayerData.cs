using UnityEngine;

namespace Player
{
    public class PlayerData : MonoBehaviour, ITrackable
    {
        public IntVector2 GetPosition() => new IntVector2(transform.position);
        public uint GoldCollected { get; private set; }

        public void AddItem(ItemTypes item)
        {
            if (item == ItemTypes.GoldPiece) GoldCollected++;
        }
    }
}