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

        private readonly int _platformWidth = 1;

        private IntVector2 _railStart = IntVector2.Zero;
        private IntVector2 _railEnd = IntVector2.Zero;

        private readonly List<IntVector2> _stops = new List<IntVector2>();

        public ElevatorBuilder RegisterStop(IntVector2 stop)
        {
            if (!IntVector2.IsOnLine(_railStart, _railEnd, stop))
            {
                throw new ArgumentException($"Cannot stop at point not on rail.");
            }
            else if (_stops.Contains(stop))
            {
                throw new InvalidOperationException($"Elevator already contains stop {stop}.");
            }
            else _stops.Add(stop);

            return this;
        }

        public ElevatorBuilder ClearStops()
        {
            _stops.Clear();
            return this;
        }

        public ElevatorBuilder SetSpawnPosition(IntVector2 position)
        {
            if (!IntVector2.IsOnLine(_railStart, _railEnd, position))
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

            if (!IntVector2.IsOnLine(startRail, endRail, Position))
            {
                Position = startRail;
            }

            foreach (var stop in _stops.ToArray())
            {
                if (!IntVector2.IsOnLine(startRail, endRail, stop))
                {
                    _stops.Remove(stop);
                }
            }

            return this;
        }

        public override Feature Build()
        {
            var elevator = FeatureLoader.CreateFeature(FeatureTypes.Elevator, Position) as Elevator;
            elevator.Initialize(_stops);

            var spriteRenderer = elevator.GetComponent<SpriteRenderer>();
            spriteRenderer.size = new Vector2(_platformWidth, spriteRenderer.size.y);

            return elevator;
        }
    }
}