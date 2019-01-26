using Characters;
using Data;
using Items;
using System.Collections.Generic;
using WorldObjects.Actors.Player.Guns;

namespace WorldObjects.Actors.Player
{
    public class PlayerHealth : ActorHealth
    {
        private Character _character = null;
        private PlayerCollider _collider = null;
        private Guns.ElectricalHands _hands = null;

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

        public void SetHands(ElectricalHands hands)
        {
            _hands = hands;

            _hands.OnPowerGained += FillSegment;
            _hands.OnPowerLost += EmptySegment;
        }

        public void AddCollectedItems(List<Item> collectedItems)
        {
            foreach (var item in collectedItems)
            {
                _character.Inventory.Add(item);
            }
        }

        public void ApplyHealing(int amount) => Health.Heal(amount);
        public override void Hit(int damage, int force) => ApplyDamage(damage);

        public void EmptySegment()
        {
            Health.EmptySegment();

            if (!Health.IsAlive)
            {
                Die();
            }
        }

        public void FillSegment() => Health.FillSegment();

        protected override void OnDamage() { }

        protected override void OnDeath()
        {
            PositionTracker.StopTracking(this);

            _collider.OnDamageTaken -= ApplyDamage;
            _collider.OnItemsCollected -= AddCollectedItems;

            _hands.OnPowerGained -= FillSegment;
            _hands.OnPowerLost -= EmptySegment;

            var spawnedItem = ItemLoader.CreateItem(Items.ItemActors.ItemActorTypes.HealthPack, Position);
            var chunk = GameManager.World.ChunkArchitect.GetContainingChunk(Position);

            chunk.Register(spawnedItem);

            Destroy(_collider);
        }
    }
}