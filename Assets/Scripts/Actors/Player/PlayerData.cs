using ItemManagement;
using System.Collections.Generic;

namespace Actors.Player
{
    public class PlayerData : ActorData
    {
        public int Wealth { get; private set; }

        public void AddCollectedItems(Dictionary<CollectibleType, int> collectedItems)
        {
            foreach (var kvp in collectedItems)
            {
                switch (kvp.Key)
                {
                    case CollectibleType.Wealth: Wealth += kvp.Value; break;
                }
            }
        }

        protected override void Die()
        {
            PositionTracker.StopTracking(this);
            _log.Info($"Score: {Wealth}", "blue");

            Destroy(gameObject);
        }

        private static readonly Log _log = new Log("PlayerData");
    }
}