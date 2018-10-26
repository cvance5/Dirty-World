using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors.Enemies.Maggot
{
    [RequireComponent(typeof(CircleCollider2D), typeof(TriggerList2D))]
    public class MaggotBasicDeathExplosion : MaggotDeathExplosion
    {
#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        private float _explosionExpansionTime = 0;
        [SerializeField]
        private float _explosionSize = 0;

        [SerializeField]
        private int _maxDamage = 0;
        [SerializeField]
        private int _maxForce = 0;
        [SerializeField]
        private float _maxImpulse = 0;
#pragma warning restore IDE0044 // Add readonly modifier

        private TriggerList2D _triggerList;

        private readonly List<GameObject> _hitTargets = new List<GameObject>();

        private void Awake()
        {
            _triggerList = GetComponent<TriggerList2D>();
            transform.localScale = Vector2.zero;
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            var timePassed = 0f;

            while (timePassed < _explosionExpansionTime)
            {
                timePassed += Time.deltaTime;

                var percentComplete = Mathf.Clamp01(timePassed / _explosionExpansionTime);
                var percentEffect = 1 - percentComplete;

                transform.localScale = Vector2.one * _explosionSize * percentComplete;

                var possibleTargets = _triggerList.Overlaps;

                foreach (var possibleTarget in possibleTargets)
                {
                    if (_hitTargets.Contains(possibleTarget)) continue;

                    var hittable = (IHittable)possibleTarget.GetComponent(typeof(IHittable));
                    if (hittable != null)
                    {
                        hittable.Hit(Mathf.RoundToInt(_maxDamage * percentEffect), Mathf.RoundToInt(_maxForce * percentEffect));
                    }

                    var impulsable = (IImpulsable)possibleTarget.GetComponent(typeof(IImpulsable));
                    if (impulsable != null)
                    {
                        var impulseVector = possibleTarget.transform.position - transform.position;
                        impulsable.Impulse(impulseVector * percentEffect * _maxImpulse);
                    }

                    _hitTargets.Add(possibleTarget);
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}