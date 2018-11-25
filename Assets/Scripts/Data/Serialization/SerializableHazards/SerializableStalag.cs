using Newtonsoft.Json;
using WorldObjects.Hazards;
using WorldObjects.WorldGeneration;

namespace Data.Serialization.SerializableHazards
{
    public class SerializableStalag : SerializableHazard
    {
        [JsonProperty("anchorPosition")]
        private readonly IntVector2 _anchorPosition;

        [JsonProperty("facingDirection")]
        private readonly IntVector2 _facingDirection;

        [JsonProperty("numSegments")]
        private readonly int _numSegments;

        [JsonConstructor]
        public SerializableStalag() { }

        public SerializableStalag(StalagHazard stalag)
        {
            _anchorPosition = stalag.Position;
            _facingDirection = new IntVector2(stalag.transform.up);
            _numSegments = stalag.NumSegments;
        }

        public override Hazard ToObject()
        {
            var stalag = HazardLoader.CreateHazard(HazardTypes.Stalag, _anchorPosition) as StalagHazard;
            stalag.Initialize(_facingDirection, _numSegments);
            return stalag;
        }
    }
}