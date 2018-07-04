using UnityEngine;

namespace Player
{
    public class PlayerData : MonoBehaviour, ITrackable
    {
        public int Health { get; private set; } = 100;

        public IntVector2 GetPosition() => new IntVector2(transform.position);
        public uint GoldCollected { get; private set; }

        public void ApplyDamage(int amount)
        {
            Health -= amount;

            if (Health <= 0) Die();
        }

        public void AddItem(ItemTypes item)
        {
            if (item == ItemTypes.GoldPiece) GoldCollected++;
        }

        private void Die()
        {
            PositionTracker.StopTracking(this);
            Log.Info($"Score: {GoldCollected}", "blue");

            Destroy(gameObject);
        }
    }
}