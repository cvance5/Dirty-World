using System.Collections.Generic;

namespace ItemManagement.Items.Wealth
{
    public class SilverPiece : WealthItem
    {
        public override Dictionary<CollectibleType, int> CollectedItems { get; } = new Dictionary<CollectibleType, int>()
        {
            { CollectibleType.Wealth, 10 }
        };
    }
}