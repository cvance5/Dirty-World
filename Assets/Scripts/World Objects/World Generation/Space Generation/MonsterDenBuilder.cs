using System;
using Random = UnityEngine.Random;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class MonsterDenBuilder : SpaceBuilder
    {
        private readonly IntVector2 _centerpoint;

        private readonly int _radius;

        private static int _chunkSize => GameManager.Instance.Settings.ChunkSize;

        public MonsterDenBuilder(ChunkBuilder containingChunk)
            : base(containingChunk)
        {
            _centerpoint = new IntVector2(Random.Range(_containingChunk.BottomLeft.X, _containingChunk.TopRight.X),
                                          Random.Range(_containingChunk.BottomLeft.Y, _containingChunk.TopRight.Y));

            _radius = Random.Range(5, _chunkSize / 2);
        }

        public override Spaces.Space Build() => throw new System.NotImplementedException();
        public override SpaceBuilder Clamp(IntVector2 direction, int amount)
        {
            if (direction == Directions.Up)
            {
                if (_centerpoint.Y + _radius > amount)
                {
                    var overlap = (_centerpoint.Y + _radius) - amount;
                    _centerpoint.Y -= overlap;
                }
            }
            else if (direction == Directions.Right)
            {
                if (_centerpoint.X + _radius > amount)
                {
                    var overlap = (_centerpoint.X + _radius) - amount;
                    _centerpoint.X -= overlap;
                }
            }
            else if (direction == Directions.Down)
            {
                if (_centerpoint.Y - _radius < amount)
                {
                    var overlap = (_centerpoint.Y - _radius) - amount;
                    _centerpoint.Y += overlap;
                }
            }
            else if (direction == Directions.Left)
            {
                if (_centerpoint.X - _radius < amount)
                {
                    var overlap = (_centerpoint.X - _radius) - amount;
                    _centerpoint.X += overlap;
                }
            }
            else throw new ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            return this;
        }
    }
}