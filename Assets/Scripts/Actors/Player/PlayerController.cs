﻿using ItemManagement;
using Metadata;
using UnityEngine;
using WorldObjects;

namespace Actors.Player
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

            switch (tag)
            {
                case Tags.Item: HandleItem(other.GetComponent<Item>()); break;
                case Tags.Hazard: HandleHazard(other.GetComponent<Hazard>()); break;
            }
        }

        private void HandleItem(Item item)
        {
            Log.ErrorIfNull(item, $"{item} has tag {Tags.Item} but does not have an item component.");

            foreach (var interaction in item.Interactions)
            {
                switch (interaction)
                {
                    case InteractionTypes.Collect:
                        var collectibleItem = item as ICollectible;
                        Log.ErrorIfNull(collectibleItem, $"{item} has interaction {interaction} but does not implement {typeof(ICollectible).Name}.");
                        _data.AddItem(collectibleItem.GetItemType());
                        collectibleItem.OnCollect();
                        break;
                    case InteractionTypes.Damage:
                        break;
                    default: Log.Error($"Unknown interaction '{interaction}'."); break;
                }
            }
        }

        private void HandleHazard(Hazard hazard)
        {
            Log.ErrorIfNull(hazard, $"{hazard} has tag {Tags.Hazard} but does not have an item component.");

            foreach (var effect in hazard.Effects)
            {
                switch (effect)
                {
                    case HazardEffects.Damage:
                        var damagingHazard = hazard as IDamaging;
                        Log.ErrorIfNull(damagingHazard, $"{hazard} has effect {effect} but does not implement {typeof(IDamaging).Name}.");
                        _data.ApplyDamage(damagingHazard.GetDamage());
                        break;
                    case HazardEffects.Impulse:
                        var knockbackHazard = hazard as IImpulsive;
                        Log.ErrorIfNull(knockbackHazard, $"{hazard} has effect {effect} but does not implement {typeof(IImpulsive).Name}.");
                        _rigidbody.velocity = knockbackHazard.GetImpulse(_rigidbody.velocity);
                        break;
                    default: Log.Error($"Unknown effect '{effect}'."); break;
                }
            }
        }
    }
}