using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public class SerializableLaboratory : SerializableSpace
    {
        [JsonProperty("spaces")]
        private readonly List<Space> _spaces;

        [JsonConstructor]
        private SerializableLaboratory() { }

        public SerializableLaboratory(Laboratory laboratory) => _spaces = laboratory.ContainedSpaces;

        public override Space ToObject() => new Laboratory(_spaces);
    }
}