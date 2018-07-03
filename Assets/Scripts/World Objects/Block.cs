using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects
{
    public abstract class Block : MonoBehaviour, IHittable
    {
        public SmartEvent<Block> OnCrumble = new SmartEvent<Block>();
        public SmartEvent<Block> OnDestroy = new SmartEvent<Block>();
        public SmartEvent<Block> OnStabilize = new SmartEvent<Block>();

        public IntVector2 Position => new IntVector2(transform.position);

        public int Health { get; private set; } = 100;
        public int Stability { get; private set; } = 100;
        public float RestabilizationThreshhold { get; private set; } = .01f;

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
                    if (MathUtils.Average(_velocitySamples) < RestabilizationThreshhold)
                    {
                        Stabilize();
                        break;
                    }
                }

                yield return waiter;
            }
        }

        public void OnHit(int damage, int force)
        {
            ApplyDamage(damage);
            ApplyForce(force);
        }

        public void ApplyDamage(int damage)
        {
            if (Health <= 0) return;

            Health -= damage;

            if (Health <= 0)
            {
                OnDestroy.Raise(this);
                Destroy(gameObject);
            }
            else
            {
                _sprite.color = Color.Lerp(_baseColor, Color.black, 1 - (Health / 100f));
            }
        }

        public void ApplyForce(int force)
        {
            if (Stability <= 0) return;

            Stability -= force;

            if (Stability <= 0)
            {
                Crumble();
            }
        }

        private void Crumble()
        {
            OnCrumble.Raise(this);

            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _velocitySamples = new Queue<float>();

            StartCoroutine(CheckForStability());
        }

        private void Stabilize()
        {
            transform.SnapToGrid();

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Stability = 100;
            _velocitySamples = null;

            OnStabilize.Raise(this);
        }
    }
}