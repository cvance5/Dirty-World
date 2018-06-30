using System;
using UnityEngine;

namespace Player
{
    public class Gun : MonoBehaviour
    {
        public float Range;
        public int Damage;
        public int Force;

        private ContactFilter2D _filter;
        private RaycastHit2D[] _hits = new RaycastHit2D[1];

        private LineRenderer _bulletRenderer;

        private void Awake()
        {
            _filter = new ContactFilter2D();
            _filter.SetLayerMask(new LayerMask()
            {
                value = LayerMask.GetMask("WorldObject")
                      | LayerMask.GetMask("Enemy")
            });

            _bulletRenderer = GetComponent<LineRenderer>();
            _bulletRenderer.enabled = false;
        }

        public void Fire()
        {
            var targetVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            targetVector.z = 0;
            targetVector = targetVector.normalized;

            if (Physics2D.Raycast(transform.position, targetVector, _filter, _hits, Range) == 0)
            {
                LineToTarget(transform.position + (targetVector * Range));
                return;
            }

            if (_hits[0].collider != null)
            {
                var block = _hits[0].collider.GetComponent<Block>();

                if (block != null)
                {
                    block.OnHit(Damage, Force);
                }

                LineToTarget(_hits[0].point);
            }
        }

        private void LineToTarget(Vector3 target)
        {
            _bulletRenderer.SetPosition(0, transform.position);
            _bulletRenderer.SetPosition(1, target);
            _bulletRenderer.enabled = true;
            Timekeeper.SetTimer(.1f, () => _bulletRenderer.enabled = false);
        }
    }
}