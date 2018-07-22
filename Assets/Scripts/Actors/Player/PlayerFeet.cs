using System.Collections.Generic;
using UnityEngine;

namespace Actors.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerFeet : MonoBehaviour
    {
        public SmartEvent OnFootTouch = new SmartEvent();
        public SmartEvent OnFootLeave = new SmartEvent();

        private List<Collider2D> _currentCollisions = new List<Collider2D>();

        private Collider2D _footCollider;

        private void Awake()
        {
            _footCollider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!_currentCollisions.Contains(col))
            {
                _currentCollisions.Add(col);
            }

            if (_currentCollisions.Count > 0)
            {
                OnFootTouch.Raise();
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (_currentCollisions.Contains(col))
            {
                _currentCollisions.Remove(col);
            }

            if (_currentCollisions.Count == 0)
            {
                OnFootLeave.Raise();
            }
        }
    }
}