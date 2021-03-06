﻿using System.Collections.Generic;

namespace Items.ItemActors.WealthActors
{
    public class PlatinumPiece : WealthItemActor
    {
        public override List<Item> CollectedItems { get; } = new List<Item>()
        {
            new Item(ItemCategories.Wealth, 50)
        };

        public override ItemActorTypes Type { get; } = ItemActorTypes.PlatinumPiece;

        public override string ObjectName { get; } = "Platinum Piece";
    }
}