using System.Collections.Generic;
using WorldObjects.Features;

namespace WorldObjects.Spaces
{
    public class ElevatorShaft : Shaft
    {
        public override string Name => $"Elevator {base.Name}";

        private readonly List<Room> _landings = new List<Room>();

        public ElevatorShaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, bool isUncapped, List<Room> landings)
            : base(bottomLeftCorner, topRightCorner, isUncapped)
        {
            _landings = landings;
        }
    }
}