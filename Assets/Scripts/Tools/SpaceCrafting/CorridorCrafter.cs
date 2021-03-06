﻿using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class CorridorCrafter : SpaceCrafter
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
            var corridor = space as Corridor;
            transform.position = corridor.BottomLeftCorner;
            Width = corridor.TopRightCorner.X - corridor.BottomLeftCorner.X;
            Height = corridor.TopRightCorner.Y - corridor.BottomLeftCorner.Y;
        }

        protected override Space RawBuild() => new Corridor(BottomLeftCorner, TopRightCorner);
    }
}