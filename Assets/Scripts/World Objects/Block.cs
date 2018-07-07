using Metadata;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects
{
    public abstract class Block : WorldObject, IHittable
    {
        public SmartEvent<Block> OnCrumbled = new SmartEvent<Block>();
        public SmartEvent<Block> OnDestroyed = new SmartEvent<Block>();
        public SmartEvent<Block> OnStabilized = new SmartEvent<Block>();

        public int Health { get; protected set; } = 100;
        public int Stability { get; protected set; } = 100;

        [SerializeField]
        [Range(0, 1)]
        protected float _damageResistance = 0f;
        [SerializeField]
        [Range(0, 1)]
        protected float _forceResistance = 0f;
        [SerializeField]
        protected float _restabilizationThreshold = .01f;

        [SerializeField]
        [Tooltip("The force removed from collisions initiated by this object.")]
        protected int _impactAbsorption = 0;
        [SerializeField]
        [Tooltip("The force this object ignores before calculating impact damage.")]
        protected int _impactDurability = 0;


        private SpriteRenderer _sprite;
        private Color _baseColor;

        private Rigidbody2D _rigidbody;
        private Queue<float> _velocitySamples;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _baseColor = _sprite.color;
        }

        private IEnumerator CheckForStability()
        {
            var waitForFixedUpdate = new WaitForFixedUpdate();

            while (true)
            {
                _velocitySamples.Enqueue(_rigidbody.velocity.magnitude);
                if (_velocitySamples.Count > 10)
                {
                    _velocitySamples.Dequeue();

                    if (MathUtils.Average(_velocitySamples) < _restabilizationThreshold &&
                        IntVector2.Distance(Position, transform.position) < .02f)
                    {
                        Stabilize();
                        break;
                    }
                }

                yield return waitForFixedUpdate;
            }
        }

        public void Hit(int damage, int force)
        {
            ApplyDamage(damage);

            if (Health > 0) ApplyForce(force);
        }

        public void Impact(int impactMagniture)
        {
            if (impactMagniture > _impactDurability)
            {
                var remainder = impactMagniture - _impactDurability;
                Hit(remainder, remainder);
            }
        }

        public virtual void HandleNeighborUpdate() { }

        public void ApplyDamage(int damage)
        {
            if (Health <= 0) return;

            Health -= (int)(damage * (1f - _damageResistance));

            if (Health <= 0)
            {
                Destroy();
            }
            else
            {
                _sprite.color = Color.Lerp(_baseColor, Color.black, 1 - (Health / 100f));
            }
        }

        public void ApplyForce(int force)
        {
            if (Stability <= 0) return;

            Stability -= (int)(force * (1f - _forceResistance));

            if (Stability <= 0)
            {
                Crumble();
            }
        }

        protected virtual void Crumble()
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _velocitySamples = new Queue<float>();

            StartCoroutine(CheckForStability());

            OnCrumbled.Raise(this);
            AlertNeighbors();
        }

        protected virtual void Stabilize()
        {
            transform.SnapToGrid();

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Stability = 100;
            _velocitySamples = null;

            OnStabilized.Raise(this);
            AlertNeighbors();
        }

        protected virtual void Destroy()
        {
            Destroy(gameObject);

            OnDestroyed.Raise(this);
            AlertNeighbors();
        }

        private void AlertNeighbors()
        {
            var neighbors = World.GetNeighbors(this);

            foreach (var neighbor in neighbors)
            {
                neighbor.HandleNeighborUpdate();
            }
        }

        protected virtual void DropItem() { }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_rigidbody.bodyType == RigidbodyType2D.Dynamic)
            {
                var otherObject = collision.collider.gameObject;

                var impact = Mathf.RoundToInt(collision.relativeVelocity.magnitude);
                int unabsorbedImpact = impact - _impactAbsorption;

                switch (otherObject.tag)
                {
                    case Tags.Hazard: HandleHazard(otherObject.GetComponent<Hazard>()); break;
                    case Tags.Player: HandlePlayerCollision(otherObject.GetComponent<PlayerData>(), unabsorbedImpact); break;
                }

                if (unabsorbedImpact > 0)
                {
                    Impact(unabsorbedImpact);

                    var otherHittable = otherObject.GetComponent(typeof(IHittable)) as IHittable;

                    if (otherHittable != null)
                    {
                        otherHittable.Impact(unabsorbedImpact);
                    }
                }
            }
        }

        private void HandleHazard(Hazard hazard)
        {
            Log.ErrorIfNull(hazard, $"{hazard} has tag {Tags.Hazard} but does not have an item component.");

            foreach (var effect in hazard.Effects)
            {
                switch (effect)
                {
                    case HazardEffects.Damage:
                        var damagingHazard = hazard as IDamaging;
                        Log.ErrorIfNull(damagingHazard, $"{hazard} has effect {effect} but does not implement {typeof(IDamaging).Name}.");
                        ApplyDamage(damagingHazard.GetDamage());
                        break;
                    case HazardEffects.Impulse:
                        var knockbackHazard = hazard as IImpulsive;
                        Log.ErrorIfNull(knockbackHazard, $"{hazard} has effect {effect} but does not implement {typeof(IImpulsive).Name}.");
                        _rigidbody.velocity = knockbackHazard.GetImpulse(_rigidbody.velocity);
                        break;
                    default: Log.Error($"Unknown effect '{effect}'."); break;
                }
            }
        }

        protected void HandlePlayerCollision(PlayerData playerData, int impact)
        {
            playerData.ApplyDamage(impact);
        }

        private void OnDestroy()
        {
            if (!_isQuitting)
            {
                DropItem();
            }
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private static bool _isQuitting = false;
    }
}