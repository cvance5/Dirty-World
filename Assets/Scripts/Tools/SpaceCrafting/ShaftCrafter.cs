using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class ShaftCrafter : SpaceCrafter
    {
        public IntVector2 BottomLeftCorner;
        public IntVector2 TopRightCorner;

        public bool IsUncapped;

        public override bool IsValid => MinX <= MaxX && MinY <= MaxY;

        public override int MinX => BottomLeftCorner.X;
        public override int MaxX => TopRightCorner.X;

        public override int MinY => BottomLeftCorner.Y;
        public override int MaxY => TopRightCorner.Y;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Shaft";

            BottomLeftCorner = new IntVector2(-2, -2);
            TopRightCorner = new IntVector2(2, 2);
            IsUncapped = false;
        }

        protected override Space RawBuild() => new Shaft(BottomLeftCorner, TopRightCorner, IsUncapped);
    }
}