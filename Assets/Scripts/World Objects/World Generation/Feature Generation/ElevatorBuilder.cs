using MathConcepts;
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Construction;
using WorldObjects.Features;

namespace WorldObjects.WorldGeneration.FeatureGeneration
{
    public class ElevatorBuilder : FeatureBuilder
    {
        public override bool IsValid => _stops.Count > 0;

        public override IntVector2 Position { get; protected set; } = IntVector2.Zero;

        private int _platformWidth = 1;

        private IntVector2 _railStart = IntVector2.Zero;
        private IntVector2 _railEnd = IntVector2.Zero;

        private readonly List<int> _stops = new List<int>();

        public ElevatorBuilder RegisterStop(int distance)
        {
            if (IntVector2.Distance(_railStart, _railEnd) < distance)
            {
                throw new ArgumentException($"Cannot stop at point not on rail.");
            }
            else if (_stops.Contains(distance))
            {
                throw new InvalidOperationException($"Elevator already contains stop at {distance}.");
            }
            else _stops.Add(distance);

            return this;
        }

        public ElevatorBuilder ClearStops()
        {
            _stops.Clear();
            return this;
        }

        public ElevatorBuilder SetSpawnPosition(IntVector2 position)
        {
            if (!IntVector2.IsBetween(_railStart, _railEnd, position))
            {
                throw new ArgumentException($"Cannot spawn off of rail.");
            }
            else Position = position;

            return this;
        }

        public ElevatorBuilder SetRail(IntVector2 startRail, IntVector2 endRail)
        {
            _railStart = startRail;
            _railEnd = endRail;

            if (!IntVector2.IsBetween(startRail, endRail, Position))
            {
                Position = startRail;
            }

            foreach (var stopDistance in _stops.ToArray())
            {
                if (IntVector2.Distance(_railStart, _railEnd) < stopDistance)
                {
                    _stops.Remove(stopDistance);
                }
            }

            return this;
        }

        public ElevatorBuilder SetPlatformSize(int size)
        {
            _platformWidth = size;
            return this;
        }

        public override Feature Build()
        {
            var elevator = FeatureLoader.CreateFeature(FeatureTypes.Elevator, Position) as Elevator;

            var stops = new List<IntVector2>();
            foreach (var stopDistance in _stops)
            {
                stops.Add(_railStart + ((_railEnd - _railStart).Normalized * stopDistance));
            }
            elevator.Initialize(stops);

            var spriteRenderer = elevator.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.size = new Vector2(_platformWidth, spriteRenderer.size.y);

            return elevator;
        }
    }
}