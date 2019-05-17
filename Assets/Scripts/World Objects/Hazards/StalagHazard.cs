using MathConcepts;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Hazards
{
    public class StalagHazard : Hazard
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private GameObject _segmentObject = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public override string ObjectName => $"Stalag {Position}";

        public override HazardEffects[] Effects { get; } = new HazardEffects[] { };
        public override HazardTypes Type => HazardTypes.Stalag;

        private Stack<StalagSegment> _stalagSegments;
        public int NumSegments => _stalagSegments.Count;

        public void Initialize(IntVector2 facingDirection, int numSegments)
        {
            transform.up = facingDirection;

            _stalagSegments = new Stack<StalagSegment>();

            InitializeSegment(GetComponent<StalagSegment>(), numSegments);

            for (var segmentNumber = 1; segmentNumber < numSegments; segmentNumber++)
            {
                var segmentObject = Instantiate(_segmentObject);
                segmentObject.transform.SetParent(transform, false);
                segmentObject.transform.localPosition = Vector2.up * segmentNumber;
                segmentObject.name = $"Segment {segmentObject} for Stalag at {Position}.";

                InitializeSegment(segmentObject.GetComponent<StalagSegment>(), numSegments - segmentNumber);
            }
        }

        private void InitializeSegment(StalagSegment segment, int numberFromTip)
        {
            segment.OnHit += OnSegmentHit;
            segment.Initialize(HEALTH_PER_STALAG_SEGMENT * numberFromTip, STABILITY_PER_STALAG_SEGMENT * numberFromTip);
            _stalagSegments.Push(segment);
        }

        public void OnSegmentHit(IHittable hittableSegment, int damage, int force)
        {
            // These two segments may the same.  Do we care?
            var segment = hittableSegment as StalagSegment;

            if (!ApplyHitToSegment(segment, damage / 2, force / 2))
            {
                CascadeDamage(damage / 2, force / 2);
            }
        }

        private void CascadeDamage(int remainingDamage, int remainingForce)
        {
            while ((remainingDamage > 0 || remainingForce > 0) && _stalagSegments.Count > 0)
            {
                var tipSegment = _stalagSegments.Peek();

                var damageToApply = Mathf.Min(tipSegment.Health, remainingDamage);
                var forceToApply = Mathf.Min(tipSegment.Stability, remainingForce);

                remainingDamage -= damageToApply;
                remainingForce -= forceToApply;

                ApplyHitToSegment(tipSegment, damageToApply, forceToApply);
            }
        }

        private bool ApplyHitToSegment(StalagSegment segment, int damage, int force)
        {
            var isSegmentGone = false;

            segment.Health -= damage;
            segment.Stability -= force;

            if (segment.Health <= 0)
            {
                DestroySegment(segment);
                isSegmentGone = true;
            }
            else if (segment.Stability <= 0)
            {
                DropSegment(segment);
                isSegmentGone = true;
            }

            return isSegmentGone;
        }

        private void DestroySegment(StalagSegment segment)
        {
            while (_stalagSegments.Peek() != segment)
            {
                DropSegment(_stalagSegments.Peek());
            }

            _stalagSegments.Pop();
            Destroy(segment.gameObject);

            RemoveSegment(segment);
        }

        private void DropSegment(StalagSegment segment)
        {
            while (_stalagSegments.Peek() != segment)
            {
                DropSegment(_stalagSegments.Peek());
            }

            _stalagSegments.Pop();
            segment.Crumble();

            RemoveSegment(segment);
        }

        private void RemoveSegment(StalagSegment segment)
        {
            segment.OnHit -= OnSegmentHit;

            if (_stalagSegments.Count == 0)
            {
                OnHazardDestroyed.Raise(this);
                Destroy(this);
            }
            else
            {
                OnHazardChanged.Raise(this);
            }
        }

        private const int MAX_STALAG_HEIGHT = 3;
        private const int HEALTH_PER_STALAG_SEGMENT = 25;
        private const int STABILITY_PER_STALAG_SEGMENT = 12;
    }
}