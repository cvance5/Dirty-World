using Items.ItemActors;

namespace WorldObjects.Blocks
{
    public class PlatinumBlock : Block
    {
        public override string ObjectName => $"Platinum {Position}";
        public override BlockTypes Type => BlockTypes.Platinum;

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemActorTypes.PlatinumPiece, Position);
        }
    }
}
