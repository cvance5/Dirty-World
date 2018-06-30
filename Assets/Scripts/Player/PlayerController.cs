using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public float MovementSpeed;
        public float MaximumSpeed;

        public Gun Gun;

        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Vector2 movementVector = new Vector2()
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };

            if (movementVector.x != 0) Orient(movementVector.x);

            AddForce(movementVector * MovementSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Fire"))
            {
                Gun.Fire();
            }
        }

        private void LateUpdate()
        {
            Camera.main.transform.position = new Vector3()
            {
                x = transform.position.x,
                y = transform.position.y,
                z = -10
            };
        }

        private void AddForce(Vector2 force)
        {
            var currentForce = _rigidbody.velocity + force;

            if (currentForce.magnitude > MaximumSpeed)
            {
                currentForce *= (currentForce.magnitude / MaximumSpeed);
            }

            _rigidbody.velocity = currentForce;
        }

        private void Orient(float orientDirction)
        {
            if (orientDirction < 0)
            {
                transform.right = Vector2.left;
            }
            else if (orientDirction > 0)
            {
                transform.right = Vector2.right;
            }
        }
    }
}