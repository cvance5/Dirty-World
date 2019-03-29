using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.FeatureGeneration;

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
            else if (target is Shaft)
            {
                ModifyShaft(target as Shaft);
            }
        }

        private void ModifyCorridor(Corridor corridor)
        {
            var lightY = corridor.TopRightCorner.Y - 1;

            for (var lightX = corridor.BottomLeftCorner.X; lightX <= corridor.TopRightCorner.X; lightX++)
            {
                if (lightX % 8 == 0)
                {
                    corridor.AddFeature(new IntVector2(lightX, lightY), FeatureTypes.WallLight);
                }
            }
        }

        private void ModifyShaft(Shaft shaft)
        {
            var randomElevatorHeight = Chance.Range(0, shaft.Height - 6);
            var randomElevatorPosition = shaft.BottomLeftCorner + (Directions.Up * randomElevatorHeight);

            shaft.AddFeature(randomElevatorPosition, FeatureTypes.Elevator);
        }
    }
}