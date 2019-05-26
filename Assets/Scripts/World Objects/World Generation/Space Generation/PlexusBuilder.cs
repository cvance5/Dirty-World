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
        public override bool IsValid => _origin != null && _tunnels.Count > 0;

        private int _tunnelWidth;
        private int _coreLength;
        private int _maxOffshootLength;
        private Quaternion _coreRotation;
        private Quaternion _offshootRotation;

        private Dictionary<Offset, TunnelBuilder> _tunnels = new Dictionary<Offset, TunnelBuilder>();

        public PlexusBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            var origin = new IntVector2(Chance.Range(chunkBuilder.BottomLeftCorner.X, chunkBuilder.TopRightCorner.X),
                                        Chance.Range(chunkBuilder.BottomLeftCorner.Y, chunkBuilder.TopRightCorner.Y));

            SetOrigin(origin);

            _coreLength = Chance.Range(3, 10);
            _tunnelWidth = Chance.Range(2, 6);
            _coreRotation = Quaternion.Euler(0, 0, Chance.Range(0, 180));

            UpdateOffsetKeys();

            var numberOfOffshoots = Chance.Range(0, _coreLength);
            for(int offshootCount = 0; offshootCount < numberOfOffshoots; offshootCount++)
            {
                AddRandomOffshoot();
            }

            Recalculate();
        }

        public PlexusBuilder SetOrigin(IntVector2 origin)
        {
            _origin = origin;

            return this;
        }

        public PlexusBuilder SetTunnelWidth(int width)
        {
            _tunnelWidth = width;

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

            tunnel.SetOrigin(_origin + offset.Position)
                  .TrimToLength(_maxOffshootLength)
                  .SetWidth(_tunnelWidth)
                  .SetRotation(_coreRotation * _offshootRotation);

            foreach (var direction in Directions.Cardinals)
            {
                if (tunnel.PassesBy(direction, _maximalValues[direction]) > 0)
                {
                    _maximalValues[direction] = tunnel.MaximalValues[direction];
                }
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

        public override void Cut(IntVector2 direction, int amount)
        {
            foreach (var tunnel in _tunnels.Values)
            {
                tunnel.Cut(direction, amount);
            }
        }

        protected override Spaces.Space BuildRaw()
        {
            var shapes = new List<Shape>();

            foreach (var tunnel in _tunnels.Values)
            {
                var space = tunnel.Build();
                shapes.AddRange(space.Extents.Shapes);
            }

            return new Spaces.Space($"Plexus {SpaceNamer.GetName()}", new Extents(shapes));
        }

        protected override void Recalculate()
        {
            _tunnels[Offset.IDENTITY] = new TunnelBuilder(_chunkBuilder)
                                           .SetOrigin(_origin)
                                           .SetLength(_coreLength)
                                           .SetWidth(_tunnelWidth)
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
                    kvp.Key.Position.Y = _tunnelWidth;
                }

                // This offset is no longer valid, kill it
                if (kvp.Key.Position.X > _coreLength ||
                    kvp.Key.Position.X < 0)
                {
                    _tunnels.Remove(kvp.Key);
                }
                // Count and validate this tunnel
                else
                {
                    tunnelCount++;
                    // Revalidate this tunnel
                    AppendOffshoot(kvp.Key, kvp.Value);
                }
            }

            UpdateOffsetKeys();

            OnSpaceBuilderChanged.Raise(this);
        }

        private void UpdateOffsetKeys()
        {
            for (var length = 0; length < _coreLength; length++)
            {
                var bottomOffset = new Offset(length, 0, 90);
                if (!_tunnels.ContainsKey(bottomOffset))
                {
                    _tunnels[bottomOffset] = null;
                }
                var topOffset = new Offset(length, _tunnelWidth, -90);
                if (!_tunnels.ContainsKey(topOffset))
                {
                    _tunnels[topOffset] = null;
                }
            }
        }

        private static readonly Log _log = new Log("PlexusBuilder");
    }
}