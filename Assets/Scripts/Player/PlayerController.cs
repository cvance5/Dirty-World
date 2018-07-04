using ItemManagement;
using Metadata;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public float MovementSpeed;
        public float MaximumSpeed;

        public Gun Gun;

        private Rigidbody2D _rigidbody;
        private PlayerData _data;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _data = GetComponent<PlayerData>();
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
                currentForce = (MaximumSpeed * currentForce.normalized);
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tag = other.tag;

            if (tag == Tags.Item)
            {
                var otherItem = other.GetComponent<Item>();

                Log.ErrorIfNull(otherItem, $"{other} has tag {tag} but does not have an item component.");

                foreach (var interaction in otherItem.Interactions)
                {
                    switch (interaction)
                    {
                        case InteractionTypes.Collect:
                            var otherCollectible = otherItem as ICollectible;
                            Log.ErrorIfNull(otherCollectible, $"{other} has interaction {interaction} but does not implement ICollectible.");
                            _data.AddItem(otherCollectible.GetItemType());
                            otherCollectible.OnCollect();
                            break;
                        case InteractionTypes.Damage:
                            break;
                        default: Log.Error($"Unknown interaction '{interaction}'."); break;
                    }
                }
            }
        }
    }
}