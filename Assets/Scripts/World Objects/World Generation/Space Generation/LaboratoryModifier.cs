using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class LaboratoryModifier : SpaceModifier
    {
        public LaboratoryModifier(ChunkBuilder chunkBuilder, SpaceBuilder spaceBuilder)
            : base(chunkBuilder, spaceBuilder) { }

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
                    corridor.AddFeature(new IntVector2(lightX, lightY), FeatureTypes.WallLight);
                }
            }
        }
    }
}