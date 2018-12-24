using Characters;
using Data;
using Items;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors.Player
{
    public class PlayerData : ActorData
    {
        private Character _character = null;
        private PlayerCollider _collider = null;

        public override string ObjectName => "Player";

        protected override void OnActorAwake()
        {
            PositionTracker.BeginTracking(this);

            _collider = GetComponent<PlayerCollider>();

            _collider.OnDamageTaken += ApplyDamage;
            _collider.OnHealingApplied += ApplyHealing;
            _collider.OnItemsCollected += AddCollectedItems;
        }

        public void AssignCharacter(Character character) => _character = character;

        public void AddCollectedItems(List<Item> collectedItems)
        {
            foreach (var item in collectedItems)
            {
                _character.Inventory.Add(item);
            }
        }

        public void ApplyHealing(int amount)
        {
            if (_health > 0)
            {
                _health = Mathf.Min(MaxHealth, _health + amount);
                OnHealthChanged.Raise(_health);
            }
        }

        public override void Hit(int damage, int force) => ApplyDamage(damage);

        protected override void OnDamage() { }

        protected override void OnDeath()
        {
            PositionTracker.StopTracking(this);

            _collider.OnDamageTaken -= ApplyDamage;
            _collider.OnItemsCollected -= AddCollectedItems;

            var spawnedItem = ItemLoader.CreateItem(Items.ItemActors.ItemActorTypes.HealthPack, Position);
            var chunk = GameManager.World.GetContainingChunk(Position);

            chunk.Register(spawnedItem);

            Destroy(_collider);
        }
    }
}