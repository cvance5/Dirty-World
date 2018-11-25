using Items.ItemActors;

namespace WorldObjects.Blocks
{
    public class GoldBlock : Block
    {
        public override string ObjectName => $"Gold {Position}";
        public override BlockTypes Type => BlockTypes.Gold;

        protected override void DropItem()
        {
            ItemLoader.CreateItem(ItemActorTypes.GoldPiece, Position);
        }
    }
}
