using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class ShaftCrafter : SpaceCrafter
    {
        public IntVector2 BottomLeftCorner;
        public IntVector2 TopRightCorner;

        public bool IsUncapped;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Shaft";

            BottomLeftCorner = new IntVector2(-2, -2);
            TopRightCorner = new IntVector2(2, 2);
            IsUncapped = false;
        }

        protected override void BuildSpace()
        {
            Result = new Shaft(BottomLeftCorner, TopRightCorner, IsUncapped);
            UpdateAffectedChunks();
        }

        protected override void UpdateAffectedChunks()
        {
            ChunksAffected.Clear();

            var minX = BottomLeftCorner.X / SpaceCraftingManager.ChunkSize;
            var maxX = TopRightCorner.X / SpaceCraftingManager.ChunkSize;

            var minY = BottomLeftCorner.Y / SpaceCraftingManager.ChunkSize;
            var maxY = TopRightCorner.Y / SpaceCraftingManager.ChunkSize;

            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    ChunksAffected.Add(new IntVector2(x, y));
                }
            }
        }
    }
}