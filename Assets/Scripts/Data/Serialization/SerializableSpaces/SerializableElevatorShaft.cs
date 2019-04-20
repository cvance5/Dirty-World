using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public class SerializableElevatorShaft : SerializableShaft
    {
        [JsonProperty("landings")]
        private readonly List<SerializableRoom> _landings = new List<SerializableRoom>();

        [JsonConstructor]
        private SerializableElevatorShaft() { }

        public SerializableElevatorShaft(ElevatorShaft shaft)
            : base(shaft)
        {
            foreach (var landing in shaft.Landings)
            {
                _landings.Add(new SerializableRoom(landing));
            }
        }

        protected override Space ToRawObject()
        {
            var landings = new List<Room>();
            foreach (var serializedLanding in _landings)
            {
                landings.Add(serializedLanding.ToObject() as Room);
            }

            return new ElevatorShaft(_bottomLeftCorner, _topRightCorner, _isUncapped, landings);
        }
    }
}