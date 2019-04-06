using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public class SerializableLaboratory : SerializableSpace
    {
        [JsonProperty("spaces")]
        private readonly List<SerializableRegion> _regions;

        [JsonProperty("metalThickness")]
        private readonly int _metalThickness;

        [JsonConstructor]
        private SerializableLaboratory() { }

        public SerializableLaboratory(Laboratory laboratory)
            :base(laboratory)
        {
            var regions = new List<SerializableRegion>();
            foreach(var region in laboratory.Regions)
            {
                regions.Add(new SerializableRegion(region));
            }
            _regions = regions;
            _metalThickness = laboratory.MetalThickness;
        }

        protected override Space ToRawObject()
        {
            var regions = new List<Region>();
            foreach(var region in _regions)
            {
                regions.Add(region.ToObject());
            }
            return new Laboratory(regions, _metalThickness);
        }
    }
}