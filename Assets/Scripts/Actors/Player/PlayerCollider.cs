using ItemManagement;
using Metadata;
using UnityEngine;
using WorldObjects.Hazards;

namespace Actors.Player
{
    public class PlayerCollider : MonoBehaviour
    {
        [SerializeField]
        private PlayerData _data = null;

        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _data.OnActorDeath += OnPlayerDeath;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var tag = collision.gameObject.tag;

            switch (tag)
            {
                case Tags.Item: HandleItem(collision.gameObject.GetComponent<ItemActor>()); break;
                case Tags.Hazard: HandleHazard(collision.gameObject.GetComponent<Hazard>()); break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tag = other.tag;

            switch (tag)
            {
                case Tags.Item: HandleItem(other.GetComponent<ItemActor>()); break;
                case Tags.Hazard: HandleHazard(other.GetComponent<Hazard>()); break;
            }
        }

        private void HandleItem(ItemActor item)
        {
            _log.ErrorIfNull(item, $"{item} has tag {Tags.Item} but does not have an item component.");

            foreach (var interaction in item.Interactions)
            {
                switch (interaction)
                {
                    case InteractionTypes.Collect:
                        var collectibleItem = item as ICollectible;
                        _log.ErrorIfNull(collectibleItem, $"{item} has interaction {interaction} but does not implement {typeof(ICollectible).Name}.");
                        _data.AddCollectedItems(collectibleItem.CollectedItems);
                        collectibleItem.OnCollect();
                        break;
                    case InteractionTypes.Damage:
                        break;
                    default: _log.Error($"Unknown interaction '{interaction}'."); break;
                }
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
                        _rigidbody.velocity = knockbackHazard.GetImpulse(_rigidbody.velocity);
                        break;
                    default: _log.Error($"Unknown effect '{effect}'."); break;
                }
            }
        }

        private void OnPlayerDeath(ActorData playerData)
        {
            playerData.OnActorDeath -= OnPlayerDeath;
            Destroy(this);
        }

        private static readonly Log _log = new Log("PlayerCollider");
    }
}