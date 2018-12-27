using System.Collections.Generic;

namespace WorldObjects.Spaces
{
    public abstract class ComplexSpace : Space
    {
        public List<Space> ContainedSpaces = new List<Space>();
    }
}