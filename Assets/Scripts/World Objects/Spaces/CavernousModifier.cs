using WorldObjects.Blocks;
using WorldObjects.Hazards;

namespace WorldObjects.Spaces
{
    public class CavernousModifier : SpaceModifier
    {
        public override string Name => $"Cavernous {ModifiedSpace.Name}.";

        public override bool IsHazardous => true;
        protected override ModifierTypes _type => ModifierTypes.Cavernous;

        public CavernousModifier(Space modifiedSpace)
            : base(modifiedSpace) { }

        public override BlockTypes GetBlock(IntVector2 position)
        {
            var intendedBlock = ModifiedSpace.GetBlock(position);
            if (intendedBlock == BlockTypes.Dirt)
            {
                return BlockTypes.Stone;
            }
            else return intendedBlock;
        }

        public override HazardTypes GetHazard(IntVector2 position) => ModifiedSpace.GetHazard(position);
    }
}