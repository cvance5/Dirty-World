namespace WorldObjects.Actors
{
    public class HealthSegment
    {
        private readonly int _maxHealth;
        public int Health { get; private set; }
        public int CurrentDamage => _maxHealth - Health;

        public HealthSegment(int maxHealth)
        {
            _maxHealth = maxHealth;
            Health = _maxHealth;
        }

        public HealthSegment(int maxHealth, int startingHealth)
        {
            _maxHealth = maxHealth;
            Health = startingHealth;
        }

        public int ChangeHealth(int amount)
        {
            Health += amount;

            var overflow = 0;
            if (Health < 0)
            {
                overflow = Health;
                Health = 0;
            }
            else if (Health > _maxHealth)
            {
                overflow = Health - _maxHealth;
                Health = _maxHealth;
            }

            return overflow;
        }
    }
}