using System.Collections.Generic;

namespace WorldObjects.Spaces
{
    public class ElevatorShaft : Tunnel
    {
        public override string Name => $"Elevator {base.Name}";

        public readonly List<IntVector2> Landings = new List<IntVector2>();

        public ElevatorShaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, List<IntVector2> landings)
            : base(bottomLeftCorner, topRightCorner) => Landings = landings;
    }
}