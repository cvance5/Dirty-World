﻿using UnityEngine;

namespace Actors.Player.GunActors
{
    public abstract class GunActor : MonoBehaviour
    {      
        protected ContactFilter2D _filter;

        protected RaycastHit2D[] _hits = new RaycastHit2D[1];

        protected void Awake()
        {
            _filter = new ContactFilter2D();
            _filter.SetLayerMask(new LayerMask()
            {
                value = LayerMask.GetMask("WorldObject")
                      | LayerMask.GetMask("Enemy")
            });

            OnAwake();
        }

        protected abstract void OnAwake();

        public abstract void Fire();
        public abstract void AlternateFire();
    }
}