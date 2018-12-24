using System.Collections.Generic;

namespace Items.ItemActors.WealthActors
{
    public class SilverPiece : WealthItemActor
    {
        public override List<Item> CollectedItems { get; } = new List<Item>()
        {
            new Item(ItemCategories.Wealth, 10)
        };

        public override ItemActorTypes Type { get; } = ItemActorTypes.SilverPiece;

        public override string ObjectName { get; } = "Silver Piece";
    }
}