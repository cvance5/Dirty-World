using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace Tools.SpaceCrafting
{
    public class RoomCrafter : SpaceCrafter
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
            gameObject.name = "Room";

            Width = 4;
            Height = 4;
        }

        protected override void InitializeFromSpaceRaw(Space space)
        {
            var room = space;
            transform.position = room.Extents.Min;
            Width = room.Extents.Max.X - room.Extents.Min.X;
            Height = room.Extents.Max.Y - room.Extents.Min.Y;
        }

        protected override Space RawBuild()
        {
            var extents = new Extents(new List<IntVector2>()
            {
                new IntVector2(BottomLeftCorner),
                new IntVector2(BottomLeftCorner.X, TopRightCorner.Y),
                new IntVector2(TopRightCorner),
                new IntVector2(TopRightCorner.X, BottomLeftCorner.Y)
            });

            return new Space($"Room {SpaceNamer.GetName()}", extents);
        }
    }
}