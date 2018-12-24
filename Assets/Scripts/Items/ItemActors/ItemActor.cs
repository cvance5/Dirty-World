using UnityEngine;

namespace Items.ItemActors
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ItemActor : MonoBehaviour, ITrackable
    {
        public SmartEvent<ItemActor> OnItemDestroyed = new SmartEvent<ItemActor>();

        public abstract InteractionTypes[] Interactions { get; }

        public IntVector2 Position => new IntVector2(transform.position);
        public abstract ItemActorTypes Type { get; }

        public abstract void HandledItem();

        private void OnDestroy() => OnItemDestroyed.Raise(this);
    }
}