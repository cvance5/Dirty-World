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
        public override bool IsValid => true;

        private readonly IntVector2 _mainShaftBottom;
        private readonly int _highestStory;
        private readonly int _maximumCorridorSegments;

        private readonly ShaftBuilder _mainShaft;
        private readonly TreasureRoomBuilder _treasureRoom;

        private readonly List<CorridorBuilder> _corridors;
        private readonly List<ShaftBuilder> _secondaryShafts;

        public LaboratoryBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _corridors = new List<CorridorBuilder>();
            _secondaryShafts = new List<ShaftBuilder>();

            _highestStory = Random.Range(6, 10);
            _maximumCorridorSegments = Random.Range(4, 8);

            _mainShaftBottom = new IntVector2(Random.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X + 1),
                                              Random.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y + 1));

            _mainShaft = AddShaft(0, _highestStory);

            GenerateCorridors();
            GenerateSecondaryShafts();
        }

        private void GenerateCorridors()
        {
            _corridors.Clear();

            var shaftLeftSide = _mainShaft.GetMaximalValue(Directions.Left);
            var shaftRightSide = _mainShaft.GetMaximalValue(Directions.Right);

            for (var story = 0; story < _highestStory; story++)
            {
                var yPosition = _mainShaftBottom.Y + (STORY_SIZE * story) + METAL_THICKNESS;

                if (Chance.CoinFlip)
                {
                    _corridors.Add(AddCorridor(new IntVector2(shaftLeftSide, yPosition), Directions.Left));
                }
                if (Chance.CoinFlip)
                {
                    _corridors.Add(AddCorridor(new IntVector2(shaftRightSide, yPosition), Directions.Right));
                }
            }
        }

        private void GenerateSecondaryShafts()
        {
            _secondaryShafts.Clear();

            for (var corridorSegment = -_maximumCorridorSegments; corridorSegment < _maximumCorridorSegments; corridorSegment++)
            {
                if (corridorSegment == 0) continue; // Main shaft...

                if (Chance.OneIn(5))
                {
                    var xOffset = (CORRIDOR_SEGMENT_LENGTH * corridorSegment);

                    var validStories = new List<int>();

                    for (var story = 0; story < _highestStory; story++)
                    {
                        var yPosition = _mainShaftBottom.Y + (STORY_SIZE * story) + METAL_THICKNESS;

                        var corridorPosition = new IntVector2(_mainShaftBottom.X + xOffset, yPosition);

                        if (_corridors.Any(corridor => corridor.Contains(corridorPosition)))
                        {
                            validStories.Add(story);
                        }
                    }

                    // At least two stories we can reach...
                    if (validStories.Count > 1)
                    {
                        var firstStory = validStories.RandomItem();
                        var secondStory = validStories.RandomItem(firstStory);

                        _secondaryShafts.Add(AddShaft(Mathf.Min(firstStory, secondStory), Mathf.Max(firstStory, secondStory), xOffset));
                    }
                }
            }
        }

        private SpaceBuilder AddTreasureRoom(IntVector2 newRoomStartingPoint, IntVector2 direction)
            => new TreasureRoomBuilder(_chunkBuilder)
                  .SetSize(ROOM_SIZE)
                  .SetCenter(newRoomStartingPoint + (direction * ROOM_SIZE))
                  .SetMinimumSize(ROOM_SIZE)
                  .SetTreasure(new UnlockItem("Seismic Bomb", UnlockTypes.Weapon));

        private CorridorBuilder AddCorridor(IntVector2 attachPoint, IntVector2 direction)
            => new CorridorBuilder(_chunkBuilder)
                  .SetStartingPoint(attachPoint, direction)
                  .SetHeight(CORRIDOR_HEIGHT)
                  .SetLength(CORRIDOR_SEGMENT_LENGTH * Random.Range(1, _maximumCorridorSegments))
                  .SetMinimumHeight(ROOM_SIZE)
                  .SetMinimumLength(ROOM_SIZE);

        private ShaftBuilder AddShaft(int bottomStory, int topStory, int xOffset = 0)
            => new ShaftBuilder(_chunkBuilder)
                  .SetStartingPoint(new IntVector2(_mainShaftBottom.X + xOffset, _mainShaftBottom.Y + (STORY_SIZE * bottomStory)), ShaftBuilder.ShaftAlignment.StartFromBottom)
                  .SetWidth(ROOM_SIZE)
                  .SetHeight(STORY_SIZE * (1 + (topStory - bottomStory)))
                  .SetMinimumWidth(ROOM_SIZE)
                  .SetMinimumHeight(ROOM_SIZE)
                  .SetUncapped(true);

        public override bool Contains(IntVector2 point) => _mainShaft.Contains(point) || _corridors.Any(room => room.Contains(point)) || _secondaryShafts.Any(shaft => shaft.Contains(point));
        public override int PassesBy(IntVector2 edge, int target) => _corridors.Max(room => room.PassesBy(edge, target));

        public override IntVector2 GetRandomPoint() => _corridors.RandomItem().GetRandomPoint();

        public override int GetMaximalValue(IntVector2 direction) => _corridors.Max(room => room.GetMaximalValue(direction));

        public override SpaceBuilder Align(IntVector2 direction, int amount) => this;

        public override void Clamp(IntVector2 edge, int maxAmount)
        {
        }

        public override void Cut(IntVector2 direction, int amount)
        {
        }

        public override void Shift(IntVector2 shift)
        {
        }

        protected override Space BuildRaw()
        {
            var rooms = new List<Space>
            {
                _mainShaft.Build()
            };

            foreach (var roomToBuild in _corridors)
            {
                rooms.Add(roomToBuild.Build());
            }

            foreach (var roomToBuild in _secondaryShafts)
            {
                rooms.Add(roomToBuild.Build());
            }

            return new Laboratory(rooms, METAL_THICKNESS);
        }


        private const int ROOM_SIZE = 4;
        private const int METAL_THICKNESS = 2;
        // A story is the size of a room, and the floor under it
        private const int STORY_SIZE = METAL_THICKNESS + ROOM_SIZE;

        private const int CORRIDOR_HEIGHT = ROOM_SIZE - METAL_THICKNESS;
        private const int CORRIDOR_SEGMENT_LENGTH = ROOM_SIZE + METAL_THICKNESS;

        private static readonly Log _log = new Log("LaboratoryBuilder");
    }
}