using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class CorridorCrafter : SpaceCrafter
    {
        public IntVector2 BottomLeftCorner;
        public IntVector2 TopRightCorner;

        public override bool IsValid => MinX <= MaxX && MinY <= MaxY;

        public override int MinX => BottomLeftCorner.X;
        public override int MaxX => TopRightCorner.X;

        public override int MinY => BottomLeftCorner.Y;
        public override int MaxY => TopRightCorner.Y;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Corridor";

            BottomLeftCorner = new IntVector2(-2, -2);
            TopRightCorner = new IntVector2(2, 2);
        }

        public override Space Build() => new Corridor(BottomLeftCorner, TopRightCorner);
    }
}