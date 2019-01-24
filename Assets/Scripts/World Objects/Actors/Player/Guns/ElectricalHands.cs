using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors.Player.Guns
{
    public class ElectricalHands : Gun
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private int _damage = 15;
        [SerializeField]
        private int _force = 0;
        [SerializeField]
        private int _range = 3;
        [SerializeField]
        private float _cooldown = 1f;
        [SerializeField]
        private float _chargeTime = 5f;

        [SerializeField]
        private List<Sprite> _lightningSprites = null;

        [SerializeField]
        private float _flashDuration = .25f;
        [SerializeField]
        private AnimationCurve _flashIntensityCurve = null;
        [SerializeField]
        private SpriteRenderer _primaryLightningRenderer = null;
        [SerializeField]
        private SpriteRenderer _secondaryLightningRenderer = null;
        [SerializeField]
        private Animator _sparkAnimator = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private bool _canFire = true;

        public override void Fire()
        {
            if (_canFire)
            {
                var targetVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                targetVector.z = 0;
                targetVector = targetVector.normalized;

                if (Physics2D.Raycast(transform.position, targetVector, _filter, _hits, _range) > 0)
                {
                    var hit = _hits[0];

                    var powerable = hit.collider.GetComponent<IPowerable>();

                    if (powerable != null && !powerable.IsPowered)
                    {
                        StartCoroutine(ConnectTo(hit, powerable));
                    }
                    else
                    {
                        ShockTarget(hit);
                    }
                }
                else
                {
                    Spark();
                }
            }
        }

        private void ShockTarget(RaycastHit2D hit)
        {
            _canFire = false;

            var targetPoint = hit.point;
            StartCoroutine(FlashLightning(_primaryLightningRenderer, targetPoint));

            var hittable = hit.collider.GetComponent<IHittable>();

            if (hittable != null)
            {
                hittable.Hit(_damage, _force);
            }

            StartCooldown();
        }

        private void Spark()
        {
            _canFire = false;

            StartCoroutine(FlashSpark());

            StartCooldown();
        }

        private IEnumerator ConnectTo(RaycastHit2D hit, IPowerable powerable)
        {
            _canFire = false;

            var time = 0f;

            var numPrimary = 0;
            var numSecondary = 0;

            while (time < _chargeTime &&
                   Vector2.Distance(hit.point, transform.position) < _range &&
                   Input.GetButton("Fire"))
            {
                if (time > numPrimary * _flashDuration)
                {
                    FlashLightning(_primaryLightningRenderer, hit.point);
                    numPrimary++;
                }

                if (time > (numSecondary * _flashDuration) + (_flashDuration / 2))
                {
                    FlashLightning(_secondaryLightningRenderer, hit.point);
                    numSecondary++;
                }

                time += Time.deltaTime;

                yield return null;
            }

            if (time > _chargeTime)
            {
                powerable.AddPower();
            }

            StartCooldown();
        }

        private IEnumerator FlashLightning(SpriteRenderer renderer, Vector2 targetPoint)
        {
            renderer.sprite = _lightningSprites.RandomItem();

            var time = 0f;

            while (time < _flashDuration)
            {
                var vectorToTarget = targetPoint - (Vector2)transform.position;
                transform.up = vectorToTarget.normalized;
                renderer.size = new Vector2(renderer.size.x, vectorToTarget.magnitude);

                var lightningIntensity = _flashIntensityCurve.Evaluate(time / _flashDuration);
                renderer.color = renderer.color.SwapAlpha(lightningIntensity);

                time += Time.deltaTime;

                yield return null;
            }

            renderer.color = renderer.color.SwapAlpha(0);
        }

        private IEnumerator FlashSpark()
        {
            _sparkAnimator.SetBool("Alternator", !_sparkAnimator.GetBool("Alternator"));
            yield return null;
        }

        private void StartCooldown() => Timekeeper.SetTimer(_cooldown, () => _canFire = true);

        public override void AlternateFire() => throw new System.NotImplementedException();
    }
}