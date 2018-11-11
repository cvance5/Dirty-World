using WorldObjects.Blocks;
using WorldObjects.Hazards;

namespace WorldObjects.Spaces
{
    public class Corridor : Space
    {
        public override string Name => $"Corridor from {BottomLeftCorner} to {TopRightCorner}.";

        private readonly bool _isHazardous;
        public override bool IsHazardous => _isHazardous;

        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public int Height => TopRightCorner.Y - BottomLeftCorner.Y;
        public int Length => TopRightCorner.X - BottomLeftCorner.X;

        private readonly int _spikeY;

        public Corridor(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, bool isHazardous)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;

            Extents.Add(BottomLeftCorner);
            Extents.Add(new IntVector2(BottomLeftCorner.X, TopRightCorner.Y));
            Extents.Add(TopRightCorner);
            Extents.Add(new IntVector2(TopRightCorner.X, BottomLeftCorner.Y));

            _isHazardous = isHazardous;

            if (_isHazardous)
            {
                _spikeY = BottomLeftCorner.Y;
            }
        }

        public override bool Contains(IntVector2 position) =>
            !(position.X < BottomLeftCorner.X ||
            position.Y < BottomLeftCorner.Y ||
            position.X > TopRightCorner.X ||
            position.Y > TopRightCorner.Y);

        public override BlockTypes GetBlock(IntVector2 position) => BlockTypes.None;
        public override HazardTypes GetHazard(IntVector2 position) => HazardTypes.None;
    }
}
