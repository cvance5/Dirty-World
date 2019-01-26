using Data;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors
{
    public abstract class ActorHealth : WorldObject, IHittable
    {
        public SmartEvent<IHittable, int, int> OnHit { get; set; } = new SmartEvent<IHittable, int, int>();

        public SmartEvent<ActorHealth> OnActorDamaged = new SmartEvent<ActorHealth>();
        public SmartEvent<ActorHealth> OnActorDeath = new SmartEvent<ActorHealth>();

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        protected int _maxHealthSegments = 4;
        [SerializeField]
        protected int _maxHealthPerSegment = 25;

        [SerializeField]
        protected float _damageInvulnerabilityDuration = 1f;
        public float DamageInvulnerabilityDuration => _damageInvulnerabilityDuration;
        [SerializeField]
        protected int _damageResistance = 5;
#pragma warning restore IDE0044 // Add readonly modifier

        public Health Health { get; protected set; }

        private SpriteRenderer _sprite;
        private bool _isTakingDamage = false;

        protected override void OnWorldObjectAwake()
        {
            Health = new Health(_maxHealthPerSegment, _maxHealthSegments);
            _sprite = GetComponent<SpriteRenderer>();

            OnActorAwake();
        }

        protected virtual void OnActorAwake() { }

        public void ApplyDamage(int amount)
        {
            if (amount < _damageResistance) return;
            else amount -= _damageResistance;

            if (!_isTakingDamage)
            {
                Health.Damage(amount);

                if (!Health.IsAlive) Die();
                else
                {
                    OnDamage();
                    OnActorDamaged.Raise(this);
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