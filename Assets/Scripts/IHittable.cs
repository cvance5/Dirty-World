public interface IHittable
{
    /// <summary>
    /// Parameters should be the effective damage and force of the hit.
    /// </summary>
    SmartEvent<int, int> OnHit { get; set; }
    void Hit(int damage, int force);
}