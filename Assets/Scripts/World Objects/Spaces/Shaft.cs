﻿namespace WorldObjects.Spaces
{
    public class Shaft : Space
    {
        private IntVector2 _bottomLeftCorner;
        private IntVector2 _topRightCorner;

        public Shaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            _bottomLeftCorner = bottomLeftCorner;
            _topRightCorner = topRightCorner;

            Extents.Add(_bottomLeftCorner);
            Extents.Add(_topRightCorner);

            Name = $"Shaft from {_bottomLeftCorner} to {_topRightCorner}.";
        }

        public override bool Contains(IntVector2 position) =>
            !(position.X < _bottomLeftCorner.X ||
            position.Y < _bottomLeftCorner.Y ||
            position.X > _topRightCorner.X ||
            position.Y > _topRightCorner.Y);

        public override Block GetBlock(IntVector2 position)
        {
            Block block = null;

            if (position.Y == _topRightCorner.Y)
            {
                if (Chance.CoinFlip)
                {
                    block = BlockLoader.CreateBlock(BlockTypes.Stone, position).GetComponent<Block>();
                }
                else
                {
                    block = BlockLoader.CreateBlock(BlockTypes.Dirt, position).GetComponent<Block>();
                }
            }

            return block;
        }
    }
}