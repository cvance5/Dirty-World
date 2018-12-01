using Characters;
using Data;
using Items;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors.Player
{
    public class PlayerData : ActorData
    {
#pragma warning disable IDE0044 // Add readonly modifier
        [Header("Actor Components")]
        [SerializeField]
        private PlayerCollider _collider = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private Character _character = null;

        public override string ObjectName => "Player";

        protected override void OnActorAwake()
        {
            PositionTracker.BeginTracking(this);

            _collider = GetComponent<PlayerCollider>();

            _collider.OnDamageTaken += ApplyDamage;
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

        public override void Hit(int damage, int force) => ApplyDamage(damage);

        protected override void OnDamage() { }

        protected override void OnDeath()
        {
            PositionTracker.StopTracking(this);

            _collider.OnDamageTaken -= ApplyDamage;
            _collider.OnItemsCollected -= AddCollectedItems;

            Destroy(_collider);
        }
    }
}