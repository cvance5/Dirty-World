using UnityEngine;

namespace WorldObjects.Actors.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField]
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        private float _minZoom = 5f;
        [SerializeField]
        private float _maxZoom = 10f;

        [SerializeField]
        private float _minScroll = 0f;
        [SerializeField]
        private float _maxScroll = 10f;
        [SerializeField]
        private float _scrollSensitivity = .5f;
        private float _scrollZoom;

        [SerializeField]
        private PlayerMovement _movement = null;
        [SerializeField]
        private PlayerHealth _data = null;
        [SerializeField]
        private Rigidbody2D _rigidbody = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private void Awake()
        {
            _data.OnActorDeath += OnPlayerDeath;
        }

        private void Update()
        {
            _scrollZoom -= (Input.GetAxis("Scroll") * _scrollSensitivity);
            _scrollZoom = Mathf.Clamp(_scrollZoom, _minScroll, _maxScroll);
        }

        private void LateUpdate()
        {
            Camera.main.transform.position = new Vector3()
            {
                x = transform.position.x,
                y = transform.position.y,
                z = -10
            };

            var percentOfMaxSpeed = 0f;
            if (_rigidbody.velocity != Vector2.zero)
            {
                percentOfMaxSpeed = (_rigidbody.velocity.magnitude / _movement.MaximumSpeed);
            }

            var minZoom = _minZoom + _scrollZoom;
            var maxZoom = _maxZoom + _scrollZoom;

            var targetZoom = (percentOfMaxSpeed * (maxZoom - minZoom) + minZoom);
            var actualZoom = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime);

            Camera.main.orthographicSize = actualZoom;
        }

        private void OnPlayerDeath(ActorHealth playerData)
        {
            playerData.OnActorDeath -= OnPlayerDeath;
            Destroy(this);
        }
    }
}