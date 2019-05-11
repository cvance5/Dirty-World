using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class TunnelCrafter : SpaceCrafter
    {
        public int Width;
        public int Height;

        public override bool IsValid => MinX <= MaxX && MinY <= MaxY;

        private IntVector2 BottomLeftCorner => new IntVector2(transform.position.x, transform.position.y);
        private IntVector2 TopRightCorner => new IntVector2(transform.position.x + Width, transform.position.y + Height);

        public override int MinX => BottomLeftCorner.X;
        public override int MaxX => TopRightCorner.X;

        public override int MinY => BottomLeftCorner.Y;
        public override int MaxY => TopRightCorner.Y;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Corridor";

            Width = 4;
            Height = 2;
        }

        protected override void InitializeFromSpaceRaw(Space space)
        {
            var tunnel = space as Tunnel;
            transform.position = tunnel.BottomLeftCorner;
            Width = tunnel.TopRightCorner.X - tunnel.BottomLeftCorner.X;
            Height = tunnel.TopRightCorner.Y - tunnel.BottomLeftCorner.Y;
        }

        protected override Space RawBuild() => new Tunnel(BottomLeftCorner, TopRightCorner);
    }
}