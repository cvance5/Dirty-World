using Data;
using System.Collections;
using UnityEngine;
using WorldObjects;
using WorldObjects.Blocks;

namespace Actors.Player.Guns
{
    public class HarpoonGun : Gun
    {
        public bool IsAttached => _attachedObject != null;

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        protected int _damage;
        [SerializeField]
        protected int _force;
        [SerializeField]
        protected float _range;

        [SerializeField]
        private float _minLength = .5f;
        [SerializeField]
        private float _maxLength = 10;

        [SerializeField]
        private float _pullForce = 100f;
        [SerializeField]
        [Range(0, 10)]
        private float _selfPullRatio = .5f;

        [SerializeField]
        private float _climbSpeed = 0;

        [SerializeField]
        private PlayerMovement _playerMovement = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private LineRenderer _renderer;
        private DistanceJoint2D _joint;
        private WorldObject _attachedObject;

        protected override void OnAwake()
        {
            _renderer = GetComponent<LineRenderer>();
            _renderer.enabled = false;

            _joint = _playerMovement.gameObject.AddComponent<DistanceJoint2D>();
            _joint.maxDistanceOnly = true;
            _joint.enableCollision = true;
            _joint.enabled = false;
        }

        private void Update()
        {
            if (IsAttached)
            {
                var ropeLengthChange = Input.GetAxis("Vertical");

                if (ropeLengthChange != 0)
                {
                    ChangeLength(ropeLengthChange * _climbSpeed * Time.deltaTime);
                }
            }
        }

        public override void Fire()
        {
            if (IsAttached)
            {
                Detach(_attachedObject);
            }
            else
            {
                LaunchHarpoon();
            }
        }

        public override void AlternateFire()
        {
            RetractHarpoon();
        }

        private void LaunchHarpoon()
        {
            var targetVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            targetVector.z = 0;
            targetVector = targetVector.normalized;

            if (Physics2D.Raycast(transform.position, targetVector, _filter, _hits, _range) == 0)
            {
                DrawLine(transform.position + (targetVector * _range));
                return;
            }

            var hit = _hits[0];

            if (hit.collider != null)
            {
                var hittable = (IHittable)hit.collider.GetComponent(typeof(IHittable));
                if (hittable != null)
                {
                    hittable.Hit(_damage, _force);
                }

                var worldObject = hit.collider.GetComponent<WorldObject>();
                if (worldObject != null)
                {
                    Attach(worldObject);
                }
                else
                {
                    DrawLine(hit.point);
                }
            }
        }

        public void ChangeLength(float ropeLengthChange)
        {
            if (IsAttached)
            {
                _joint.distance = Mathf.Clamp(ropeLengthChange + _joint.distance, _minLength, _maxLength);

                if(ropeLengthChange > 0)
                {
                    _joint.attachedRigidbody.AddForce(Vector2.down * Time.deltaTime);
                }
                else
                {
                    _joint.attachedRigidbody.AddForce(Vector2.up * Time.deltaTime);
                }
            }
        }

        private void RetractHarpoon()
        {
            if (IsAttached)
            {
                var pullDirection = transform.position - _attachedObject.Position;
                var pullVector = pullDirection.normalized * _pullForce;

                _playerMovement.Impulse(-pullVector * _selfPullRatio);

                if (_attachedObject is IImpulsable)
                {
                    var impulsableObject = _attachedObject as IImpulsable;
                    impulsableObject.Impulse(pullVector);
                }
            }
        }

        private void Attach(WorldObject target)
        {
            target.OnWorldObjectDestroyed += Detach;
            _attachedObject = target;

            StartCoroutine(AttachmentUpdate());

            if (_attachedObject is Block)
            {
                var attachedBlock = _attachedObject as Block;
                attachedBlock.CanStabalize = false;

                _joint.enabled = true;
                _joint.connectedBody = _attachedObject.GetComponent<Rigidbody2D>();
                _joint.distance = Vector2.Distance(transform.position, target.Position);
            }
        }

        private IEnumerator AttachmentUpdate()
        {
            while (IsAttached)
            {
                DrawLine(_attachedObject.transform.position);
                yield return null;
            }

            BreakLine();
        }

        private void Detach(WorldObject target)
        {
            target.OnWorldObjectDestroyed -= Detach;

            if (_attachedObject is Block)
            {
                var attachedBlock = _attachedObject as Block;
                attachedBlock.CanStabalize = true;
            }

            _attachedObject = null;
            _joint.enabled = false;
            _joint.connectedBody = null;
        }

        private void DrawLine(Vector3 target)
        {
            _renderer.SetPosition(0, transform.position);
            _renderer.SetPosition(1, target);
            _renderer.enabled = true;

            if (!IsAttached)
            {
                Timekeeper.SetTimer(.1f, BreakLine);
            }
        }

        private void BreakLine()
        {
            _renderer.enabled = false;
        }
    }
}