using Data;
using UnityEngine;

namespace WorldObjects.Actors
{
    public abstract class ActorData : WorldObject, IHittable
    {
        public SmartEvent<IHittable, int, int> OnHit { get; set; } = new SmartEvent<IHittable, int, int>();

        public SmartEvent<ActorData> OnActorDamaged = new SmartEvent<ActorData>();
        public SmartEvent<ActorData> OnActorDeath = new SmartEvent<ActorData>();

        public SmartEvent<int> OnHealthChanged = new SmartEvent<int>();

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        protected int _maxHealth = 100;
        public int MaxHealth => _maxHealth;

        [SerializeField]
        protected float _damageInvulnerabilityDuration = 1f;
        public float DamageInvulnerabilityDuration => _damageInvulnerabilityDuration;
        [SerializeField]
        protected int _damageResistance = 5;
#pragma warning restore IDE0044 // Add readonly modifier

        protected int _health;

        private SpriteRenderer _sprite;
        private bool _isTakingDamage = false;

        protected override void OnWorldObjectAwake()
        {
            _health = MaxHealth;

            _sprite = GetComponent<SpriteRenderer>();

            OnActorAwake();
        }

        protected virtual void OnActorAwake() { }

        public void ApplyDamage(int amount)
        {
            // We don't desecrate corpses, here.
            if (_health <= 0) return;

            if (amount < _damageResistance) return;
            else amount -= _damageResistance;

            if (!_isTakingDamage)
            {
                _health -= amount;

                if (_health <= 0) Die();
                else
                {
                    OnDamage();
                    OnActorDamaged.Raise(this);

                    OnHealthChanged.Raise(_health);
                }

                _isTakingDamage = true;
                Timekeeper.SetTimer(_damageInvulnerabilityDuration, () => _isTakingDamage = false);
            }
        }

        public abstract void Hit(int damage, int force);

        protected void Die()
        {
            OnActorDeath.Raise(this);
            OnDeath();
        }

        protected abstract void OnDamage();
        protected abstract void OnDeath();
    }
}