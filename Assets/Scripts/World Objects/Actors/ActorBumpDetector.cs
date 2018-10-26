using Metadata;
using UnityEngine;
using WorldObjects;

namespace WorldObjects.Actors
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class ActorBumpDetector : MonoBehaviour
    {
        public SmartEvent<WorldObject> OnWorldObjectBumped = new SmartEvent<WorldObject>();

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer(Layers.WorldObject))
            {
                var worldObject = collider.GetComponent<WorldObject>();

                if (worldObject)
                {
                    OnWorldObjectBumped.Raise(worldObject);
                }
                else
                {
                    _log.Warning($"{worldObject.name} is on the World Object layer but is not a World Object.");
                }
            }
        }

        private static readonly Log _log = new Log("ActorBumpDetector");
    }
}