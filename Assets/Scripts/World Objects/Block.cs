using Actors;
using Metadata;
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
        [Tooltip("Percent of damage ignored when this block is hit.")]
        protected float _damageResistance = 0f;
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Percent of force ignored when this block is hit.")]
        protected float _forceResistance = 0f;
        [SerializeField]
        [Tooltip("Velocity magnitude limit under which this block restabilizes.")]
        protected float _restabilizationThreshold = .01f;

        [SerializeField]
        [Tooltip("The force removed from collisions initiated by this object.")]
        protected int _impactAbsorption = 0;
        [SerializeField]
        [Tooltip("The force this object ignores before calculating impact damage.")]
        protected int _impactDurability = 0;

        [SerializeField]
        [Tooltip("Multiplier to impact force to simulate denser materials.")]
        protected float _weight = 1f;

        private SpriteRenderer _sprite;
        private Color _baseColor;

        private Rigidbody2D _rigidbody;
        private Queue<float> _velocitySamples;
        protected bool _isStable => _rigidbody.bodyType == RigidbodyType2D.Kinematic;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _baseColor = _sprite.color;
        }

        public void Hit(int damage, int force)
        {
            ApplyDamage(damage);

            if (Health > 0) ApplyForce(force);
        }

        public void Impact(Vector2 impact)
        {
            int impactMagniture = (int)impact.magnitude;
            if (impactMagniture > _impactDurability)
            {
                int remainder = impactMagniture - _impactDurability;
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
                StartCoroutine(Destroy());
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
                StartCoroutine(Crumble());
            }
        }

        protected virtual IEnumerator Crumble()
        {
            yield return null;

            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _velocitySamples = new Queue<float>();

            StartCoroutine(CheckForStability());

            OnCrumbled.Raise(this);
            AlertNeighbors();
        }

        protected virtual IEnumerator Stabilize()
        {
            yield return null;

            transform.SnapToGrid();

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Stability = 100;
            _velocitySamples = null;

            OnStabilized.Raise(this);
            AlertNeighbors();
        }

        protected virtual IEnumerator Destroy()
        {
            yield return null;

            Destroy(gameObject);

            OnDestroyed.Raise(this);

            if (_isStable)
            {
                AlertNeighbors();
            }
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
                        IntVector2.Distance(GetPosition(), transform.position) < .02f)
                    {
                        StartCoroutine(Stabilize());
                        break;
                    }
                }

                yield return waitForFixedUpdate;
            }
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
            if (!_isStable)
            {
                var otherObject = collision.collider.gameObject;
                var impactVelocity = (collision.relativeVelocity - collision.rigidbody.velocity);

                var impactMagnitude = Mathf.RoundToInt(impactVelocity.magnitude);
                int unabsorbedImpactMagnitude = impactMagnitude - _impactAbsorption;

                switch (otherObject.tag)
                {
                    case Tags.Hazard:
                        HandleHazard(otherObject.GetComponent<Hazard>());
                        break;
                    case Tags.Enemy:
                    case Tags.Player:
                        HandleActor(otherObject.GetComponent<ActorData>(), unabsorbedImpactMagnitude);
                        break;
                }

                if (unabsorbedImpactMagnitude > 0)
                {
                    var unabsorbedImpact = collision.relativeVelocity.normalized * unabsorbedImpactMagnitude;

                    Impact(unabsorbedImpact);

                    var otherHittable = otherObject.GetComponent(typeof(IHittable)) as IHittable;

                    if (otherHittable != null)
                    {
                        otherHittable.Impact(unabsorbedImpact * _weight);
                    }
                }
            }
        }

        private void HandleHazard(Hazard hazard)
        {
            _log.ErrorIfNull(hazard, $"{hazard} has tag {Tags.Hazard} but does not have an item component.");

            foreach (var effect in hazard.Effects)
            {
                switch (effect)
                {
                    case HazardEffects.Damage:
                        var damagingHazard = hazard as IDamaging;
                        _log.ErrorIfNull(damagingHazard, $"{hazard} has effect {effect} but does not implement {typeof(IDamaging).Name}.");
                        ApplyDamage(damagingHazard.GetDamage());
                        break;
                    case HazardEffects.Impulse:
                        var knockbackHazard = hazard as IImpulsive;
                        _log.ErrorIfNull(knockbackHazard, $"{hazard} has effect {effect} but does not implement {typeof(IImpulsive).Name}.");
                        _rigidbody.velocity = knockbackHazard.GetImpulse(_rigidbody.velocity);
                        break;
                    default: _log.Error($"Unknown effect '{effect}'."); break;
                }
            }
        }

        protected void HandleActor(ActorData actorData, int impactMagnitude)
        {
            actorData.ApplyDamage(impactMagnitude);
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

        protected static readonly Log _log = new Log("Block");
    }
}