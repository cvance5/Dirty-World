using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class ElevatorShaftBuilder : ShaftBuilder
    {
        public override bool IsValid => base.IsValid && _storyHeight >= _minStoryHeight &&
                                        _numberOfStories >= _minNumberOfStories && _elevatorHeight < _height;

        private int _elevatorHeight;

        private int _storyHeight;
        private int _minStoryHeight = 1;
        private int _numberOfStories;
        private int _minNumberOfStories = 1;

        private bool _allowLandings = true;
        private readonly Dictionary<int, RoomBuilder> _landings
                   = new Dictionary<int, RoomBuilder>();

        public ElevatorShaftBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _storyHeight = Chance.Range(3, 7);
            _numberOfStories = Chance.Range(2, 7);

            Rebuild();

            SetElevatorSpawnHeight(Chance.Range(0, _height - 1));
        }

        public override ShaftBuilder SetHeight(int blockHigh)
        {
            // If the new height just needs a different number of stories
            if (blockHigh % _storyHeight == 0)
            {
                _numberOfStories = blockHigh / _storyHeight;
            }
            // Else if the new height just needs a different story size
            else if (blockHigh % _numberOfStories == 0)
            {
                _storyHeight = blockHigh / _numberOfStories;
            }

            // If this height does not cleanly scale one of the other
            // fields, it will be reset in the rebuild
            _height = blockHigh;
            Rebuild();

            return this;
        }

        public ElevatorShaftBuilder SetStoryHeight(int newStoryHeight)
        {
            _storyHeight = Mathf.Max(0, newStoryHeight);
            Rebuild();
            return this;
        }

        public ElevatorShaftBuilder SetMinimumStoryHeight(int newMinStoryHeight)
        {
            _minStoryHeight = newMinStoryHeight;
            return this;
        }

        public ElevatorShaftBuilder SetNumberOfStories(int newNumberOfStories)
        {
            _numberOfStories = Mathf.Max(0, newNumberOfStories);
            Rebuild();
            return this;
        }

        public ElevatorShaftBuilder SetMinimumNumberOfStories(int newMinNumberOfStories)
        {
            _minNumberOfStories = newMinNumberOfStories;
            return this;
        }

        public ElevatorShaftBuilder SetElevatorSpawnHeight(int elevatorSpawnHeight)
        {
            _elevatorHeight = Mathf.Clamp(elevatorSpawnHeight, 0, _height - 1);
            return this;
        }

        public ElevatorShaftBuilder SetAllowLandings(bool isAllowed)
        {
            _allowLandings = isAllowed;
            if (!_allowLandings)
            {
                _landings.Clear();
            }

            return this;
        }

        public ElevatorShaftBuilder AddLanding(int story)
        {
            if (!_allowLandings)
            {
                throw new System.ArgumentException($"This elevator shaft does not allow landings!");
            }

            if (GetLanding(story) == null)
            {
                var roomBuilder = new RoomBuilder(_chunkBuilder);
                _landings.Add(story, roomBuilder);
            }

            return this;
        }

        public ElevatorShaftBuilder FillLandings()
        {
            for (var story = 0; story < _numberOfStories; story++)
            {
                if (GetLanding(story) == null)
                {
                    AddLanding(story);
                }
            }

            return this;
        }

        public ElevatorShaftBuilder ClearLandings()
        {
            _landings.Clear();

            return this;
        }

        public RoomBuilder GetLanding(int story)
        {
            if (story < 0 || story >= _numberOfStories)
            {
                throw new System.ArgumentException($"This elevator shaft doesn't have a story `{story}`.");
            }
            else
            {
                _landings.TryGetValue(story, out var landing);
                return landing;
            }
        }

        protected override Spaces.Space BuildRaw()
        {
            var landings = new List<Room>();

            foreach (var kvp in _landings)
            {
                var startingPoint = new IntVector2(_middle.X + (_width / 2), _storyHeight * kvp.Key);

                var landingBuilder = kvp.Value
                                    .SetCenter(startingPoint)
                                    .SetSize(_storyHeight)
                                    .AddModifiers(_modifiersApplied);

                var landing = landingBuilder.Build() as Room;
                landings.Add(landing);
            }

            var shaft = new ElevatorShaft(_bottom, new IntVector2(_top.X + _width, _top.Y), _isUncapped, landings);
            shaft.AddFeature(new IntVector2(_middle.X, _elevatorHeight), FeatureGeneration.FeatureTypes.Elevator);
            return shaft;
        }

        protected override void Rebuild()
        {
            _height = _storyHeight * _numberOfStories;

            _landings.RemoveAll(landing => landing.Key >= _numberOfStories);

            base.Rebuild();
        }
    }
}