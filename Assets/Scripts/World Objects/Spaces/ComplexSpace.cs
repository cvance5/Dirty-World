﻿using System.Collections.Generic;

namespace WorldObjects.Spaces
{
    public abstract class ComplexSpace : Space
    {
        public List<Region> Regions = new List<Region>();
    }
}