using System.Collections;
using UnityEngine;

namespace Actors
{
    public abstract class ActorData : MonoBehaviour, ITrackable
    {
        public SmartEvent<ActorData> OnActorDamaged = new SmartEvent<ActorData>();
        public SmartEvent<ActorData> OnActorDeath = new SmartEvent<ActorData>();

        public int Health { get; private set; } = 100;
        public IntVector2 Position => new IntVector2(transform.position);

        [SerializeField]
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        private float _damageInvulnerabilityDuration = 1f;
        public float DamageInvulnerabilityDuration => _damageInvulnerabilityDuration;
        [SerializeField]
        private int _damageResistance = 5;
#pragma warning restore IDE0044 // Add readonly modifier

        private SpriteRenderer _sprite;
        private bool _isTakingDamage = false;

        private void Awake()
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

        protected void Die()
        {
            OnActorDeath.Raise(this);
            OnDeath();
        }

        protected abstract void OnDamage();

        protected abstract void OnDeath();
    }
}