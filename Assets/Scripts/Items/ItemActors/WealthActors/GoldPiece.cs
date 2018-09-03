﻿using System.Collections.Generic;

namespace Items.ItemActors.WealthActors
{
    public class GoldPiece : WealthItemActor
    {
        public override List<Item> CollectedItems { get; } = new List<Item>()
        {
            new Item(ItemCategories.Wealth, 25)
        };
    }
}