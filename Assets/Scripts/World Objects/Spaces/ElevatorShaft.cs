using System.Collections.Generic;

namespace WorldObjects.Spaces
{
    public class ElevatorShaft : Shaft
    {
        private readonly List<Room> _landings = new List<Room>();

        public ElevatorShaft(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, bool isUncapped, List<Room> landings)
            : base(bottomLeftCorner, topRightCorner, isUncapped) => _landings = landings;
    }
}