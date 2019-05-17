using MathConcepts;
using System.Collections.Generic;
using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class Tunnel : Space
    {
        public override string Name => $"Tunnel from {BottomLeftCorner} to {TopRightCorner}";
        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Height => TopRightCorner.Y - BottomLeftCorner.Y;
        public int Width => TopRightCorner.X - BottomLeftCorner.X;

        public Tunnel(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;

            if (BottomLeftCorner.X > TopRightCorner.X ||
               BottomLeftCorner.Y > TopRightCorner.Y)
            {
                throw new System.ArgumentException($"{Name} has impossible extents.  The bottom left corner is {BottomLeftCorner} and the top right corner is {TopRightCorner}.");
            }

            Extents.AddShape(new List<IntVector2>()
            {
                BottomLeftCorner,
                new IntVector2(bottomLeftCorner.X, topRightCorner.Y),
                TopRightCorner,
                new IntVector2(topRightCorner.X, bottomLeftCorner.Y)
            });
        }

        public override BlockTypes GetBlockType(IntVector2 position)
        {
            _blockOverride.TryGetValue(position, out var result);
            return result;
        }

        public override IntVector2 GetRandomPosition() => new IntVector2(
            Chance.Range(BottomLeftCorner.X, TopRightCorner.X),
            Chance.Range(BottomLeftCorner.Y, TopRightCorner.Y));
    }
}