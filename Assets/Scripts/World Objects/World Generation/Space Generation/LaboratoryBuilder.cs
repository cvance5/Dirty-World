using System.Collections.Generic;
using System.Linq;
using Utilities.Debug;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class LaboratoryBuilder : SpaceBuilder
    {
        public override bool IsValid => _entryRoom != null && _treasureRoom != null;

        private SpaceBuilder _entryRoom;
        private List<SpaceBuilder> _rooms = new List<SpaceBuilder>();
        private Dictionary<SpaceBuilder, List<SpaceBuilder>> _roomMap
          = new Dictionary<SpaceBuilder, List<SpaceBuilder>>();
        private SpaceBuilder _treasureRoom;

        public LaboratoryBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder) => Rebuild();

        private void Rebuild()
        {
            _roomMap.Clear();
            _treasureRoom = null;

            _entryRoom = new ShaftBuilder(_chunkBuilder);
            _roomMap.Add(_entryRoom, new List<SpaceBuilder>());

            var previousRoom = _entryRoom;
            var direction = Directions.Up;

            while (_treasureRoom == null)
            {
                // Pick something perpendicular to the previous room
                direction = Directions.Cardinals.RandomItem(direction, -direction);
                var randomPoint = previousRoom.GetRandomPoint();

                var newRoomStartingPoint = randomPoint;
                while (previousRoom.Contains(newRoomStartingPoint + direction))
                {
                    newRoomStartingPoint += direction;
                }

                var newRoom = BuildRoom(newRoomStartingPoint, direction, previousRoom);
                _roomMap[previousRoom].Add(newRoom);
                _roomMap.Add(newRoom, new List<SpaceBuilder>());
                _rooms.Add(newRoom);
                previousRoom = newRoom;
            }
        }

        private SpaceBuilder BuildRoom(IntVector2 newRoomStartingPoint, IntVector2 randomDirection, SpaceBuilder previousRoom)
        {
            if (Chance.OneIn(5))
            {
                var room = new CorridorBuilder(_chunkBuilder)
                                    .SetStartingPoint(newRoomStartingPoint, randomDirection)
                                    .SetLength(6)
                                    .SetHeight(6)
                                    .SetAllowEnemies(false);

                _treasureRoom = room;

                return room;
            }
            else if (randomDirection == Directions.Up || randomDirection == Directions.Down)
            {
                return new ShaftBuilder(_chunkBuilder)
                                 .SetStartingPoint(newRoomStartingPoint, randomDirection)
                                 .SetWidth(LABORATORY_SHAFT_WIDTH);
            }
            else
            {
                return new CorridorBuilder(_chunkBuilder)
                                 .SetStartingPoint(newRoomStartingPoint, randomDirection)
                                 .SetHeight(LABORATORY_CORRIDOR_HEIGHT);
            }
        }

        public override bool Contains(IntVector2 point) => _roomMap.Keys.Any(room => room.Contains(point));
        public override int PassesBy(IntVector2 direction, int amount) => _roomMap.Keys.Max(room => room.PassesBy(direction, amount));
        public override IntVector2 GetRandomPoint() => _roomMap.RandomItem().Key.GetRandomPoint();

        public override void Clamp(IntVector2 direction, int amount)
        {
            var difference = PassesBy(direction, amount);
            ShiftRooms(_entryRoom, -direction * difference);
        }

        public override void Cut(IntVector2 direction, int amount)
        {
            foreach (var room in _roomMap.Keys)
            {
                if (room.PassesBy(direction, amount) > 0)
                {
                    RemoveRooms(room);
                }
            }
        }

        public override void Shift(IntVector2 shift) => ShiftRooms(_entryRoom, shift);
        private void ShiftRooms(SpaceBuilder roomToShift, IntVector2 shift)
        {
            roomToShift.Shift(shift);

            if (_roomMap.TryGetValue(roomToShift, out var connectedRooms))
            {
                foreach (var connectedRoom in connectedRooms)
                {
                    ShiftRooms(connectedRoom, shift);
                }
            }
        }

        private void RemoveRooms(SpaceBuilder roomToRemove)
        {
            if (_roomMap.TryGetValue(roomToRemove, out var connectedRooms))
            {
                foreach (var connectedRoom in connectedRooms)
                {
                    RemoveRooms(connectedRoom);
                }

                _roomMap.Remove(roomToRemove);
            }

            if (roomToRemove == _entryRoom)
            {
                _entryRoom = null;
            }

            if (roomToRemove == _treasureRoom)
            {
                _treasureRoom = null;
            }
        }

        protected override Space BuildRaw()
        {
            var rooms = new List<Space>();

            foreach (var roomToBuild in _rooms)
            {
                rooms.Add(roomToBuild.Build());
            }

            return new Laboratory(rooms);
        }

        private const int LABORATORY_SHAFT_WIDTH = 4;
        private const int LABORATORY_CORRIDOR_HEIGHT = 4;

        private static readonly Log _log = new Log("LaboratoryBuilder");
    }
}