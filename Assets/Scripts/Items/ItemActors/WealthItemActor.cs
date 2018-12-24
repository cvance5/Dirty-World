using System.Collections.Generic;

namespace Items.ItemActors
{
    public abstract class WealthItemActor : ItemActor, ICollectible
    {
        public abstract List<Item> CollectedItems { get; }

        public override InteractionTypes[] Interactions { get; } = new InteractionTypes[] { InteractionTypes.Collect };

        public override void HandledItem() => Destroy(gameObject);
    }
}