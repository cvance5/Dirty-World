using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class ElevatorShaftBuilder : ShaftBuilder
    {
        public override bool IsValid => base.IsValid && _storyHeight >= _minStoryHeight &&
                                        _numberOfStories >= _minNumberOfStories && _elevatorBuilder.IsValid;

        private readonly ElevatorBuilder _elevatorBuilder = new ElevatorBuilder();

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

            SetElevatorSpawn(Chance.Range(0, _height - 1));

            _elevatorBuilder.SetRail(_bottom, _top);
            FillLandings();
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
            else
            {
                var nearestStoryToTarget = (blockHigh / _storyHeight);
                var storyDifference = nearestStoryToTarget - _numberOfStories;
                _numberOfStories += storyDifference;
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

        public ElevatorShaftBuilder SetElevatorSpawn(int elevatorSpawnHeight)
        {
            _elevatorBuilder.SetSpawnPosition(new IntVector2(_middle.X, elevatorSpawnHeight));

            OnSpaceBuilderChanged.Raise(this);

            return this;
        }

        public ElevatorShaftBuilder SetAllowLandings(bool isAllowed)
        {
            _allowLandings = isAllowed;
            if (!_allowLandings)
            {
                ClearLandings();
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
                _elevatorBuilder.RegisterStop(new IntVector2(_middle.X, _bottom.X + (_storyHeight * story)));

                OnSpaceBuilderChanged.Raise(this);
            }

            return this;
        }

        public ElevatorShaftBuilder FillLandings()
        {
            for (var story = 0; story < _numberOfStories; story++)
            {
                AddLanding(story);
            }

            return this;
        }

        public ElevatorShaftBuilder ClearLandings()
        {
            _landings.Clear();
            _elevatorBuilder.ClearStops();

            OnSpaceBuilderChanged.Raise(this);

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

        private Room BuildLanding(int story, RoomBuilder landingBuilder)
        {
            var startingPoint = new IntVector2(_middle.X, _bottom.Y + (_storyHeight * story));

            landingBuilder.SetSize(_width + 2)
                          .Align(Directions.Down, startingPoint.Y + 1)
                          .Align(Directions.Left, startingPoint.X - 1)
                          .AddModifiers(_modifiersApplied);

            return landingBuilder.Build() as Room;
        }

        protected override Spaces.Space BuildRaw()
        {
            var landings = new List<Room>();

            foreach (var kvp in _landings)
            {
                var landing = BuildLanding(kvp.Key, kvp.Value);
                landings.Add(landing);
            }

            var shaft = new ElevatorShaft(_bottom, new IntVector2(_top.X + _width, _top.Y), _isUncapped, landings);
            shaft.AddFeatureBuilder(_elevatorBuilder);
            return shaft;
        }

        protected override void Rebuild()
        {
            _height = _storyHeight * _numberOfStories;

            _landings.RemoveAll(landing => landing.Key >= _numberOfStories);

            base.Rebuild();

            _elevatorBuilder.SetRail(_bottom, _top);
        }
    }
}