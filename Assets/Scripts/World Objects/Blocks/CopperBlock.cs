namespace WorldObjects.Blocks
{
    public class CopperBlock : Block
    {
        public override string GetObjectName() => $"Copper {GetPosition()}";

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemType.CopperPiece, GetPosition());
        }
    }
}
