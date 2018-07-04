using UnityEngine;

namespace WorldObjects.WorldGeneration
{
    public class HazardLoader : Singleton<HazardLoader>
    {
        public GameObject SpikeHazard;

        public static Hazard CreateHazard(HazardTypes type, IntVector2 worldPosition)
        {
            GameObject hazardObject;

            switch (type)
            {
                case HazardTypes.Spike: hazardObject = Instance.SpikeHazard; break;
                default: throw new System.ArgumentException($"Unknown hazard type of {type}.");
            }

            hazardObject = Instantiate(hazardObject);
            hazardObject.transform.position = worldPosition;
            hazardObject.name = $"[{worldPosition.X}, {worldPosition.Y}]";

            var hazard = hazardObject.GetComponent<Hazard>();

            Log.ErrorIfNull(hazard, $"Hazard of type {type} has not been given a 'hazard' component.");

            return hazard;
        }
    }
}