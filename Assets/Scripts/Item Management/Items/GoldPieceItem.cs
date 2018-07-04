namespace ItemManagement.Items
{
    public class GoldPieceItem : Item, ICollectible
    {
        public ItemTypes GetItemType() => ItemTypes.GoldPiece;

        public void OnCollect()
        {
            Log.Info("BOOP!", "green");
            Destroy(gameObject);
        }

        protected override void InitializeInteractions() => Interactions = new InteractionTypes[] { InteractionTypes.Collect };
    }
}