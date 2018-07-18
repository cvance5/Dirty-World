using UnityEngine;

namespace Actors.Player
{
    public abstract class Gun : MonoBehaviour
    {
        [SerializeField]
        protected float _range;
        [SerializeField]
        protected int _damage;
        [SerializeField]
        protected int _force;

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
    }
}