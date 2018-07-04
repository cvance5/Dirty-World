namespace WorldObjects.Spaces
{
    public class Corridor : Space
    {
        private bool _isHazardous;
        public override bool IsHazardous => _isHazardous;

        private IntVector2 _bottomLeftCorner;
        private IntVector2 _topRightCorner;

        private int _spikeY;

        public Corridor(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, bool isHazardous)
        {
            _bottomLeftCorner = bottomLeftCorner;
            _topRightCorner = topRightCorner;

            Extents.Add(_bottomLeftCorner);
            Extents.Add(_topRightCorner);

            Name = $"Corridor from {_bottomLeftCorner} to {_topRightCorner}.";

            _isHazardous = isHazardous;

            if (_isHazardous)
            {
                _spikeY = _bottomLeftCorner.Y;
            }
        }

        public override bool Contains(IntVector2 position) =>
            !(position.X < _bottomLeftCorner.X ||
            position.Y < _bottomLeftCorner.Y ||
            position.X > _topRightCorner.X ||
            position.Y > _topRightCorner.Y);

        public override BlockTypes GetBlock(IntVector2 position) => BlockTypes.None;
        public override HazardTypes GetHazard(IntVector2 position)
        {
            HazardTypes hazard = HazardTypes.None;

            if (position.Y == _spikeY)
            {
                hazard = HazardTypes.Spike;
            }

            return hazard;
        }
    }
}
