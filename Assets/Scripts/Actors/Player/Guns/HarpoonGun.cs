using UnityEngine;

namespace Actors.Player.Guns
{
    public class HarpoonGun : Gun
    {
        private LineRenderer _renderer;

        protected override void OnAwake()
        {
            _renderer = GetComponent<LineRenderer>();
            _renderer.enabled = false;
        }

        public override void Fire()
        {
            var targetVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            targetVector.z = 0;
            targetVector = targetVector.normalized;

            if (Physics2D.Raycast(transform.position, targetVector, _filter, _hits, _range) == 0)
            {
                LineToTarget(transform.position + (targetVector * _range));
                return;
            }

            if (_hits[0].collider != null)
            {
                var hittable = (IHittable)_hits[0].collider.GetComponent(typeof(IHittable));

                if (hittable != null)
                {
                    hittable.Hit(_damage, _force);
                }

                LineToTarget(_hits[0].point);
            }
        }

        private void LineToTarget(Vector3 target)
        {
            _renderer.SetPosition(0, transform.position);
            _renderer.SetPosition(1, target);
            _renderer.enabled = true;
            Timekeeper.SetTimer(.1f, () => _renderer.enabled = false);
        }
    }
}