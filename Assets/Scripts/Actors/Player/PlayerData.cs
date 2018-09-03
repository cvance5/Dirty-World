using Data;
using ItemManagement;
using Metadata;
using System.Collections.Generic;

namespace Actors.Player
{
    public class PlayerData : ActorData
    {
        private User _owner = null;

        private void Awake()
        {
            PositionTracker.BeginTracking(this);
        }

        public void AssignToUser(User user)
        {
            _owner = user;
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
            PositionTracker.StopTracking(this);
        }

        private static readonly Log _log = new Log("PlayerData");
    }
}