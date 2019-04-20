using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Construction;
using WorldObjects.Features;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace Data.Serialization.SerializableFeatures
{
    public class SerializableElevator : SerializableFeature
    {
        [JsonProperty("stops")]
        private readonly List<IntVector2> _stops = new List<IntVector2>();

        public SerializableElevator(Elevator elevator)
            : base(elevator) => _stops = elevator.Stops;

        public override Feature ToObject()
        {
            var elevator = FeatureLoader.CreateFeature(FeatureTypes.Elevator, _position) as Elevator;
            elevator.Initialize(_stops);
            return elevator;
        }
    }
}