using Items.ItemActors;

namespace WorldObjects.Blocks
{
    public class GoldBlock : Block
    {
        public override string ObjectName => $"Gold {Position}";

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemActorTypes.GoldPiece, Position);
        }
    }
}
