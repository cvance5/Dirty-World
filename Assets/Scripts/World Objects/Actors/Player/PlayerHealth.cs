using Characters;
using Data;
using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Actors.Player.Guns;

namespace WorldObjects.Actors.Player
{
    public class PlayerHealth : ActorHealth
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly for Unity Serialization
        [SerializeField]
        private float _healthRegenerationDelay = 1f;
        [SerializeField]
        private float _healthRegenerationTick = .2f;
        [SerializeField]
        private int _healthRenerationAmount = 1;
#pragma warning restore IDE0044 // Add readonly modifier

        private Character _character = null;
        private PlayerCollider _collider = null;
        private ElectricalHands _hands = null;

        private Coroutine _regenerationCoroutine = null;

        public override string ObjectName => "Player";

        protected override void OnActorAwake()
        {
            PositionTracker.BeginTracking(this);

            _collider = GetComponent<PlayerCollider>();

            _collider.OnDamageTaken += ApplyDamage;
            _collider.OnHealingApplied += ApplyHealing;
            _collider.OnItemsCollected += AddCollectedItems;

            Health.OnHealthChanged += CheckForRegeneration;
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

        public bool TryEmptySegment()
        {
            if (Health.SegmentsAvailable > 1)
            {
                EmptySegment();
                return true;
            }
            else return false;
        }

        public void EmptySegment()
        {
            Health.EmptySegment();

            if (!Health.IsAlive)
            {
                Die();
            }
        }

        public void FillSegment() => Health.FillSegment();

        private void CheckForRegeneration(int delta)
        {
            if (delta < 0)
            {
                if (_regenerationCoroutine != null)
                {
                    StopCoroutine(_regenerationCoroutine);
                    _regenerationCoroutine = null;
                }

                _regenerationCoroutine = StartCoroutine(DelayRegenerationCoroutine());
            }
        }

        private IEnumerator DelayRegenerationCoroutine()
        {
            yield return new WaitForSeconds(_healthRegenerationDelay);

            _regenerationCoroutine = StartCoroutine(RegenerationCoroutine());
        }

        private IEnumerator RegenerationCoroutine()
        {
            var wfs = new WaitForSeconds(_healthRegenerationTick);

            while (!Health.IsSegmentFull)
            {
                Health.Heal(_healthRenerationAmount, false);
                yield return wfs;
            }

            _regenerationCoroutine = null;
        }

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

            if (_regenerationCoroutine != null)
            {
                StopCoroutine(_regenerationCoroutine);
                _regenerationCoroutine = null;
            }

            Destroy(_collider);
        }
    }
}