using System.Collections;
using UnityEngine;
using WorldObjects;

namespace Actors.Player.Guns
{
    public class DrillDrone : MonoBehaviour
    {
        public SmartEvent<DrillDrone> OnDroneLifespanEnded = new SmartEvent<DrillDrone>();

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private float _lifespan = 0;
        [SerializeField]
        private int _damagePerHit = 0;
        [SerializeField]
        private int _forcePerHit = 0;
        [SerializeField]
        private int _hitsPerSecond = 0;
        [SerializeField]
        private float _movespeedPerSecond = 0;

        [SerializeField]
        private float _explosionRange = 0;
        [SerializeField]
        private int _explosionDamage = 0;
        [SerializeField]
        private int _explosionForce = 0;
#pragma warning restore IDE0044 // Add readonly modifier

        private WorldObject _attachedObject = null;

        private float _lifeRemaining = 0;

        private void Awake() => _lifeRemaining = _lifespan;

        private void Update()
        {
            if (_attachedObject == null)
            {
                transform.position += (transform.right * _movespeedPerSecond * Time.deltaTime);
            }

            _lifeRemaining -= Time.deltaTime;

            if (_lifeRemaining <= 0)
            {
                Explode();
                OnDroneLifespanEnded.Raise(this);
            }
        }

        public void Explode()
        {
            var targets = Physics2D.OverlapCircleAll(transform.position, _explosionRange);

            foreach (var target in targets)
            {
                var vector = target.transform.position - transform.position;
                var force = Mathf.RoundToInt((_explosionRange - vector.magnitude) * _explosionForce);
                var damage = Mathf.RoundToInt((_explosionRange - vector.magnitude) * _explosionDamage);

                var hittable = (IHittable)target.GetComponent(typeof(IHittable));
                if (hittable != null)
                {
                    hittable.Hit(damage, force);
                }
            }

            Destroy(gameObject);
        }

        private IEnumerator TickHits()
        {
            var wfs = new WaitForSeconds(1f / _hitsPerSecond);
            var hittable = (IHittable)_attachedObject.GetComponent(typeof(IHittable));

            while (_attachedObject != null)
            {
                HitTarget(hittable);
                yield return wfs;
            }
        }

        private void HitTarget(IHittable target) => target.Hit(_damagePerHit, _forcePerHit);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_attachedObject == null)
            {
                // Check to see if we stick to the thing we hit
                var worldObject = other.GetComponent<WorldObject>();
                if (worldObject != null)
                {
                    // If we do, stick to it and see if we can hit it
                    _attachedObject = worldObject;

                    var hittable = (IHittable)other.GetComponent(typeof(IHittable));
                    if (hittable != null)
                    {
                        // If we can, start hitting
                        StartCoroutine(TickHits());
                    }
                }
                else
                {
                    // If the object can't be stuck to, see if hitting it does anything
                    var hittable = (IHittable)other.GetComponent(typeof(IHittable));
                    if (hittable != null)
                    {
                        HitTarget(hittable);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_attachedObject != null &&
                other.gameObject == _attachedObject.gameObject)
            {
                _attachedObject = null;
            }
        }
    }
}