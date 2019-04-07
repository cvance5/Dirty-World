using UnityEngine;

namespace WorldObjects.Actors.Player
{
    public class PlayerMovement : MonoBehaviour, IImpulsable
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private float _movementSpeed = 10f;
        [SerializeField]
        private float _maximumSpeed = 20f;
        public float MaximumSpeed => _maximumSpeed;

        [SerializeField]
        private float _jumpForce = 10f;

        [SerializeField]
        private float _jetpackForce = 2f;
        [SerializeField]
        private int _jetpackCost = 1;
        [SerializeField]
        private float _jetpackCostTick = .2f;

        [SerializeField]
        private PlayerHealth _health = null;
        [SerializeField]
        private ActorFeet _feet = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private float _lastJetpackTick = 0;

        private ActorFeetState _state;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            _feet.OnFootTouch += OnLand;
            _feet.OnFootLeave += OnAirborne;

            if (_feet.IsColliding) _state = ActorFeetState.Grounded;
            else _state = ActorFeetState.Airborne;

            _health.OnActorDeath += OnPlayerDeath;
        }

        private void Update()
        {
            HorizontalMovementUpdate();
            JumpUpdate();
        }

        private void HorizontalMovementUpdate()
        {
            var movementVector = new Vector2()
            {
                x = Input.GetAxis("Horizontal"),
            };

            AddForce(movementVector * _movementSpeed * Time.deltaTime);
        }

        private void JumpUpdate()
        {
            if (_state == ActorFeetState.Grounded)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    AddImpulse(Vector2.up * _jumpForce);
                }
            }
            else if (_state == ActorFeetState.Airborne)
            {
                if (Input.GetButton("Jetpack"))
                {
                    AddForce(Vector2.up * _jetpackForce);
                    if (Time.time > _lastJetpackTick + _jetpackCostTick)
                    {
                        _health.ApplyDeduct(_jetpackCost);
                        _lastJetpackTick = Time.time;
                    }
                }
            }
        }

        public void Impulse(Vector2 impulseForce)
        {
            if (_state == ActorFeetState.Airborne)
            {
                AddImpulse(impulseForce);
            }
        }

        private void AddForce(Vector2 force)
        {
            var currentForce = _rigidbody.velocity + force;

            if (currentForce.magnitude > _maximumSpeed)
            {
                currentForce = (_maximumSpeed * currentForce.normalized);
            }

            _rigidbody.velocity = currentForce;
        }

        private void AddImpulse(Vector2 impulse) => _rigidbody.velocity += impulse;

        private void OnLand() => _state = ActorFeetState.Grounded;

        private void OnAirborne() => _state = ActorFeetState.Airborne;

        private void OnPlayerDeath(ActorHealth playerData)
        {
            playerData.OnActorDeath -= OnPlayerDeath;
            Destroy(_feet);
            Destroy(this);
        }
    }
}