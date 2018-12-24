using System.Collections.Generic;

namespace Items.ItemActors.WealthActors
{
    public class GoldPiece : WealthItemActor
    {
        public override List<Item> CollectedItems { get; } = new List<Item>()
        {
            new Item(ItemCategories.Wealth, 25)
        };

        public override ItemActorTypes Type { get; } = ItemActorTypes.GoldPiece;

        public override string ObjectName { get; } = "Gold Piece";
    }
}