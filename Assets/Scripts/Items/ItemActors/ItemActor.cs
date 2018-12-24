using UnityEngine;
using WorldObjects;

namespace Items.ItemActors
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ItemActor : WorldObject
    {
        public SmartEvent<ItemActor> OnItemDestroyed = new SmartEvent<ItemActor>();

        public abstract InteractionTypes[] Interactions { get; }

        public abstract ItemActorTypes Type { get; }

        public abstract void HandledItem();

        protected override void OnWorldObjectDestroy() => OnItemDestroyed.Raise(this);
    }
}