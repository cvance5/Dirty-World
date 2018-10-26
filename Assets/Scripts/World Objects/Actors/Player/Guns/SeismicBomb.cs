using UnityEngine;

namespace WorldObjects.Actors.Player.Guns
{
    public class SeismicBomb : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private int _force = 0;
        [SerializeField]
        private float _range = 0;
#pragma warning restore IDE0044 // Add readonly modifier

        public void Explode()
        {
            var targets = Physics2D.OverlapCircleAll(transform.position, _range);

            foreach (var target in targets)
            {
                var vector = target.transform.position - transform.position;
                var impactForce = Mathf.RoundToInt((_range - vector.magnitude) * _force);

                var hittable = (IHittable)target.GetComponent(typeof(IHittable));
                if (hittable != null)
                {
                    hittable.Hit(0, impactForce);
                }
            }

            Destroy(gameObject);
        }
    }
}