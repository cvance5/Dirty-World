using MathConcepts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Debug;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class PlexusBuilder : SpaceBuilder
    {
        public override bool IsValid => _gridScalar > 0 & _gridWidth > 0 && _gridHeight > 0;

        private IntVector2 _origin;

        private int _gridScalar;
        private int _gridWidth;
        private int _gridHeight;

        private readonly SortedDictionary<int, Range> _horizontalGrid =
                     new SortedDictionary<int, Range>();

        private readonly SortedDictionary<int, Range> _verticalGrid =
                     new SortedDictionary<int, Range>();

        public PlexusBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            var startingPoint = new IntVector2(Chance.Range(chunkBuilder.BottomLeftCorner.X, chunkBuilder.TopRightCorner.X),
                                               Chance.Range(chunkBuilder.BottomLeftCorner.Y, chunkBuilder.TopRightCorner.Y));

            SetStartingPoint(startingPoint);

            _gridScalar = Chance.Range(2, 6) * 4;
            _gridWidth = Chance.Range(2, 4);
            _gridHeight = Chance.Range(2, 4);

            do
            {
                var xPosition = Chance.Range(0, _gridWidth);

                var yMin = Chance.Range(0, _gridHeight);
                var yMax = yMin + Chance.Range(1, 5);

                AddVerticalTunnel(xPosition, new Range(yMin, yMax));

            } while (_verticalGrid.Count < _gridWidth && Chance.OneIn(_verticalGrid.Count));

            do
            {
                var yPosition = Chance.Range(0, _gridHeight);

                var xMin = Chance.Range(0, _gridWidth);
                var xMax = xMin + Chance.Range(1, 3);

                AddHorizontalTunnel(yPosition, new Range(xMin, xMax));

            } while (_horizontalGrid.Count < _gridHeight && Chance.OneIn(_horizontalGrid.Count));
        }

        public PlexusBuilder SetStartingPoint(IntVector2 startingPoint)
        {
            _origin = startingPoint;
            return this;
        }

        public PlexusBuilder SetGridScalar(int gridScalar)
        {
            _gridScalar = gridScalar;
            return this;
        }

        public PlexusBuilder SetGridHeight(int gridHeight)
        {
            _gridHeight = gridHeight;

            Clamp(Directions.Up, _origin.Y + (_gridHeight * _gridScalar));

            foreach (var xPosition in _verticalGrid.Keys.ToArray())
            {
                var tunnelRange = _verticalGrid[xPosition];

                if (tunnelRange.Max > gridHeight)
                {
                    tunnelRange.Max = gridHeight;
                }

                if (tunnelRange.Min > gridHeight)
                {
                    _verticalGrid.Remove(xPosition);
                }
                else if (tunnelRange.Size <= 0)
                {
                    _verticalGrid.Remove(xPosition);
                }
            }

            foreach (var yPosition in _horizontalGrid.Keys.ToArray())
            {
                if (yPosition > gridHeight)
                {
                    _horizontalGrid.Remove(yPosition);
                }
            }

            return this;
        }

        public PlexusBuilder SetGridWidth(int gridWidth)
        {
            _gridWidth = gridWidth;

            Clamp(Directions.Right, _origin.X + (_gridWidth * _gridScalar));

            foreach (var yPosition in _horizontalGrid.Keys.ToArray())
            {
                var tunnelRange = _horizontalGrid[yPosition];

                if (tunnelRange.Max > _gridWidth)
                {
                    tunnelRange.Max = _gridWidth;
                }

                if (tunnelRange.Min > _gridWidth)
                {
                    _horizontalGrid.Remove(yPosition);
                }
                else if (tunnelRange.Size <= 0)
                {
                    _horizontalGrid.Remove(yPosition);
                }
            }

            foreach (var xPosition in _verticalGrid.Keys.ToArray())
            {
                if (xPosition > _gridWidth)
                {
                    _verticalGrid.Remove(xPosition);
                }
            }

            return this;
        }

        public PlexusBuilder AddHorizontalTunnel(int yPosition, Range xRange)
        {
            if (yPosition < 0 || yPosition > _gridHeight)
            {
                _log.Warning($"YPosition did not fall within grid, clamping.");
                yPosition = Mathf.Clamp(yPosition, 0, _gridHeight);
            }

            if (xRange.Min < 0 || xRange.Min > _gridWidth)
            {
                _log.Warning($"XRange Minimum did not fall within grid, clamping.");
                xRange.Min = Mathf.Clamp(xRange.Min, 0, _gridWidth);
            }

            if (xRange.Max < 0 || xRange.Max > _gridWidth)
            {
                _log.Warning($"XRange Maximum did not fall within grid, clamping.");
                xRange.Max = Mathf.Clamp(xRange.Max, 0, _gridWidth);
            }

            if (xRange.Size <= 0)
            {
                _log.Warning($"Horizontal tunnel has 0 width, not saving.");
            }
            else
            {
                _horizontalGrid[yPosition] = xRange;
            }

            return this;
        }

        public PlexusBuilder AddVerticalTunnel(int xPosition, Range yRange)
        {
            if (xPosition < 0 || xPosition > _gridWidth)
            {
                _log.Warning($"XPosition did not fall within grid, clamping.");
                xPosition = Mathf.Clamp(xPosition, 0, _gridWidth);
            }

            if (yRange.Min < 0 || yRange.Min > _gridHeight)
            {
                _log.Warning($"YRange Minimum did not fall within grid, clamping.");
                yRange.Min = Mathf.Clamp(yRange.Min, 0, _gridHeight);
            }

            if (yRange.Max < 0 || yRange.Max > _gridHeight)
            {
                _log.Warning($"YRange Maximum did not fall within grid, clamping.");
                yRange.Max = Mathf.Clamp(yRange.Max, 0, _gridHeight);
            }

            if (yRange.Size <= 0)
            {
                _log.Warning($"Vertical tunnel has 0 height, not saving.");
            }
            else
            {
                _verticalGrid[xPosition] = yRange;
            }

            return this;
        }

        public override SpaceBuilder Align(IntVector2 direction, int amount)
        {
            var maximalValue = GetMaximalValue(direction);
            var difference = amount - maximalValue;
            Shift(direction * difference);

            return this;
        }

        public override int GetMaximalValue(IntVector2 direction)
        {
            var maximalValue = 0;

            if (direction == Directions.Up)
            {
                maximalValue = (_origin.Y + (_gridHeight * _gridScalar));
            }
            else if (direction == Directions.Right)
            {
                maximalValue = (_origin.X + (_gridWidth * _gridScalar));
            }
            else if (direction == Directions.Down)
            {
                maximalValue = _origin.Y;
            }
            else if (direction == Directions.Left)
            {
                maximalValue = _origin.X;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            return maximalValue;
        }

        public override bool Contains(IntVector2 point)
        {
            if (point.X >= _origin.X &&
                point.X <= _origin.X + (_gridWidth * _gridScalar) &&
                point.Y >= _origin.Y &&
                point.Y <= _origin.Y + (_gridHeight * _gridScalar))
            {
                var gridPosition = new IntVector2((point.X - _origin.X) / _gridScalar, (point.Y - _origin.Y) / _gridScalar);

                if (_horizontalGrid.TryGetValue(gridPosition.X, out var horizontalTunnel) &&
                    horizontalTunnel.IsInRange(gridPosition.Y))
                {
                    return true;
                }
                if (_verticalGrid.TryGetValue(gridPosition.Y, out var verticalTunnel) &&
                    verticalTunnel.IsInRange(gridPosition.X))
                {
                    return true;
                }
            }

            return false;
        }

        public override IntVector2 GetRandomPoint()
        {
            var grid = Chance.CoinFlip ? _horizontalGrid : _verticalGrid;
            var tunnel = grid.RandomItem();
            var primaryPosition = tunnel.Key;
            var secondaryPosition = Chance.Range(tunnel.Value);

            var widthFudge = Chance.Range(0, _gridScalar);
            var heightFudge = Chance.Range(0, _gridScalar);

            return (grid == _horizontalGrid) ?
                   new IntVector2(_origin.X + (primaryPosition * _gridScalar) + widthFudge,
                                  _origin.Y + (secondaryPosition * _gridScalar) + heightFudge) :
                   new IntVector2(_origin.X + (secondaryPosition * _gridScalar) + heightFudge,
                                  _origin.Y + (primaryPosition * _gridScalar) + widthFudge);
        }

        public override int PassesBy(IntVector2 direction, int amount)
        {
            var difference = 0;

            if (direction == Directions.Up)
            {
                difference = (_origin.Y + (_gridHeight * _gridScalar)) - amount;
            }
            else if (direction == Directions.Right)
            {
                difference = (_origin.X + (_gridWidth * _gridScalar)) - amount;
            }
            else if (direction == Directions.Down)
            {
                difference = amount - _origin.Y;
            }
            else if (direction == Directions.Left)
            {
                difference = amount - _origin.X;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            return Mathf.Max(0, difference);
        }

        public override void Shift(IntVector2 shift) => _origin += shift;

        public override void Clamp(IntVector2 direction, int amount)
        {
            var difference = PassesBy(direction, amount);

            if (difference > 0)
            {
                Align(direction, amount);
            }
        }

        public override void Cut(IntVector2 direction, int amount)
        {
            var difference = PassesBy(direction, amount);

            if (difference > 0)
            {
                if (direction == Directions.Up)
                {
                    SetGridHeight((amount - _origin.Y) / _gridScalar);
                }
                else if (direction == Directions.Right)
                {
                    SetGridWidth((amount - _origin.X) / _gridScalar);
                }
                else if (direction == Directions.Down)
                {
                    // Pretend we chopped off the bottom by cutting off the top and moving up
                    Shift(Directions.Up * difference);
                    SetGridHeight(_gridHeight - (difference / _gridScalar));
                }
                else if (direction == Directions.Left)
                {
                    // Pretend we chopped off the left by cutting off the right and moving right
                    Shift(Directions.Right * difference);
                    SetGridWidth(_gridWidth - (difference / _gridScalar));
                }
                else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
            }
        }

        protected override Spaces.Space BuildRaw() => new Plexus(_origin, _horizontalGrid, _verticalGrid, _gridScalar);

        public enum PlexusOrientation
        {
            Horizontal,
            Vertical
        }

        private static readonly Log _log = new Log("PlexusBuilder");
    }
}