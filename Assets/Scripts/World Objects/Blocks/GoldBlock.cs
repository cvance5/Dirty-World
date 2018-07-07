namespace WorldObjects.Blocks
{
    public class GoldBlock : Block
    {
        public override string GetObjectName() => $"Gold {Position}";

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemTypes.GoldPiece, Position);
        }
    }
}
