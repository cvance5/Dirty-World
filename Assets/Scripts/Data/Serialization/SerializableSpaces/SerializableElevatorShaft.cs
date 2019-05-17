using MathConcepts;
using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public class SerializableElevatorShaft : SerializableTunnel
    {
        [JsonProperty("landings")]
        private readonly List<IntVector2> _landings = new List<IntVector2>();

        [JsonConstructor]
        private SerializableElevatorShaft() { }

        public SerializableElevatorShaft(ElevatorShaft shaft)
            : base(shaft) => _landings = shaft.Landings;

        protected override Space ToRawObject() => new ElevatorShaft(_bottomLeftCorner, _topRightCorner, _landings);
    }
}