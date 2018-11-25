public interface IHittable
{
    /// <summary>
    /// Parameters should be the effective damage and force of the hit. Must be settable to use registration +=.
    /// </summary>
    SmartEvent<IHittable, int, int> OnHit { get; set; }
    void Hit(int damage, int force);
}