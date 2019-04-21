using System.Collections.Generic;

namespace WorldObjects.Spaces
{
    public class ElevatorShaft : Shaft
    {
        public override string Name => $"Elevator {base.Name}";

        public readonly List<IntVector2> Landings = new List<IntVector2>();

        public ElevatorShaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, bool isUncapped, List<IntVector2> landings)
            : base(bottomLeftCorner, topRightCorner, isUncapped) => Landings = landings;
    }
}