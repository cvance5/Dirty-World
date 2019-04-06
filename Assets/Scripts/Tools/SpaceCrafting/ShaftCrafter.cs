using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class ShaftCrafter : SpaceCrafter
    {
        public int Width;
        public int Height;

        public bool IsUncapped;

        public override bool IsValid => MinX <= MaxX && MinY <= MaxY;

        private IntVector2 BottomLeftCorner => new IntVector2(transform.position.x, transform.position.y);
        private IntVector2 TopRightCorner => new IntVector2(transform.position.x + Width, transform.position.y + Height);

        public override int MinX => BottomLeftCorner.X;
        public override int MaxX => TopRightCorner.X;

        public override int MinY => BottomLeftCorner.Y;
        public override int MaxY => TopRightCorner.Y;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Shaft";

            Width = 4;
            Height = 6;
            IsUncapped = false;
        }

        protected override void InitializeFromSpaceRaw(Space space)
        {
            var shaft = space as Shaft;
            transform.position = shaft.BottomLeftCorner;
            Width = shaft.TopRightCorner.X - shaft.BottomLeftCorner.X;
            Height = shaft.TopRightCorner.Y - shaft.BottomLeftCorner.Y;
            IsUncapped = shaft.IsUncapped;
        }

        protected override Space RawBuild() => new Shaft(BottomLeftCorner, TopRightCorner, IsUncapped);
    }
}