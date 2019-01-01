using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class MonsterDenCrafter : SpaceCrafter
    {
        public IntVector2 Centerpoint;
        public int Radius;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Monster Den";

            Centerpoint = new IntVector2(0, 0);
            Radius = 3;
        }

        protected override void BuildSpace()
        {
            Result = new MonsterDen(Centerpoint, Radius);
            UpdateAffectedChunks();
        }

        protected override void UpdateAffectedChunks()
        {
            var minX = (Centerpoint.X - Radius) / SpaceCraftingManager.ChunkSize;
            var maxX = (Centerpoint.X + Radius) / SpaceCraftingManager.ChunkSize;

            var minY = Centerpoint.Y / SpaceCraftingManager.ChunkSize;
            var maxY = (Centerpoint.Y + Radius) / SpaceCraftingManager.ChunkSize;

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