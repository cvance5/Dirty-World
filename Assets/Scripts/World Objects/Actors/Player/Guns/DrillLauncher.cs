using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors.Player.Guns
{
    public class DrillLauncher : Gun
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private GameObject _droneActor = null;
        [SerializeField]
        private int _maxDrones = 0;
#pragma warning restore IDE0044 // Add readonly modifier

        private readonly List<DrillDrone> _activeDrones = new List<DrillDrone>();

        public override void Fire()
        {
            if (_activeDrones.Count < _maxDrones)
            {
                LaunchDrone();
            }
        }

        public override void AlternateFire()
        {
            if (_activeDrones.Count > 0)
            {
                ExplodeDrones();
            }
        }

        private void LaunchDrone()
        {
            Vector2 launchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var droneObject = Instantiate(_droneActor, transform.position, Quaternion.identity);
            droneObject.transform.right = launchPoint - (Vector2)transform.position;

            var drillDrone = droneObject.GetComponent<DrillDrone>();
            drillDrone.OnDroneLifespanEnded += OnDroneLifespanEnded;

            _activeDrones.Add(drillDrone);
        }

        private void OnDroneLifespanEnded(DrillDrone drone)
        {
            drone.OnDroneLifespanEnded -= OnDroneLifespanEnded;
            _activeDrones.Remove(drone);
        }

        private void ExplodeDrones()
        {
            foreach (var drone in _activeDrones)
            {
                drone.Explode();
            }

            _activeDrones.Clear();
        }
    }
}