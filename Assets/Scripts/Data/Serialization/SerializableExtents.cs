using MathConcepts.Geometry;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Data.Serialization
{
    public class SerializableExtents : ISerializable<Extents>
    {
        [JsonProperty("shapes")]
        private readonly List<Shape> _shapes;

        public SerializableExtents(Extents extents) => _shapes = extents.Shapes;

        public Extents ToObject() => new Extents(_shapes);
    }
}
