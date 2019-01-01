using Items;
using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class RoomCrafter : SpaceCrafter
    {
        public IntVector2 BottomLeftCorner;
        public IntVector2 TopRightCorner;

        public Item[] Treasure = new Item[0];

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Room";

            BottomLeftCorner = new IntVector2(-2, -2);
            TopRightCorner = new IntVector2(2, 2);
        }

        protected override void BuildSpace()
        {
            if (Treasure.Length == 0)
            {
                Result = new Room(BottomLeftCorner, TopRightCorner);
            }
            else
            {
                Result = new TreasureRoom(BottomLeftCorner, TopRightCorner, Treasure);
            }
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