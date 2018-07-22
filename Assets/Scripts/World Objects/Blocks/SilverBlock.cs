namespace WorldObjects.Blocks
{
    public class SilverBlock : Block
    {
        public override string ObjectName => $"Silver {Position}";

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemType.SilverPiece, Position);
        }
    }
}
