using System.Collections.Generic;

namespace ItemManagement.Items
{
    public abstract class WealthItem : Item, ICollectible
    {
        public abstract Dictionary<CollectibleType, int> CollectedItems { get; }

        public void OnCollect()
        {
            Destroy(gameObject);
        }

        protected override void InitializeInteractions() => Interactions = new InteractionTypes[] { InteractionTypes.Collect };
    }
}