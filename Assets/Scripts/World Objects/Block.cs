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

        protected int _health = 100;
        protected int _stability = 100;

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

        public void OnHit(int damage, int force)
        {
            ApplyDamage(damage);

            if (_health > 0) ApplyForce(force);
        }

        public virtual void HandleNeighborUpdate() { }

        public void ApplyDamage(int damage)
        {
            if (_health <= 0) return;

            _health -= (int)(damage * (1f - _damageResistance));

            if (_health <= 0)
            {
                Destroy();
            }
            else
            {
                _sprite.color = Color.Lerp(_baseColor, Color.black, 1 - (_health / 100f));
            }
        }

        public void ApplyForce(int force)
        {
            if (_stability <= 0) return;

            _stability -= (int)(force * (1f - _forceResistance));

            if (_stability <= 0)
            {
                Crumble();
            }
        }

        protected virtual void Destroy()
        {
            OnDestroy.Raise(this);
            Destroy(gameObject);
        }

        protected virtual void Crumble()
        {
            OnCrumble.Raise(this);

            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _velocitySamples = new Queue<float>();

            StartCoroutine(CheckForStability());
        }

        protected virtual void Stabilize()
        {
            transform.SnapToGrid();

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _stability = 100;
            _velocitySamples = null;

            OnStabilize.Raise(this);
        }
    }
}