using WorldObjects.Construction;
using WorldObjects.Hazards;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.HazardGeneration
{
    public class StalagBuilder : HazardBuilder
    {
        private readonly IntVector2 _anchorPosition;
        private readonly IntVector2 _facingDirection;

        private int _maxSegments = 1;

        public StalagBuilder(IntVector2 position, IntVector2 facingDirection, Space containingSpace)
            : base(position)
        {
            _anchorPosition = position - facingDirection;
            _facingDirection = facingDirection;
            _maxSegments = 1;
        }

        public override Hazard Build(Chunk containingChunk, Space containingSpace)
        {
            // If we cannot guarantee this is a safe place to build, don't
            if (!containingChunk.Contains(_anchorPosition) ||
                containingChunk.GetBlockForPosition(_anchorPosition) == null)
            {
                return null;
            }

            while (_maxSegments <= MAX_STALAG_SEGMENTS)
            {
                var nextSegmentLocation = Position + (_facingDirection * _maxSegments);
                if (!containingSpace.Contains(nextSegmentLocation) ||
                    !containingChunk.Contains(nextSegmentLocation) ||
                    containingSpace.GetBlockType(nextSegmentLocation) != Blocks.BlockTypes.None)
                {
                    break;
                }
                else _maxSegments++;
            }

            var stalag = HazardLoader.CreateHazard(HazardTypes.Stalag, Position) as StalagHazard;
            stalag.Initialize(_facingDirection, UnityEngine.Random.Range(1, _maxSegments));
            return stalag;
        }

        public const int MAX_STALAG_SEGMENTS = 4;
    }
}