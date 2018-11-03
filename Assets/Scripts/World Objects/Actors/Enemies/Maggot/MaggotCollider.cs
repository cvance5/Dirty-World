using Metadata;
using UnityEngine;
using WorldObjects.Hazards;

namespace WorldObjects.Actors.Enemies.Maggot
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class MaggotCollider : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        private MaggotData _data = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private Rigidbody2D _rigidbody;

        private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var tag = collision.gameObject.tag;

            switch (tag)
            {
                case Tags.Hazard: HandleHazard(collision.gameObject.GetComponent<Hazard>()); break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tag = other.tag;

            switch (tag)
            {
                case Tags.Hazard: HandleHazard(other.GetComponent<Hazard>()); break;
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
                        _data.ApplyDamage(damagingHazard.Damage);
                        break;
                    case HazardEffects.Impulse:
                        var knockbackHazard = hazard as IImpulsive;
                        _log.ErrorIfNull(knockbackHazard, $"{hazard} has effect {effect} but does not implement {typeof(IImpulsive).Name}.");
                        _rigidbody.velocity = knockbackHazard.GetImpulse(transform.position, _rigidbody.velocity);
                        break;
                    default: _log.Error($"Unknown effect '{effect}'."); break;
                }
            }
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("MaggotCollider");
    }
}