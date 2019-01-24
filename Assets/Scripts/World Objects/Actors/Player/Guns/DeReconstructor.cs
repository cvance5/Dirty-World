using System.Collections;
using UnityEngine;

namespace WorldObjects.Actors.Player.Guns
{
    public class DeReconstructor : Gun
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private int _range = 0;
        [SerializeField]
        private int _damagePerHit = 0;
        [SerializeField]
        private int _hitsPerSecond = 0;
        [SerializeField]
        private int _maxStoredMatter = 0;
#pragma warning restore IDE0044 // Add readonly modifier

        private int _storedMatter = 0;

        private PolygonCollider2D _targetingCollider = null;
        private TriggerList2D _targetedObjectsList = null;

        private DeReconstructorMode _mode = DeReconstructorMode.Deconstructing;
        private Coroutine _fireCouroutine = null;

        protected override void OnGunAwake()
        {
            _targetedObjectsList = GetComponent<TriggerList2D>();
            _targetingCollider = GetComponent<PolygonCollider2D>();

            var conePoints = _targetingCollider.points;
            conePoints[0].Set(_range / 2, _range);
            conePoints[1].Set(-_range / 2, _range);
        }

        private void Update()
        {
            if (_fireCouroutine != null)
            {
                if ((Input.GetButtonUp("Alternate Fire") && _mode == DeReconstructorMode.Reconstructing) ||
                   (Input.GetButtonUp("Fire") && _mode == DeReconstructorMode.Deconstructing))
                {
                    StopFiring();
                }
            }
        }

        public override void Fire()
        {
            if (_storedMatter != _maxStoredMatter)
            {
                _mode = DeReconstructorMode.Deconstructing;

                if (_fireCouroutine == null)
                {
                    StartFiring();
                }
            }
        }

        public override void AlternateFire()
        {
            if (_storedMatter != 0)
            {
                _mode = DeReconstructorMode.Reconstructing;

                if (_fireCouroutine == null &&
                    _storedMatter != 0)
                {
                    StartFiring();
                }
            }
        }

        private void StartFiring() => _fireCouroutine = StartCoroutine(TickHits());

        private void StopFiring()
        {
            StopCoroutine(_fireCouroutine);
            _fireCouroutine = null;
        }

        private IEnumerator TickHits()
        {
            var wfs = new WaitForSeconds(1f / _hitsPerSecond);

            while (true)
            {
                Vector2 targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.up = targetPoint - (Vector2)transform.position;

                var targets = _targetedObjectsList.Overlaps;

                foreach (var target in targets)
                {
                    var hittable = (IHittable)target.GetComponent(typeof(IHittable));
                    if (hittable != null) ApplyHit(hittable);
                }

                if ((_storedMatter == _maxStoredMatter && _mode == DeReconstructorMode.Deconstructing) ||
                   (_storedMatter == 0 && _mode == DeReconstructorMode.Reconstructing))
                {
                    StopFiring();
                }

                yield return wfs;
            }
        }

        private void ApplyHit(IHittable target)
        {
            if (_mode == DeReconstructorMode.Deconstructing)
            {
                target.Hit(_damagePerHit, 0);
                _storedMatter += _damagePerHit;
            }
            else if (_mode == DeReconstructorMode.Reconstructing)
            {
                target.Hit(-_damagePerHit, 0);
                _storedMatter -= _damagePerHit;
            }

            _storedMatter = Mathf.Clamp(_storedMatter, 0, _maxStoredMatter);
        }

        private enum DeReconstructorMode
        {
            Deconstructing,
            Reconstructing
        }
    }
}