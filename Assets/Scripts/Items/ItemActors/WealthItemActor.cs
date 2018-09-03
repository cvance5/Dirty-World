using System.Collections.Generic;

namespace Items.ItemActors
{
    public abstract class WealthItemActor : ItemActor, ICollectible
    {
        public abstract List<Item> CollectedItems { get; }

        public void OnCollect()
        {
            Destroy(gameObject);
        }

        protected override void InitializeInteractions() => Interactions = new InteractionTypes[] { InteractionTypes.Collect };
    }
}