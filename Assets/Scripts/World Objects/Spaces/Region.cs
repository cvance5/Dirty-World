using System.Collections.Generic;

namespace WorldObjects.Spaces
{
    public class Region
    {
        public IntVector2 BottomLeftCorner { get; }
        public IntVector2 TopRightCorner { get; }

        public List<Space> Spaces { get; }

        public Region(IntVector2 bottomLeftCorner, IntVector2 topRightCorner, List<Space> spaces)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;
            Spaces = spaces;
        }

        public bool Contains(IntVector2 position) =>
            position.X >= BottomLeftCorner.X &&
            position.Y >= BottomLeftCorner.Y &&
            position.X <= TopRightCorner.X &&
            position.Y <= TopRightCorner.Y;
    }
}