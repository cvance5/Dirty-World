using UnityEngine;

namespace WorldObjects.Actors
{
    public abstract class ActorData : WorldObject, IHittable
    {
        public SmartEvent<IHittable, int, int> OnHit { get; set; } = new SmartEvent<IHittable, int, int>();

        public SmartEvent<ActorData> OnActorDamaged = new SmartEvent<ActorData>();
        public SmartEvent<ActorData> OnActorDeath = new SmartEvent<ActorData>();

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private int Health = 100;

        [SerializeField]
        private float _damageInvulnerabilityDuration = 1f;
        public float DamageInvulnerabilityDuration => _damageInvulnerabilityDuration;
        [SerializeField]
        private int _damageResistance = 5;
#pragma warning restore IDE0044 // Add readonly modifier

        private SpriteRenderer _sprite;
        private readonly bool _isTakingDamage = false;

        protected override void OnWorldObjectAwake()
        {
            _sprite = GetComponent<SpriteRenderer>();

            OnActorAwake();
        }

        protected virtual void OnActorAwake() { }

        public void ApplyDamage(int amount)
        {
            // We don't desecrate corpses, here.
            if (Health <= 0) return;

            if (amount < _damageResistance) return;
            else amount -= _damageResistance;

            if (!_isTakingDamage)
            {
                Health -= amount;

                if (Health <= 0) Die();
                else
                {
                    OnDamage();
                    OnActorDamaged.Raise(this);
                }
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