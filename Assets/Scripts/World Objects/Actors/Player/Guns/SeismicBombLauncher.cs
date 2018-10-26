using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors.Player.Guns
{
    public class SeismicBombLauncher : Gun
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private GameObject _bombActor = null;
        [SerializeField]
        private int _maxBombs = 0;
        [SerializeField]
        protected float _range;
#pragma warning restore IDE0044 // Add readonly modifier

        private readonly List<SeismicBomb> _activeBombs = new List<SeismicBomb>();

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
            var throwVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);

            var bombObject = Instantiate(_bombActor, transform.position, Quaternion.identity);

            var seismicBomb = bombObject.GetComponent<SeismicBomb>();
            var bombRigidbody = bombObject.GetComponent<Rigidbody2D>();
            bombRigidbody.AddForce(throwVector, ForceMode2D.Impulse);

            _activeBombs.Add(seismicBomb);
        }

        private void ExplodeBombs()
        {
            foreach (var activeBomb in _activeBombs)
            {
                activeBomb.Explode();
            }

            _activeBombs.Clear();
        }
    }
}