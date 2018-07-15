namespace WorldObjects.Blocks
{
    public class PlatinumBlock : Block
    {
        public override string GetObjectName() => $"Platinum {GetPosition()}";

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemType.PlatinumPiece, GetPosition());
        }
    }
}
