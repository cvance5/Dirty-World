namespace WorldObjects.Blocks
{
    public class SteelPlateBlock : Block
    {
        public override string ObjectName => $"Steel Plate {Position}";
        public override BlockTypes Type => BlockTypes.SteelPlate;
    }
}