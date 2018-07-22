﻿using Actors;
using Metadata;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Block : WorldObject, IHittable, IImpulsable
    {
        public SmartEvent<Block> OnBlockCrumbled = new SmartEvent<Block>();
        public SmartEvent<Block> OnBlockDestroyed = new SmartEvent<Block>();
        public SmartEvent<Block> OnBlockStabilized = new SmartEvent<Block>();

        public int Health { get; protected set; } = 100;
        public int Stability { get; protected set; } = 100;

        public bool CanStabalize { get; set; }

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
        public bool IsStable => _rigidbody.bodyType == RigidbodyType2D.Kinematic;

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

        public void Impulse(Vector2 impulse)
        {
            if (IsStable)
            {
                int impactMagniture = (int)impulse.magnitude;
                if (impactMagniture > _impactDurability)
                {
                    int remainder = impactMagniture - _impactDurability;
                    Hit(remainder, remainder);
                }
            }
            else _rigidbody.AddForce(impulse);
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

            OnBlockCrumbled.Raise(this);
            AlertNeighbors();
        }

        protected virtual IEnumerator Stabilize()
        {
            yield return null;

            transform.SnapToGrid();

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Stability = 100;
            _velocitySamples = null;

            OnBlockStabilized.Raise(this);
            AlertNeighbors();
        }

        protected virtual IEnumerator Destroy()
        {
            yield return null;

            Destroy(gameObject);

            OnBlockDestroyed.Raise(this);

            if (IsStable)
            {
                AlertNeighbors();
            }
        }

        private IEnumerator CheckForStability()
        {
            var waitForFixedUpdate = new WaitForFixedUpdate();

            while (true)
            {
                if (CanStabalize)
                {
                    _velocitySamples.Enqueue(_rigidbody.velocity.magnitude);
                    if (_velocitySamples.Count > 10)
                    {
                        _velocitySamples.Dequeue();

                        if (MathUtils.Average(_velocitySamples) < _restabilizationThreshold &&
                                 IntVector2.Distance(Position, transform.position) < .02f)
                        {
                            StartCoroutine(Stabilize());
                            break;
                        }
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
            if (!IsStable)
            {
                var other = collision.collider.gameObject;
                var impactVelocity = (collision.relativeVelocity - collision.rigidbody.velocity);

                var impactMagnitude = Mathf.RoundToInt(impactVelocity.magnitude);
                int unabsorbedImpactMagnitude = impactMagnitude - _impactAbsorption;

                switch (other.tag)
                {
                    case Tags.Hazard:
                        HandleHazard(other.GetComponent<Hazard>());
                        break;
                    case Tags.Enemy:
                    case Tags.Player:
                        HandleActor(other.GetComponent<ActorData>(), unabsorbedImpactMagnitude);
                        break;
                }

                if (unabsorbedImpactMagnitude > 0)
                {
                    var unabsorbedImpact = collision.relativeVelocity.normalized * unabsorbedImpactMagnitude;

                    Impulse(unabsorbedImpact);

                    var otherImpulsable = other.GetComponent<IImpulsable>();
                    if (otherImpulsable != null)
                    {
                        otherImpulsable.Impulse(unabsorbedImpact * _weight);
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
                        ApplyDamage(damagingHazard.Damage);
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

        protected override void OnDestroyed()
        {
            DropItem();
        }

        protected static readonly Log _log = new Log("Block");
    }
}