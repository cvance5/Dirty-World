namespace WorldObjects.Blocks
{
    public class GoldBlock : Block
    {
        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemTypes.GoldPiece, Position);
        }
    }
}
