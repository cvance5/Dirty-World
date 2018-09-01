using System.Collections.Generic;

namespace ItemManagement.ItemActors
{
    public abstract class WealthItemActor : ItemActor, ICollectible
    {
        public abstract Dictionary<CollectibleType, int> CollectedItems { get; }

        public void OnCollect()
        {
            Destroy(gameObject);
        }

        protected override void InitializeInteractions() => Interactions = new InteractionTypes[] { InteractionTypes.Collect };
    }
}