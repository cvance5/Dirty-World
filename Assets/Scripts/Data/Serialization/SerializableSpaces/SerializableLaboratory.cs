using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public class SerializableLaboratory : SerializableSpace
    {
        [JsonProperty("spaces")]
        private readonly List<Region> _regions;

        [JsonProperty("metalThickness")]
        private readonly int _metalThickness;

        [JsonConstructor]
        private SerializableLaboratory() { }

        public SerializableLaboratory(Laboratory laboratory)
        {
            _regions = laboratory.Regions;
            _metalThickness = laboratory.MetalThickness;
        }

        protected override Space ToRawObject() => new Laboratory(_regions, _metalThickness);
    }
}