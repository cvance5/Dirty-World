﻿using Items.ItemActors;

namespace WorldObjects.Blocks
{
    public class PlatinumBlock : Block
    {
        public override string ObjectName => $"Platinum {Position}";

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemActorTypes.PlatinumPiece, Position);
        }
    }
}
