using UnityEngine;

namespace Actors.Player
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
#pragma warning restore IDE0044 // Add readonly modifier

        [SerializeField]
        private PlayerData _data = null;
        [SerializeField]
        private PlayerFeet _feet = null;

        private PlayerState _state;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            _feet.OnFootTouch += OnLand;
            _feet.OnFootLeave += OnAirborne;

            if (_feet.IsColliding) _state = PlayerState.Grounded;
            else _state = PlayerState.Airborne;

            _data.OnActorDeath += OnPlayerDeath;
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
            if (_state == PlayerState.Grounded)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    AddImpulse(Vector2.up * _jumpForce);
                }
            }
        }

        public void Impulse(Vector2 impulseForce)
        {
            if (_state == PlayerState.Airborne)
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

        private void AddImpulse(Vector2 impulse)
        {
            _rigidbody.velocity += impulse;
        }

        private void OnLand()
        {
            _state = PlayerState.Grounded;
        }

        private void OnAirborne()
        {
            _state = PlayerState.Airborne;
        }

        private void OnPlayerDeath(ActorData playerData)
        {
            playerData.OnActorDeath -= OnPlayerDeath;
            Destroy(_feet);
            Destroy(this);
        }
    }
}