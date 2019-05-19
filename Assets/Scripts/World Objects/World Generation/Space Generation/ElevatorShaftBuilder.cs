using MathConcepts;
using System.Collections.Generic;
using UnityEngine;
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
        private readonly List<int> _landings = new List<int>();

        public ElevatorShaftBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _storyHeight = Chance.Range(3, 7);
            _numberOfStories = Chance.Range(2, 7);

            Rebuild();

            SetElevatorSpawn(Chance.Range(0, _height - 1));
            _elevatorBuilder.SetPlatformSize(_width + 1);
            _elevatorBuilder.SetRail(_bottom, _top);

            FillLandings();

            OnSpaceBuilderChanged.Raise(this);
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

        public override ShaftBuilder SetWidth(int blocksWide)
        {
            _elevatorBuilder.SetPlatformSize(blocksWide + 1);
            return base.SetWidth(blocksWide);
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
            _elevatorBuilder.SetSpawnPosition(new IntVector2(_middle.X, _bottom.Y + elevatorSpawnHeight));

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

            var height = story * _storyHeight;
            if (!_landings.Contains(height))
            {
                _landings.Add(story);
                _elevatorBuilder.RegisterStop(height);
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

            return this;
        }

        protected override Spaces.Space BuildRaw()
        {
            var landings = new List<IntVector2>();
            foreach (var landing in _landings)
            {
                landings.Add(new IntVector2(_middle.X, _bottom.Y + landing));
            }

            var shaft = base.BuildRaw();
            shaft.Name = $"Elevator {shaft.Name}";
            shaft.AddFeatureBuilder(_elevatorBuilder);
            return shaft;
        }

        protected override void Rebuild()
        {
            _height = _storyHeight * _numberOfStories;

            _landings.RemoveAll(landing => landing % _storyHeight != 0 || landing >= _height);

            base.Rebuild();

            _elevatorBuilder.SetRail(_bottom, _top);
        }
    }
}