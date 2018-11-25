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

        public override HazardEffects[] Effects { get; } = new HazardEffects[] { HazardEffects.Blocking };
        public override HazardTypes Type => HazardTypes.Stalag;

        private Stack<GameObject> _stalagSegments;
        public int NumSegments => _stalagSegments.Count;

        public void Initialize(IntVector2 facingDirection, int numSegments)
        {
            transform.up = facingDirection;

            _stalagSegments = new Stack<GameObject>();
            _stalagSegments.Push(gameObject);

            for (var segmentNumber = 1; segmentNumber < numSegments; segmentNumber++)
            {
                var segment = Instantiate(_segmentObject);
                segment.transform.SetParent(transform, false);
                segment.transform.localPosition = Vector2.up * segmentNumber;
                segment.name = $"Segment {segment} for Stalag at {Position}.";
                _stalagSegments.Push(segment);
            }
        }

        private const int MAX_STALAG_HEIGHT = 3;
    }
}