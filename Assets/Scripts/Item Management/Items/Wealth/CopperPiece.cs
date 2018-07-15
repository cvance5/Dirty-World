using System.Collections.Generic;

namespace ItemManagement.Items.Wealth
{
    public class CopperPiece : WealthItem
    {
        public override Dictionary<CollectibleType, int> CollectedItems { get; } = new Dictionary<CollectibleType, int>()
        {
            { CollectibleType.Wealth, 1 }
        };
    }
}