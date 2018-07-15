namespace WorldObjects.Blocks
{
    public class SilverBlock : Block
    {
        public override string GetObjectName() => $"Silver {GetPosition()}";

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemType.SilverPiece, GetPosition());
        }
    }
}
