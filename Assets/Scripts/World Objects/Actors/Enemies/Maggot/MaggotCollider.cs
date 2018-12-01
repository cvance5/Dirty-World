using Metadata;
using UnityEngine;
using WorldObjects.Hazards;

namespace WorldObjects.Actors.Enemies.Maggot
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class MaggotCollider : ActorCollider
    {
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

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("MaggotCollider");
    }
}