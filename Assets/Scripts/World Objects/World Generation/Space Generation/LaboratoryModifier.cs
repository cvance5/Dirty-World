using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.PropGeneration;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class LaboratoryModifier : SpaceModifier
    {
        public override ModifierTypes Type => ModifierTypes.Laboratory;

        public LaboratoryModifier(SpaceBuilder spaceBuilder)
            : base(spaceBuilder) { }

        public override void Modify(Space target)
        {
            if (target is Corridor)
            {
                ModifyCorridor(target as Corridor);
            }
        }

        private void ModifyCorridor(Corridor corridor)
        {
            var lightY = corridor.TopRightCorner.Y - 1;

            for (var lightX = corridor.BottomLeftCorner.X; lightX <= corridor.TopRightCorner.X; lightX++)
            {
                if (lightX % 8 == 0)
                {
                    corridor.AddProp(new IntVector2(lightX, lightY), PropTypes.WallLight);
                }
            }
        }
    }
}