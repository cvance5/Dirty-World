using UnityEngine;

namespace Actors.Player
{
    public class PlayerData : ActorData
    {
        public int GoldCollected { get; private set; }

        public void AddItem(ItemTypes item)
        {
            if (item == ItemTypes.GoldPiece) GoldCollected++;
        }

        protected override void Die()
        {
            PositionTracker.StopTracking(this);
            Log.Info($"Score: {GoldCollected}", "blue");

            Destroy(gameObject);
        }
    }
}