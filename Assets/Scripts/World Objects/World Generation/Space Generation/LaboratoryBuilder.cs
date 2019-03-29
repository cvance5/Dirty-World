using Items.Unlocking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Debug;
using WorldObjects.Spaces;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class LaboratoryBuilder : SpaceBuilder
    {
        public override bool IsValid => _mainShafts.Count > 0 && _treasureRoom != null && _treasureRoom.IsValid;

        private readonly int _maximumCorridorSegments;

        private readonly List<ShaftBuilder> _mainShafts;

        private readonly Dictionary<ShaftBuilder, List<CorridorBuilder>> _corridorsByShaft;
        private readonly Dictionary<ShaftBuilder, List<ShaftBuilder>> _secondaryShaftsByShaft;

        private readonly Dictionary<CorridorBuilder, RoomBuilder> _connectionsByCorridor;

        private readonly TreasureRoomBuilder _treasureRoom;

        private bool _allowAdditionalMainShafts = true;

        public LaboratoryBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _mainShafts = new List<ShaftBuilder>();
            _corridorsByShaft = new Dictionary<ShaftBuilder, List<CorridorBuilder>>();
            _secondaryShaftsByShaft = new Dictionary<ShaftBuilder, List<ShaftBuilder>>();

            _connectionsByCorridor = new Dictionary<CorridorBuilder, RoomBuilder>();

            _maximumCorridorSegments = Chance.Range(4, 8);

            var highestStory = Chance.Range(6, 10);
            var shaftPosition = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X + 1),
                                               Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y + 1));

            RegisterNewMainShaft(AddShaft(0, highestStory, shaftPosition), highestStory);

            var randomConnectionWithRoom = _connectionsByCorridor.RandomItem(kvp => !(kvp.Value is null));
            _treasureRoom = PromoteTreasureRoom(randomConnectionWithRoom.Value);
            _connectionsByCorridor[randomConnectionWithRoom.Key] = _treasureRoom;

            OnSpaceBuilderChanged.Raise(this);
        }

        private void RegisterNewMainShaft(ShaftBuilder mainShaft, int height)
        {
            if (_allowAdditionalMainShafts)
            {
                _mainShafts.Add(mainShaft);
                mainShaft.AddModifier(ModifierTypes.Laboratory);

                GenerateCorridors(mainShaft, height);
                GenerateSecondaryShafts(mainShaft, height);

                // The more shafts there are, the less likely we are to allow any more
                if (Chance.ChanceOf(_mainShafts.Count / (float)MAX_MAIN_SHAFTS))
                {
                    _allowAdditionalMainShafts = false;
                }
            }

            OnSpaceBuilderChanged.Raise(this);
        }

        private List<ShaftBuilder> FindMainShaftsThatReachPosition(IntVector2 position)
        {
            var shaftsReachingPosition = new List<ShaftBuilder>();

            foreach (var shaft in _mainShafts)
            {
                var maxY = GetMaximalValueForSpacesInShaft(shaft, Directions.Up);
                var maxX = GetMaximalValueForSpacesInShaft(shaft, Directions.Right);
                var minY = GetMaximalValueForSpacesInShaft(shaft, Directions.Down);
                var minX = GetMaximalValueForSpacesInShaft(shaft, Directions.Left);

                if (position.X >= minX &&
                   position.X <= maxX &&
                   position.Y >= minY &&
                   position.Y <= maxY)
                {
                    shaftsReachingPosition.Add(shaft);
                }
            }

            return shaftsReachingPosition;
        }

        private List<ShaftBuilder> FindMainShaftsThatPassEdge(IntVector2 direction, int target)
        {
            var shaftsPassingEdge = new List<ShaftBuilder>();

            foreach (var shaft in _mainShafts)
            {
                if (direction == Directions.Up ||
                    direction == Directions.Right)
                {
                    if (GetMaximalValueForSpacesInShaft(shaft, direction) > target) shaftsPassingEdge.Add(shaft);
                }
                else if (direction == Directions.Down ||
                         direction == Directions.Left)
                {
                    if (GetMaximalValueForSpacesInShaft(shaft, direction) < target) shaftsPassingEdge.Add(shaft);
                }
                else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
            }

            return shaftsPassingEdge;
        }

        private List<SpaceBuilder> GetSpacesForShaft(ShaftBuilder mainShaft)
        {
            var spaces = new List<SpaceBuilder>
            {
                mainShaft
            };

            foreach (var corridorToBuild in _corridorsByShaft[mainShaft])
            {
                spaces.Add(corridorToBuild);

                if (_connectionsByCorridor[corridorToBuild] != null)
                {
                    spaces.Add(_connectionsByCorridor[corridorToBuild]);
                }
            }

            foreach (var shaftToBuild in _secondaryShaftsByShaft[mainShaft])
            {
                spaces.Add(shaftToBuild);
            }

            return spaces;
        }

        private List<SpaceBuilder> GetSpacesForShafts(List<ShaftBuilder> mainShafts)
        {
            var spaces = new List<SpaceBuilder>();

            foreach (var mainShaft in mainShafts)
            {
                spaces.AddRange(GetSpacesForShaft(mainShaft));
            }

            return spaces;
        }

        private void GenerateCorridors(ShaftBuilder mainShaft, int numberStories)
        {
            if (_corridorsByShaft.TryGetValue(mainShaft, out var corridors))
            {
                corridors.Clear();
            }
            else corridors = new List<CorridorBuilder>();

            var shaftLeftSide = mainShaft.GetMaximalValue(Directions.Left);
            var shaftRightSide = mainShaft.GetMaximalValue(Directions.Right);

            var shaftY = mainShaft.GetMaximalValue(Directions.Down);

            for (var story = 0; story < Mathf.Abs(numberStories); story++)
            {
                var yPosition = GetYPositionForStory(shaftY, story);

                if (Chance.CoinFlip)
                {
                    var corridor = AddCorridor(new IntVector2(shaftLeftSide, yPosition), Directions.Left);
                    corridor.AddModifier(ModifierTypes.Laboratory);
                    var room = AddRoom(new IntVector2(corridor.GetMaximalValue(Directions.Left), yPosition), Directions.Left);
                    room.AddModifier(ModifierTypes.Laboratory);
                    corridors.Add(corridor);
                    _connectionsByCorridor.Add(corridor, room);
                }
                if (Chance.CoinFlip)
                {
                    var corridor = AddCorridor(new IntVector2(shaftRightSide, yPosition), Directions.Right);
                    corridor.AddModifier(ModifierTypes.Laboratory);
                    var room = AddRoom(new IntVector2(corridor.GetMaximalValue(Directions.Right), yPosition), Directions.Right);
                    room.AddModifier(ModifierTypes.Laboratory);
                    corridors.Add(corridor);
                    _connectionsByCorridor.Add(corridor, room);
                }
            }

            _corridorsByShaft[mainShaft] = corridors;

            OnSpaceBuilderChanged.Raise(this);
        }

        private void GenerateSecondaryShafts(ShaftBuilder mainShaft, int height)
        {
            if (_secondaryShaftsByShaft.TryGetValue(mainShaft, out var shafts))
            {
                shafts.Clear();
            }
            else shafts = new List<ShaftBuilder>();

            var shaftY = mainShaft.GetMaximalValue(Directions.Down);
            var shaftX = mainShaft.GetMaximalValue(Directions.Left);

            for (var corridorSegment = -_maximumCorridorSegments; corridorSegment < _maximumCorridorSegments; corridorSegment++)
            {
                if (corridorSegment == 0) continue; // Main shaft...

                var xPosition = (CORRIDOR_SEGMENT_LENGTH * corridorSegment) + shaftX;

                var validStories = new List<int>();

                for (var story = 0; story < Mathf.Abs(height); story++)
                {
                    var yPosition = GetYPositionForStory(shaftY, story);

                    var corridorPosition = new IntVector2(xPosition, yPosition);

                    if (_corridorsByShaft[mainShaft].Any(corridor => corridor.Contains(corridorPosition)))
                    {
                        validStories.Add(story);
                    }
                }

                if (validStories.Count > 0)
                {
                    if (corridorSegment == -_maximumCorridorSegments ||
                       corridorSegment == _maximumCorridorSegments)
                    {
                        var storyStartForShaft = validStories.RandomItem();
                        var yPosition = GetYPositionForStory(shaftY, storyStartForShaft);
                        var newShaftHeight = Chance.Range(-10, 10);

                        var shaft = AddShaft(storyStartForShaft, storyStartForShaft + newShaftHeight, new IntVector2(xPosition, yPosition));
                        RegisterNewMainShaft(shaft, newShaftHeight);

                        validStories.Remove(storyStartForShaft);
                    }
                    else if (validStories.Contains(0))
                    {
                        var newShaftHeight = Chance.Range(-10, -4);

                        var shaft = AddShaft(0, newShaftHeight, new IntVector2(xPosition, shaftY));
                        RegisterNewMainShaft(shaft, newShaftHeight);
                        validStories.Remove(0);
                    }
                    else if (validStories.Contains(height))
                    {
                        var yPosition = GetYPositionForStory(shaftY, height);
                        var newShaftHeight = Chance.Range(4, 10);

                        var shaft = AddShaft(height, height + newShaftHeight, new IntVector2(xPosition, yPosition));
                        RegisterNewMainShaft(shaft, newShaftHeight);
                        validStories.Remove(height);
                    }

                    // At least two stories we can reach...
                    while (validStories.Count > 1)
                    {
                        var firstStory = validStories.RandomItem();
                        var secondStory = validStories.RandomItem(firstStory);

                        var shaftPosition = new IntVector2(xPosition, GetYPositionForStory(shaftY, firstStory));

                        var shaft = AddShaft(Mathf.Min(firstStory, secondStory), Mathf.Max(firstStory, secondStory), shaftPosition);

                        if (!shafts.Any(otherShaft => otherShaft.IntersectsWith(shaft)))
                        {
                            shaft.AddModifier(ModifierTypes.Laboratory);
                            shafts.Add(shaft);
                        }

                        foreach (var corridor in _corridorsByShaft[mainShaft])
                        {
                            if (shaft.IntersectsWith(_connectionsByCorridor[corridor]))
                            {
                                _connectionsByCorridor[corridor] = null;
                            }
                        }

                        validStories.Remove(firstStory);
                        validStories.Remove(secondStory);
                    }
                }
            }

            _secondaryShaftsByShaft[mainShaft] = shafts;
            OnSpaceBuilderChanged.Raise(this);
        }

        private RoomBuilder AddRoom(IntVector2 corridorEndpoint, IntVector2 direction)
        {
            var room = new RoomBuilder(_chunkBuilder)
                             .SetCenter(corridorEndpoint + (direction * ROOM_SIZE) + (Directions.Up * ROOM_SIZE))
                             .SetSize(ROOM_SIZE)
                             .SetMinimumSize(ROOM_SIZE);

            OnSpaceBuilderChanged.Raise(this);

            return room;
        }

        private TreasureRoomBuilder PromoteTreasureRoom(RoomBuilder roomBuilder)
        {
            var treasureRoom = new TreasureRoomBuilder(roomBuilder)
                             .SetTreasure(new UnlockItem("Seismic Bomb", UnlockTypes.Weapon));

            OnSpaceBuilderChanged.Raise(this);

            return treasureRoom;
        }

        private CorridorBuilder AddCorridor(IntVector2 attachPoint, IntVector2 direction)
        {
            var corridor = new CorridorBuilder(_chunkBuilder)
                             .SetStartingPoint(attachPoint, direction)
                             .SetHeight(CORRIDOR_HEIGHT)
                             .SetLength(CORRIDOR_SEGMENT_LENGTH * Chance.Range(1, _maximumCorridorSegments))
                             .SetMinimumHeight(CORRIDOR_HEIGHT)
                             .SetMinimumLength(CORRIDOR_SEGMENT_LENGTH);

            OnSpaceBuilderChanged.Raise(this);

            return corridor;
        }

        private ShaftBuilder AddShaft(int startingStory, int endingStory, IntVector2 position)
        {
            var shaft = new ShaftBuilder(_chunkBuilder)
                             .SetStartingPoint(position, startingStory < endingStory ? Directions.Up : Directions.Down)
                             .SetWidth(ROOM_SIZE)
                             .SetHeight(STORY_SIZE * (1 + Mathf.Abs((endingStory - startingStory))))
                             .SetMinimumWidth(ROOM_SIZE)
                             .SetMinimumHeight(STORY_SIZE)
                             .SetUncapped(true);

            OnSpaceBuilderChanged.Raise(this);

            return shaft;
        }

        public override bool Contains(IntVector2 point)
        {
            var possibleContainingShafts = FindMainShaftsThatReachPosition(point);

            var possibleContainingSpaces = GetSpacesForShafts(possibleContainingShafts);

            return possibleContainingSpaces.Any(space => space.Contains(point));
        }

        public override int PassesBy(IntVector2 edge, int target)
        {
            var shaftsPassingEdge = FindMainShaftsThatPassEdge(edge, target);

            var possiblePassingSpaces = GetSpacesForShafts(shaftsPassingEdge);

            return possiblePassingSpaces.Max(space => space.PassesBy(edge, target));
        }

        public override IntVector2 GetRandomPoint() => GetSpacesForShaft(_mainShafts.RandomItem()).RandomItem().GetRandomPoint();

        private int GetMaximalValueForSpacesInShaft(ShaftBuilder shaft, IntVector2 direction)
        {
            if (direction == Directions.Up)
            {
                return GetSpacesForShaft(shaft).Max(space => space.GetMaximalValue(direction));
            }
            else if (direction == Directions.Right)
            {
                return GetSpacesForShaft(shaft).Max(space => space.GetMaximalValue(direction)) + (_maximumCorridorSegments * CORRIDOR_SEGMENT_LENGTH) + METAL_THICKNESS;
            }
            else if (direction == Directions.Down)
            {
                return GetSpacesForShaft(shaft).Min(space => space.GetMaximalValue(direction));
            }
            else if (direction == Directions.Left)
            {
                return GetSpacesForShaft(shaft).Min(space => space.GetMaximalValue(direction)) - (_maximumCorridorSegments * CORRIDOR_SEGMENT_LENGTH);
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public override int GetMaximalValue(IntVector2 direction)
        {
            ShaftBuilder shaftWithMaximalReach;

            if (direction == Directions.Up)
            {
                shaftWithMaximalReach = _mainShafts.OrderByDescending(shaft => shaft.GetMaximalValue(direction)).FirstOrDefault();
            }
            else if (direction == Directions.Right)
            {
                shaftWithMaximalReach = _mainShafts.OrderByDescending(shaft => shaft.GetMaximalValue(direction)).FirstOrDefault();
            }
            else if (direction == Directions.Down)
            {
                shaftWithMaximalReach = _mainShafts.OrderByDescending(shaft => shaft.GetMaximalValue(direction)).FirstOrDefault();
            }
            else if (direction == Directions.Left)
            {
                shaftWithMaximalReach = _mainShafts.OrderByDescending(shaft => shaft.GetMaximalValue(direction)).FirstOrDefault();
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            return GetMaximalValueForSpacesInShaft(shaftWithMaximalReach, direction);
        }

        public override SpaceBuilder Align(IntVector2 direction, int amount)
        {
            var maximalValue = GetMaximalValue(direction);
            var difference = maximalValue - amount;
            Shift(direction * difference);

            return this;
        }

        public override void Clamp(IntVector2 edge, int maxAmount)
        {
            var difference = PassesBy(edge, maxAmount);
            if (difference > 0)
            {
                Align(edge, maxAmount);
            }
        }

        public override void Cut(IntVector2 direction, int amount)
        {
            var shaftsThatPassEdge = FindMainShaftsThatPassEdge(direction, amount);
            foreach (var mainShaft in shaftsThatPassEdge)
            {
                if (mainShaft.PassesBy(direction, amount) > 0)
                {
                    RemoveMainShaft(mainShaft);
                }
                else
                {
                    foreach (var corridor in _corridorsByShaft[mainShaft].ToList())
                    {
                        if (corridor.PassesBy(direction, amount) > 0)
                        {
                            _connectionsByCorridor.Remove(corridor);
                            _corridorsByShaft[mainShaft].Remove(corridor);
                        }
                    }

                    foreach (var secondaryShaft in _secondaryShaftsByShaft[mainShaft].ToList())
                    {
                        if (secondaryShaft.PassesBy(direction, amount) > 0)
                        {
                            _secondaryShaftsByShaft[mainShaft].Remove(secondaryShaft);
                        }
                    }
                }
            }

            OnSpaceBuilderChanged.Raise(this);
        }

        public override void Shift(IntVector2 shift)
        {
            var spacesToShift = GetSpacesForShafts(_mainShafts);
            foreach (var spaceToShift in spacesToShift)
            {
                spaceToShift.Shift(shift);
            }

            OnSpaceBuilderChanged.Raise(this);
        }

        private void RemoveMainShaft(ShaftBuilder mainShaft)
        {
            foreach (var corridor in _corridorsByShaft[mainShaft])
            {
                _connectionsByCorridor.Remove(corridor);
            }
            _corridorsByShaft.Remove(mainShaft);
            _secondaryShaftsByShaft.Remove(mainShaft);
            _mainShafts.Remove(mainShaft);

            OnSpaceBuilderChanged.Raise(this);
        }

        protected override Space BuildRaw()
        {
            var regions = new List<Region>();

            foreach (var mainShaft in _mainShafts)
            {
                var bottomLeftCorner = new IntVector2(GetMaximalValueForSpacesInShaft(mainShaft, Directions.Left) - METAL_THICKNESS,
                                                      GetMaximalValueForSpacesInShaft(mainShaft, Directions.Down) - METAL_THICKNESS);


                var topRightCorner = new IntVector2(GetMaximalValueForSpacesInShaft(mainShaft, Directions.Right) + METAL_THICKNESS,
                                                    GetMaximalValueForSpacesInShaft(mainShaft, Directions.Up) + METAL_THICKNESS);

                var builders = GetSpacesForShaft(mainShaft);
                var spaces = new List<Space>();

                foreach (var builder in builders)
                {
                    if (builder.IsValid)
                    {
                        spaces.Add(builder.Build());
                    }
                }

                var region = new Region(bottomLeftCorner, topRightCorner, spaces);

                regions.Add(region);
            }

            return new Laboratory(regions, METAL_THICKNESS);
        }

        private int GetYPositionForStory(int baseY, int story) => baseY + (STORY_SIZE * story) + METAL_THICKNESS;

        private const int ROOM_SIZE = 6;
        private const int METAL_THICKNESS = 3;
        // A story is the size of a room, and the floor under it
        private const int STORY_SIZE = METAL_THICKNESS + (ROOM_SIZE * 2);

        private const int CORRIDOR_HEIGHT = ROOM_SIZE - METAL_THICKNESS;
        private const int CORRIDOR_SEGMENT_LENGTH = (ROOM_SIZE + METAL_THICKNESS) * 2;

        private const int MAX_MAIN_SHAFTS = 6;

        private static readonly Log _log = new Log("LaboratoryBuilder");
    }
}