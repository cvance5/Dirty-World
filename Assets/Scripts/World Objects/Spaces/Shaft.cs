namespace WorldObjects.Spaces
{
    public class Shaft : Space
    {
        public override bool IsHazardous => false;

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

        public override BlockTypes GetBlock(IntVector2 position)
        {
            BlockTypes block = BlockTypes.None;

            if (position.Y == _topRightCorner.Y)
            {
                if (Chance.CoinFlip)
                {
                    block = BlockTypes.Stone;
                }
                else
                {
                    block = BlockTypes.Dirt;
                }
            }

            return block;
        }

        public override HazardTypes GetHazard(IntVector2 position) => HazardTypes.None;
    }
}