using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Debug;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class PlexusBuilder : SpaceBuilder
    {
        public override bool IsValid => Origin != null && _tunnels.Count > 0 && _coreWidth > 0 && _coreLength > 0 && _tunnels[Offset.IDENTITY].IsValid;

        private int _coreWidth;
        private int _coreLength;
        private int _maxOffshootLength;
        private readonly int _offshootWidth;
        private Quaternion _coreRotation;
        private Quaternion _offshootRotation;

        private Dictionary<Offset, TunnelBuilder> _tunnels = new Dictionary<Offset, TunnelBuilder>();

        public PlexusBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            var origin = new IntVector2(Chance.Range(chunkBuilder.BottomLeftCorner.X, chunkBuilder.TopRightCorner.X),
                                        Chance.Range(chunkBuilder.BottomLeftCorner.Y, chunkBuilder.TopRightCorner.Y));

            SetOrigin(origin);

            _coreLength = Chance.Range(7, 20);
            _coreWidth = Chance.Range(2, _coreLength / 2);
            _coreRotation = Quaternion.Euler(0, 0, Chance.Range(0, 180));
            _offshootRotation = _coreRotation * Quaternion.Euler(0, 0, 90);
            _maxOffshootLength = Chance.Range(3, _coreLength);
            _offshootWidth = Chance.Range(1, _coreWidth);

            UpdateOffsetKeys();
            Recalculate();

            var numberOfOffshoots = Chance.Range(0, _coreLength);
            for (var offshootCount = 0; offshootCount < numberOfOffshoots; offshootCount++)
            {
                AddRandomOffshoot();
            }

            Recalculate();
        }

        public PlexusBuilder SetOrigin(IntVector2 origin)
        {
            Origin = origin;

            return this;
        }

        public PlexusBuilder SetTunnelWidth(int width)
        {
            _coreWidth = width;

            Recalculate();

            return this;
        }

        public PlexusBuilder SetCoreRotation(float rotation)
        {
            _coreRotation = Quaternion.Euler(0, 0, rotation);
            _offshootRotation = _coreRotation * Quaternion.Euler(0, 0, 90);

            Recalculate();

            return this;
        }

        public PlexusBuilder SetCoreLength(int length)
        {
            _coreLength = length;

            Recalculate();

            return this;
        }

        public PlexusBuilder SetMaxOffshootLength(int maxLength)
        {
            _maxOffshootLength = maxLength;

            Recalculate();

            return this;
        }

        public PlexusBuilder AddRandomOffshoot()
        {
            Offset offset = null;
            foreach (var kvp in _tunnels.RandomOrder())
            {
                if (kvp.Value == null)
                {
                    offset = kvp.Key;
                    break;
                }
            }

            if (offset == null)
            {
                _log.Warning($"No unused offsets for this plexus. Cannot add offshoot.");
            }
            else
            {
                var tunnel = new TunnelBuilder(_chunkBuilder);
                AppendOffshoot(offset, tunnel);
            }

            return this;
        }

        public PlexusBuilder AppendOffshoot(Offset offset, TunnelBuilder tunnel)
        {
            if (offset == Offset.IDENTITY) throw new System.InvalidOperationException($"Cannot append over the core tunnel.");

            _tunnels[offset] = tunnel;

            tunnel.SetOrigin(Origin + new IntVector2(_coreRotation * offset.Position))
                  .TrimToLength(_maxOffshootLength)
                  .SetWidth(_offshootWidth)
                  .SetRotation(_offshootRotation);

            // If this is on the bottom, it needs to face the other way
            if (offset.Position.Y == 0)
            {
                tunnel.SetRotation(_offshootRotation * Quaternion.Euler(0, 0, 180));
            }

            if (tunnel.IsValid)
            {
                foreach (var direction in Directions.Cardinals)
                {
                    if (tunnel.DistanceFrom(direction, _maximalValues[direction]) > 0)
                    {
                        _maximalValues[direction] = tunnel.MaximalValues[direction];
                    }
                }

                tunnel.OnSpaceBuilderChanged += OnTunnelBuilderChanged;
            }
            else
            {
                _tunnels[offset] = null;
            }

            return this;
        }

        public override bool Contains(IntVector2 point)
        {
            // Outside of max bounds, definitely doesn't contain
            if (point.Y > _maximalValues[Directions.Up] ||
               point.X > _maximalValues[Directions.Right] ||
               point.Y < _maximalValues[Directions.Down] ||
               point.X < _maximalValues[Directions.Left])
            {
                return false;
            }

            // Otherwise, ask the tunnels
            return _tunnels.Any(kvp => kvp.Value.Contains(point));
        }

        public override void Squash(IntVector2 direction, int amount)
        {
            foreach (var kvp in _tunnels.ToArray())
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Squash(direction, amount);

                    if (kvp.Key == Offset.IDENTITY)
                    {
                        Origin = kvp.Value.Origin;
                        _coreLength = kvp.Value.Length;
                        _coreWidth = kvp.Value.Width;
                        _coreRotation = kvp.Value.Rotation;
                    }
                    else if (!kvp.Value.IsValid)
                    {
                        _tunnels[kvp.Key] = null;
                    }
                }
            }

            Recalculate();
        }

        protected override Spaces.Space BuildRaw()
        {
            var shapes = new List<Shape>();

            foreach (var tunnel in _tunnels.Values)
            {
                if (tunnel != null)
                {
                    var space = tunnel.Build();
                    shapes.AddRange(space.Extents.Shapes);
                }
            }

            return new Spaces.Space($"Plexus {Name}", new Extents(shapes));
        }

        protected override void Recalculate()
        {
            _tunnels[Offset.IDENTITY] = new TunnelBuilder(_chunkBuilder)
                                           .SetOrigin(Origin)
                                           .SetLength(_coreLength)
                                           .SetWidth(_coreWidth)
                                           .SetRotation(_coreRotation);

            foreach (var direction in Directions.Cardinals)
            {
                _maximalValues[direction] = _tunnels[Offset.IDENTITY].MaximalValues[direction];
            }

            var tunnelCount = 0;
            foreach (var kvp in _tunnels.ToArray())
            {
                // Make sure the top offshoots are at the right offset
                if (kvp.Key.Position.Y != 0)
                {
                    kvp.Key.Position.Y = _coreWidth;
                }

                // This offset is no longer valid, kill it
                if (kvp.Key.Position.X > _coreLength ||
                    kvp.Key.Position.X < 0)
                {
                    _tunnels.Remove(kvp.Key);
                    if (kvp.Value != null) kvp.Value.OnSpaceBuilderChanged -= OnTunnelBuilderChanged;
                }
                // Count and validate this tunnel if any
                else if (kvp.Key != Offset.IDENTITY && kvp.Value != null)
                {
                    // Revalidate this tunnel
                    AppendOffshoot(kvp.Key, kvp.Value);

                    if (_tunnels[kvp.Key] != null)
                    {
                        tunnelCount++;
                    }
                }
            }

            UpdateOffsetKeys();

            OnSpaceBuilderChanged.Raise(this);
        }

        private void OnTunnelBuilderChanged(SpaceBuilder tunnelBuilder)
        {
            // Don't report changes to invalid tunnels, they will be deleted soon
            if (tunnelBuilder.IsValid)
            {
                OnSpaceBuilderChanged.Raise(this);
            }
        }

        private void UpdateOffsetKeys()
        {
            for (var pointAlongLenfth = 0; pointAlongLenfth < _coreLength; pointAlongLenfth += _offshootWidth)
            {
                var bottomOffset = new Offset(pointAlongLenfth, 0, 90);
                if (!_tunnels.ContainsKey(bottomOffset))
                {
                    _tunnels[bottomOffset] = null;
                }
                var topOffset = new Offset(pointAlongLenfth, _coreWidth, -90);
                if (!_tunnels.ContainsKey(topOffset))
                {
                    _tunnels[topOffset] = null;
                }
            }
        }

        private static readonly Log _log = new Log("PlexusBuilder");
    }
}