using Items.Unlocking;
using System.Collections.Generic;
using System.Linq;
using Utilities.Debug;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class LaboratoryBuilder : SpaceBuilder
    {
        public override bool IsValid => _entryRoom != null && _entryRoom.IsValid && _treasureRoom != null && _treasureRoom.IsValid;

        private SpaceBuilder _entryRoom;
        private List<SpaceBuilder> _rooms = new List<SpaceBuilder>();
        private Dictionary<SpaceBuilder, List<SpaceBuilder>> _roomMap
          = new Dictionary<SpaceBuilder, List<SpaceBuilder>>();
        private SpaceBuilder _treasureRoom;

        public LaboratoryBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _roomMap.Clear();
            _treasureRoom = null;

            var startingPoint = new IntVector2(UnityEngine.Random.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X + 1),
                                               UnityEngine.Random.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y + 1));

            var buildDirection = Directions.RandomLeftOrRight;
            _entryRoom = AddCorridor(startingPoint, buildDirection);
            _roomMap.Add(_entryRoom, new List<SpaceBuilder>());
            _rooms.Add(_entryRoom);

            var previousRoom = _entryRoom;

            while (_treasureRoom == null)
            {
                // Pick something perpendicular to the previous room
                buildDirection = Directions.Cardinals.RandomItem(buildDirection, -buildDirection);
                var randomPoint = previousRoom.GetRandomPoint();

                var newRoomStartingPoint = randomPoint;
                var newRoom = BuildRoom(newRoomStartingPoint, buildDirection);

                foreach (var direction in Directions.Cardinals)
                {
                    if (direction == buildDirection)
                    {
                        newRoom.Align(-direction, previousRoom.GetMaximalValue(direction));
                    }
                    else newRoom.AddBoundary(direction, previousRoom.GetMaximalValue(direction));
                }

                _roomMap[previousRoom].Add(newRoom);
                _roomMap.Add(newRoom, new List<SpaceBuilder>());
                _rooms.Add(newRoom);
                previousRoom = newRoom;
            }

            foreach (var room in _rooms)
            {
                var intersectingRooms = _rooms.FindAll(otherRoom => otherRoom.IntersectsWith(room));
                while (intersectingRooms.Count > 0 && room.IsValid)
                {
                    foreach (var intersectingRoom in intersectingRooms)
                    {
                        foreach (var boundedDirection in Directions.Cardinals)
                        {
                            room.AddBoundary(boundedDirection, intersectingRoom.GetMaximalValue(-boundedDirection));
                        }
                    }

                    intersectingRooms = _rooms.FindAll(otherRoom => room.IntersectsWith(room));
                }
            }
        }

        private SpaceBuilder BuildRoom(IntVector2 newRoomStartingPoint, IntVector2 direction)
        {
            if (Chance.OneIn(7))
            {
                var room = AddTreasureRoom(newRoomStartingPoint, direction);

                _treasureRoom = room;

                return room;
            }
            if (direction == Directions.Left || direction == Directions.Right)
            {
                return AddCorridor(newRoomStartingPoint, direction);
            }
            else if (direction == Directions.Up || direction == Directions.Down)
            {
                return AddShaft(newRoomStartingPoint, direction);
            }
            else throw new System.ArgumentOutOfRangeException($"Cannot build room in direction {direction}.  Expected cardinal direction.");
        }

        private SpaceBuilder AddTreasureRoom(IntVector2 newRoomStartingPoint, IntVector2 direction)
            => new TreasureRoomBuilder(_chunkBuilder)
                  .SetSize(LABORATORY_MINIMUM_SIZE)
                  .SetCenter(newRoomStartingPoint + (direction * LABORATORY_MINIMUM_SIZE))
                  .SetMinimumSize(LABORATORY_MINIMUM_SIZE)
                  .SetTreasure(new UnlockItem("Seismic Bomb", UnlockTypes.Weapon));

        private SpaceBuilder AddCorridor(IntVector2 newRoomStartingPoint, IntVector2 direction)
            => new CorridorBuilder(_chunkBuilder)
                  .SetStartingPoint(newRoomStartingPoint, direction)
                  .SetHeight(LABORATORY_MINIMUM_SIZE)
                  .SetMinimumHeight(LABORATORY_MINIMUM_SIZE)
                  .SetMinimumLength(LABORATORY_MINIMUM_SIZE);

        private SpaceBuilder AddShaft(IntVector2 newRoomStartingPoint, IntVector2 direction)
            => new ShaftBuilder(_chunkBuilder)
                  .SetStartingPoint(newRoomStartingPoint, direction)
                  .SetWidth(LABORATORY_MINIMUM_SIZE)
                  .SetMinimumWidth(LABORATORY_MINIMUM_SIZE)
                  .SetMinimumHeight(LABORATORY_MINIMUM_SIZE);

        public override bool Contains(IntVector2 point) => _rooms.Any(room => room.Contains(point));
        public override int PassesBy(IntVector2 edge, int target) => _rooms.Max(room => room.PassesBy(edge, target));

        public override IntVector2 GetRandomPoint() => _roomMap.RandomItem().Key.GetRandomPoint();

        public override int GetMaximalValue(IntVector2 direction) => _rooms.Max(room => room.GetMaximalValue(direction));

        public override SpaceBuilder Align(IntVector2 direction, int amount)
        {
            var difference = GetMaximalValue(direction) - amount;
            ShiftRooms(_entryRoom, -direction * difference);

            return this;
        }

        public override void Clamp(IntVector2 edge, int maxAmount)
        {
            var difference = PassesBy(edge, maxAmount);

            if (difference > 0)
            {
                ShiftRooms(_entryRoom, -edge * difference);
            }
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
                if (roomToBuild.IsValid)
                {
                    rooms.Add(roomToBuild.Build());
                }
            }

            return new Laboratory(rooms, 2);
        }

        private const int LABORATORY_MINIMUM_SIZE = 4;

        private static readonly Log _log = new Log("LaboratoryBuilder");
    }
}