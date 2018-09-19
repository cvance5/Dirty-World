using System.Collections.Generic;
using UnityEngine;

namespace Actors.Player.GunActors
{
    public class SeismicBombLauncher : GunActor
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private GameObject _bombActor;
        [SerializeField]
        private int _maxBombs;
#pragma warning restore IDE0044 // Add readonly modifier

        private readonly List<SeismicBomb> _activeBombs = new List<SeismicBomb>();

        protected override void OnAwake()
        {

        }

        public override void Fire()
        {
            if (_activeBombs.Count < _maxBombs)
            {
                LaunchBomb();
            }
        }

        public override void AlternateFire()
        {
            if (_activeBombs.Count > 0)
            {
                ExplodeBombs();
            }
        }

        private void LaunchBomb()
        {
            Vector3 throwForce = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);

            GameObject bombObject = Instantiate(_bombActor, transform);

            SeismicBomb seismicBomb = bombObject.GetComponent<SeismicBomb>();
            seismicBomb.Ignite(_force, _range);

            Rigidbody2D bombRigidbody = bombObject.GetComponent<Rigidbody2D>();
            bombRigidbody.AddForce(throwForce, ForceMode2D.Impulse);

            _activeBombs.Add(seismicBomb);
        }

        private void ExplodeBombs()
        {
            foreach (SeismicBomb activeBomb in _activeBombs)
            {
                activeBomb.Explode();
            }

            _activeBombs.Clear();
        }
    }
}