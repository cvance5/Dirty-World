using System.Collections;
using UnityEngine;
using WorldObjects.Blocks;

namespace WorldObjects.Actors.Player.Guns
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
        private float _projectileSpeed = 150f;

        [SerializeField]
        private float _pullForce = 100f;
        [SerializeField]
        [Range(0, 10)]
        private float _selfPullRatio = .5f;

        [SerializeField]
        private float _climbSpeed = 0;
#pragma warning restore IDE0044 // Add readonly modifier

        private PlayerMovement _playerMovement;
        private LineRenderer _renderer;
        private DistanceJoint2D _joint;
        private Vector3 _attachementOffset;
        private WorldObject _attachedObject;

        protected override void OnGunAwake()
        {
            _renderer = GetComponent<LineRenderer>();
            _renderer.enabled = false;
        }

        public void Initialize(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;

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

        public override void AlternateFire() => PullHarpoon();

        private void LaunchHarpoon()
        {
            var targetVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            targetVector.z = 0;
            targetVector = targetVector.normalized;

            if (Physics2D.Raycast(transform.position, targetVector, _filter, _hits, _range) == 0)
            {
                StartCoroutine(ExtendLine(transform.position + (targetVector * _range)));
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
                    Attach(worldObject, hit.point);
                }
                StartCoroutine(ExtendLine(hit.point));
            }
        }

        public void ChangeLength(float ropeLengthChange)
        {
            if (IsAttached)
            {
                _joint.distance = Mathf.Clamp(ropeLengthChange + _joint.distance, _minLength, _maxLength);

                if (ropeLengthChange > 0)
                {
                    _joint.attachedRigidbody.AddForce(Vector2.down * Time.deltaTime);
                }
                else
                {
                    _joint.attachedRigidbody.AddForce(Vector2.up * Time.deltaTime);
                }
            }
        }

        private void PullHarpoon()
        {
            if (IsAttached)
            {
                var pullDirection = transform.position - ((Vector3)_attachedObject.Position + _attachementOffset);
                var pullVector = pullDirection.normalized * _pullForce;

                _playerMovement.Impulse(-pullVector * _selfPullRatio);

                if (_attachedObject is IImpulsable)
                {
                    var impulsableObject = _attachedObject as IImpulsable;
                    impulsableObject.Impulse(pullVector);
                }
            }
        }

        private void Attach(WorldObject target, Vector3 hitPosition)
        {
            target.OnWorldObjectDestroyed += Detach;
            _attachedObject = target;
            _attachementOffset = hitPosition - _attachedObject.Position;

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
                DrawLine(_attachedObject.transform.position + _attachementOffset);
                yield return null;
            }

            StartCoroutine(RetractLine());
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
            _attachementOffset = Vector3.zero;
            _joint.enabled = false;
            _joint.connectedBody = null;
        }

        private IEnumerator ExtendLine(Vector3 target)
        {
            var initialPosition = transform.position;
            var currentPosition = initialPosition;
            _renderer.enabled = true;

            var timePassed = 0f;
            var distance = Vector3.Distance(initialPosition, target);
            var percentOfTheWayThere = 0f;

            while (currentPosition != target)
            {
                currentPosition = Vector3.Lerp(initialPosition, target, percentOfTheWayThere);
                DrawLine(currentPosition);
                timePassed += Time.deltaTime;
                percentOfTheWayThere = (timePassed * _projectileSpeed) / distance;

                yield return null;
            }

            if (!IsAttached)
            {
                StartCoroutine(RetractLine());
            }
            else StartCoroutine(AttachmentUpdate());
        }

        private IEnumerator RetractLine()
        {
            var initialPosition = _renderer.GetPosition(1);
            var currentPosition = initialPosition;

            var timePassed = 0f;
            var distance = Vector3.Distance(initialPosition, transform.position);
            var percentOfTheWayThere = 0f;

            while (currentPosition != transform.position)
            {
                currentPosition = Vector3.Lerp(initialPosition, transform.position, percentOfTheWayThere);
                DrawLine(currentPosition);
                timePassed += Time.deltaTime;
                percentOfTheWayThere = (timePassed * _projectileSpeed) / distance;

                yield return null;
            }

            _renderer.enabled = false;
        }

        private void DrawLine(Vector3 target)
        {
            _renderer.SetPosition(0, transform.position);
            _renderer.SetPosition(1, target);
        }
    }
}