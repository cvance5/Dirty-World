using UnityEngine;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace WorldObjects.Features
{
    public class WallLight : Feature, IPowerable
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private Light _light = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public bool IsPowered { get; private set; }
        public override string ObjectName => $"Wall Light at {Position}";

        public override FeatureTypes Type { get; } = FeatureTypes.WallLight;

        protected override void OnWorldObjectAwake() => _light.enabled = IsPowered;

        public void AddPower()
        {
            IsPowered = true;
            _light.enabled = true;
        }

        public void RemovePower()
        {
            IsPowered = false;
            _light.enabled = false;
        }
    }
}