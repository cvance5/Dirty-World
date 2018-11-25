using Items.ItemActors;

namespace WorldObjects.Blocks
{
    public class SilverBlock : Block
    {
        public override string ObjectName => $"Silver {Position}";
        public override BlockTypes Type => BlockTypes.Silver;

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemActorTypes.SilverPiece, Position);
        }
    }
}
