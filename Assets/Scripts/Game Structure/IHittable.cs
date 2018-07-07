using UnityEngine;

public interface IHittable
{
    void Hit(int damage, int force);
    void Impact(Vector2 impact);
}