using System.Collections.Generic;

namespace ItemManagement.Items.Wealth
{
    public class PlatinumPiece : WealthItem
    {
        public override Dictionary<CollectibleType, int> CollectedItems { get; } = new Dictionary<CollectibleType, int>()
        {
            { CollectibleType.Wealth, 50 }
        };
    }
}