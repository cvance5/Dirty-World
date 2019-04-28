﻿using System.Collections.Generic;
using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class Corridor : Space
    {
        public override string Name => $"Corridor from {BottomLeftCorner} to {TopRightCorner}.";

        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Height => TopRightCorner.Y - BottomLeftCorner.Y;
        public int Length => TopRightCorner.X - BottomLeftCorner.X;

        public Corridor(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;

            Extents.AddShape(new List<IntVector2>()
            {
                BottomLeftCorner,
                new IntVector2(BottomLeftCorner.X, TopRightCorner.Y),
                TopRightCorner,
                new IntVector2(TopRightCorner.X, BottomLeftCorner.Y)
            });
        }

        public override IntVector2 GetRandomPosition() =>
            new IntVector2(Chance.Range(BottomLeftCorner.X, TopRightCorner.X),
                           Chance.Range(BottomLeftCorner.Y, TopRightCorner.Y));

        public override BlockTypes GetBlockType(IntVector2 position)
        {
            if (!Extents.Contains(position)) throw new System.ArgumentOutOfRangeException($"{Name} does not contain {position}.  Cannot get block.");
            else return _blockOverride.TryGetValue(position, out var type) ? type : BlockTypes.None;
        }
    }
}