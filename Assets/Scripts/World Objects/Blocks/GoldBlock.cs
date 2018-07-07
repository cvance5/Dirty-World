namespace WorldObjects.Blocks
{
    public class GoldBlock : Block
    {
        public override string GetObjectName() => $"Gold {GetPosition()}";

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemTypes.GoldPiece, GetPosition());
        }
    }
}
