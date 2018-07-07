﻿using ItemManagement;
using Metadata;
using UnityEngine;
using WorldObjects;

namespace Actors.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _movementSpeed = 10f;
        [SerializeField]
        private float _maximumSpeed = 20f;

        [SerializeField]
        private float _minZoom = 5f;
        [SerializeField]
        private float _maxZoom = 10f;

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

            AddForce(movementVector * _movementSpeed * Time.deltaTime);

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

            var percentOfMaxSpeed = 0f;
            if (_rigidbody.velocity != Vector2.zero)
            {
                percentOfMaxSpeed = (_rigidbody.velocity.magnitude / _maximumSpeed);
            }

            var targetZoom = (percentOfMaxSpeed * (_maxZoom - _minZoom) + _minZoom);
            var actualZoom = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime);

            Camera.main.orthographicSize = actualZoom;
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
            _log.ErrorIfNull(item, $"{item} has tag {Tags.Item} but does not have an item component.");

            foreach (var interaction in item.Interactions)
            {
                switch (interaction)
                {
                    case InteractionTypes.Collect:
                        var collectibleItem = item as ICollectible;
                        _log.ErrorIfNull(collectibleItem, $"{item} has interaction {interaction} but does not implement {typeof(ICollectible).Name}.");
                        _data.AddItem(collectibleItem.GetItemType());
                        collectibleItem.OnCollect();
                        break;
                    case InteractionTypes.Damage:
                        break;
                    default: _log.Error($"Unknown interaction '{interaction}'."); break;
                }
            }
        }

        private void HandleHazard(Hazard hazard)
        {
            _log.ErrorIfNull(hazard, $"{hazard} has tag {Tags.Hazard} but does not have an item component.");

            foreach (var effect in hazard.Effects)
            {
                switch (effect)
                {
                    case HazardEffects.Damage:
                        var damagingHazard = hazard as IDamaging;
                        _log.ErrorIfNull(damagingHazard, $"{hazard} has effect {effect} but does not implement {typeof(IDamaging).Name}.");
                        _data.ApplyDamage(damagingHazard.GetDamage());
                        break;
                    case HazardEffects.Impulse:
                        var knockbackHazard = hazard as IImpulsive;
                        _log.ErrorIfNull(knockbackHazard, $"{hazard} has effect {effect} but does not implement {typeof(IImpulsive).Name}.");
                        _rigidbody.velocity = knockbackHazard.GetImpulse(_rigidbody.velocity);
                        break;
                    default: _log.Error($"Unknown effect '{effect}'."); break;
                }
            }
        }

        private static readonly Log _log = new Log("PlayerController");
    }
}