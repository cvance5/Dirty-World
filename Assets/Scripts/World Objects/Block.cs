using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects
{
    public abstract class Block : MonoBehaviour, IHittable
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

        public IntVector2 Position => new IntVector2(transform.position);

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
            var waiter = new WaitForFixedUpdate();

            while (true)
            {
                _velocitySamples.Enqueue(_rigidbody.velocity.magnitude);
                if (_velocitySamples.Count > 10)
                {
                    _velocitySamples.Dequeue();
                    if (MathUtils.Average(_velocitySamples) < _restabilizationThreshold)
                    {
                        Stabilize();
                        break;
                    }
                }

                yield return waiter;
            }
        }

        public void Hit(int damage, int force)
        {
            ApplyDamage(damage);

            if (Health > 0) ApplyForce(force);
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
    }
}