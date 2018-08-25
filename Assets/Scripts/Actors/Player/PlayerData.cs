using Data;
using ItemManagement;
using System.Collections.Generic;

namespace Actors.Player
{
    public class PlayerData : ActorData
    {
        public int Wealth { get; private set; }

        private void Awake()
        {
            PositionTracker.BeginTracking(this);
        }

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

        protected override void OnDamage() { }

        protected override void OnDeath()
        {
            _log.Info($"Score: {Wealth}", "blue");
            PositionTracker.StopTracking(this);
        }

        private static readonly Log _log = new Log("PlayerData");
    }
}