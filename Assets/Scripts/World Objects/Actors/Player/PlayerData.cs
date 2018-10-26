using Characters;
using Data;
using Items;
using System.Collections.Generic;

namespace WorldObjects.Actors.Player
{
    public class PlayerData : ActorData
    {
        private Character _character = null;

        public override string ObjectName => "Player";

        protected override void OnActorAwake()
        {
            PositionTracker.BeginTracking(this);
        }

        public void AssignCharacter(Character character)
        {
            _character = character;
        }

        public void AddCollectedItems(List<Item> collectedItems)
        {
            foreach (var item in collectedItems)
            {
                _character.Inventory.Add(item);
            }
        }

        protected override void OnDamage() { }

        protected override void OnDeath()
        {
            PositionTracker.StopTracking(this);
        }
    }
}