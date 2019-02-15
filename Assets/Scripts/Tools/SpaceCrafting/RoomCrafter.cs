using Items;
using WorldObjects.Spaces;

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

        public Item[] Treasure = new Item[0];

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Room";

            Width = 4;
            Height = 4;
        }

        public override void InitializeFromSpace(Space space)
        {
            var room = space as Room;
            transform.position = room.BottomLeftCorner;
            Width = room.TopRightCorner.X - room.BottomLeftCorner.X;
            Height = room.TopRightCorner.Y - room.BottomLeftCorner.Y;

            if (space is TreasureRoom treasureRoom)
            {
                Treasure = treasureRoom.Treasure ?? new Item[0];
            }
            else Treasure = new Item[0];

            InitializeEnemySpawns(room.EnemySpawns);
        }

        protected override Space RawBuild() => Treasure.Length == 0
                ? new Room(BottomLeftCorner, TopRightCorner)
                : new TreasureRoom(BottomLeftCorner, TopRightCorner, Treasure);
    }
}