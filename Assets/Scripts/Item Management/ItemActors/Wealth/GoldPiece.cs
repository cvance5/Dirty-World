using System.Collections.Generic;

namespace ItemManagement.ItemActors.Wealth
{
    public class GoldPiece : WealthItemActor
    {
        public override Dictionary<CollectibleType, int> CollectedItems { get; } = new Dictionary<CollectibleType, int>()
        {
            { CollectibleType.Wealth, 25 }
        };
    }
}