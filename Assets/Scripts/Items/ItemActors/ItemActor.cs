using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ItemActor : MonoBehaviour
    {
        public InteractionTypes[] Interactions { get; protected set; }

        private void Awake() => InitializeInteractions();

        protected abstract void InitializeInteractions();
    }
}