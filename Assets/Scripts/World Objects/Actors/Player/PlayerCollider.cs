using Items;
using Items.ItemActors;
using Metadata;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Hazards;

namespace WorldObjects.Actors.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerCollider : ActorCollider
    {
        public SmartEvent<List<Item>> OnItemsCollected = new SmartEvent<List<Item>>();

        public SmartEvent<int> OnHealingApplied = new SmartEvent<int>();

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

        private void HandleItem(ItemActor itemActor)
        {
            _log.ErrorIfNull(itemActor, $"{itemActor} has tag {Tags.Item} but does not have an item component.");

            foreach (var interaction in itemActor.Interactions)
            {
                switch (interaction)
                {
                    case InteractionTypes.Collect:
                        var collectibleItem = itemActor as ICollectible;
                        _log.ErrorIfNull(collectibleItem, $"{itemActor} has interaction {interaction} but does not implement {typeof(ICollectible).Name}.");
                        OnItemsCollected.Raise(collectibleItem.CollectedItems);
                        break;

                    case InteractionTypes.Damage:
                        var damagingItem = itemActor as IDamaging;
                        _log.ErrorIfNull(damagingItem, $"{itemActor} has interaction {interaction} but does not implement {typeof(IDamaging).Name}.");
                        OnDamageTaken.Raise(damagingItem.Damage);
                        break;

                    case InteractionTypes.Healing:
                        var healingItem = itemActor as IHealing;
                        _log.ErrorIfNull(healingItem, $"{itemActor} has interaction {interaction} but does not implement {typeof(IHealing).Name}.");
                        OnHealingApplied.Raise(healingItem.Healing);
                        break;

                    default: _log.Error($"Unknown interaction '{interaction}'."); break;
                }
            }

            itemActor.HandledItem();
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("PlayerCollider");
    }
}