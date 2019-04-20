using System.Collections.Generic;

namespace WorldObjects.Spaces
{
    public class ElevatorShaft : Shaft
    {
        public override string Name => $"Elevator {base.Name}";

        public readonly List<Room> Landings = new List<Room>();

        public ElevatorShaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, bool isUncapped, List<Room> landings)
            : base(bottomLeftCorner, topRightCorner, isUncapped) => Landings = landings;
    }
}