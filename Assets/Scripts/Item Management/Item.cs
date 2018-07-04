using UnityEngine;

namespace ItemManagement
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Item : MonoBehaviour
    {
        public InteractionTypes[] Interactions { get; protected set; }

        private void Awake() => InitializeInteractions();

        protected abstract void InitializeInteractions();
    }
}