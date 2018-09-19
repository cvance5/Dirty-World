using UnityEngine;

namespace Actors.Player.GunActors
{
    public class SeismicBomb : MonoBehaviour
    {
        private int _force;
        private float _range;

        public void Ignite(int force, float range)
        {
            _force = force;
            _range = range;
        }

        public void Explode()
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, _range);

            foreach (Collider2D target in targets)
            {
                Vector3 vector = target.transform.position - transform.position;
                var _impactForce = Mathf.RoundToInt((_range - vector.magnitude) * _force);

                var hittable = (IHittable)target.GetComponent(typeof(IHittable));
                if (hittable != null)
                {
                    hittable.Hit(0, _impactForce);
                }
            }

            Destroy(gameObject);
        }
    }
}