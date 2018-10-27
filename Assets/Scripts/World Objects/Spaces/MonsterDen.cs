using WorldObjects.Blocks;
using WorldObjects.Hazards;

namespace WorldObjects.Spaces
{
    public class MonsterDen : Space
    {
        public override bool IsHazardous => true;

        public IntVector2 Center { get; }
        public int Radius { get; }

        public override bool Contains(IntVector2 position) => throw new System.NotImplementedException();
        public override BlockTypes GetBlock(IntVector2 position) => throw new System.NotImplementedException();
        public override HazardTypes GetHazard(IntVector2 position) => throw new System.NotImplementedException();
    }
}