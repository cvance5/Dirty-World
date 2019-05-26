using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class TunnelBuilder : SpaceBuilder
    {
        public override bool IsValid => _origin != null && _length > _minLength && _width > _minWidth;

        private int _length;
        private int _minLength = 1;
        private int _width;
        private int _minWidth = 1;

        private Quaternion _rotation;

        private IntVector2 _offset;

        public TunnelBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _length = Chance.Range(4, 40);
            _width = Chance.Range(3, 6);

            _rotation = Quaternion.Euler(0, 0, Chance.Range(0, 359));

            _origin = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                     Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            Recalculate();
        }

        public TunnelBuilder SetLength(int length)
        {
            _length = length;

            Recalculate();

            return this;
        }

        public TunnelBuilder TrimToLength(int maximumLength)
        {
            if (_length > maximumLength)
            {
                _length = maximumLength;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetMinimumLength(int minLength)
        {
            _minLength = minLength;

            return this;
        }

        public TunnelBuilder SetWidth(int width)
        {
            if (_width != width)
            {
                _width = width;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetMinimumWidth(int minWidth)
        {
            _minWidth = minWidth;

            return this;
        }

        public TunnelBuilder SetRotation(float rotation)
        {
            var quatRotation = Quaternion.Euler(0, 0, rotation);

            if (_rotation != quatRotation)
            {
                _rotation = quatRotation;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetRotation(Quaternion rotation)
        {
            if (_rotation.eulerAngles.x != 0 ||
               _rotation.eulerAngles.y != 0)
            {
                throw new System.ArgumentOutOfRangeException($"Tunnels cannot rotate on any non-Z axis.");
            }
            else if (_rotation != rotation)
            {
                _rotation = rotation;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetOrigin(IntVector2 origin)
        {
            if (_origin != origin)
            {
                _origin = origin;

                Recalculate();
            }

            return this;
        }

        public override bool Contains(IntVector2 point) =>
            point.X >= _maximalValues[Directions.Left] &&
            point.X <= _maximalValues[Directions.Right] &&
            point.Y >= _maximalValues[Directions.Down] &&
            point.Y <= _maximalValues[Directions.Up];

        public override void Cut(IntVector2 direction, int amount)
        {
            var difference = DistanceFrom(direction, amount);

            if (difference > 0)
            {
                if (direction == Directions.Up)
                {
                    if (_origin.Y > _offset.Y)
                    {
                        _origin.Y = amount;
                    }
                    else _offset.Y = amount;
                }
                else if (direction == Directions.Right)
                {
                    if (_origin.X > _offset.X)
                    {
                        _origin.X = amount;
                    }
                    else _offset.X = amount;
                }
                else if (direction == Directions.Down)
                {
                    if (_origin.Y < _offset.Y)
                    {
                        _origin.Y = amount;
                    }
                    else _offset.Y = amount;
                }
                else if (direction == Directions.Left)
                {
                    if (_origin.X < _offset.X)
                    {
                        _origin.X = amount;
                    }
                    else _offset.X = amount;
                }
                else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

                var unrotatedOffset = new IntVector2(Quaternion.Inverse(_rotation) * (Vector2)_offset);

                _length = _origin.X - unrotatedOffset.X;
                _width = _origin.Y - unrotatedOffset.Y;

                Recalculate();
            }
        }

        protected override Spaces.Space BuildRaw()
        {
            var extents = new Extents(new Shape(new List<IntVector2>()
            {
                _origin,
                _origin + new IntVector2(_rotation * new Vector2(0, _width)),
                _origin + new IntVector2(_rotation * new Vector2(_length,_width)),
                _origin + new IntVector2(_rotation * new Vector2(_length, 0))
            }));

            return new Spaces.Space($"Tunnel {Name}", extents);
        }

        protected override void Recalculate()
        {
            _offset = _origin + new IntVector2(_rotation * new Vector2(_length, _width));

            _maximalValues[Directions.Up] = _offset.Y > _origin.Y ? _offset.Y : _origin.Y;
            _maximalValues[Directions.Right] = _offset.X > _origin.X ? _offset.X : _origin.X;
            _maximalValues[Directions.Down] = _offset.Y < _origin.Y ? _offset.Y : _origin.Y;
            _maximalValues[Directions.Left] = _offset.X < _origin.X ? _offset.X : _origin.X;

            OnSpaceBuilderChanged.Raise(this);
        }
    }
}