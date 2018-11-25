using Items.ItemActors;

namespace WorldObjects.Blocks
{
    public class CopperBlock : Block
    {
        public override string ObjectName => $"Copper {Position}";
        public override BlockTypes Type => BlockTypes.Copper;

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemActorTypes.CopperPiece, Position);
        }
    }
}
