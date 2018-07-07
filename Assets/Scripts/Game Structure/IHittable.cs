public interface IHittable
{
    void Hit(int damage, int force);
    void Impact(int impactMagnitude);
}