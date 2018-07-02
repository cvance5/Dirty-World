namespace WorldObjects.Spaces
{
    public class Corridor : Space
    {
        private IntVector2 _bottomLeftCorner;
        private IntVector2 _topRightCorner;

        public Corridor(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            _bottomLeftCorner = bottomLeftCorner;
            _topRightCorner = topRightCorner;

            Name = $"{_bottomLeftCorner} to {_topRightCorner}.";
        }

        public override bool Contains(IntVector2 position) =>
            position.X >= _bottomLeftCorner.X &&
            position.Y >= _bottomLeftCorner.Y &&
            position.X <= _topRightCorner.X &&
            position.Y <= _topRightCorner.Y;

        public override Block GetBlock(IntVector2 position) => null;
    }
}
