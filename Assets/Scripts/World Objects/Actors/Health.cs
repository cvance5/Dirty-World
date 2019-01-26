using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors
{
    public class Health
    {
        public SmartEvent OnHealthChanged { get; set; } = new SmartEvent();

        public bool IsAlive => _healthSegments.Count != 0;
        public bool IsMaxSegments => _healthSegments.Count == _maxHealthSegments;

        private Stack<HealthSegment> _healthSegments = new Stack<HealthSegment>();
        private HealthSegment _nextSegment => IsAlive ? _healthSegments.Peek() : null;

        private readonly int _maxHealthPerSegment;
        private readonly int _maxHealthSegments;

        public int MaxHealth => _maxHealthPerSegment * _maxHealthSegments;
        public int CurrentHealth => IsAlive ? (_maxHealthPerSegment * _healthSegments.Count) - _healthSegments.Peek().CurrentDamage : 0;

        public Health(int maxHealthPerSegment, int maxHealthSegments)
        {
            _maxHealthPerSegment = maxHealthPerSegment;
            _maxHealthSegments = maxHealthSegments;

            for (var segment = 0; segment < maxHealthSegments; segment++)
            {
                _healthSegments.Push(new HealthSegment(maxHealthPerSegment));
            }
        }

        public void Damage(int amount)
        {
            if (_healthSegments.Count == 0) return;

            _nextSegment.ChangeHealth(-amount);

            if (_nextSegment.Health == 0)
            {
                _healthSegments.Pop();
            }

            OnHealthChanged.Raise();
        }

        public void Heal(int amount)
        {
            if (_healthSegments.Count == 0) return;

            amount = _nextSegment.ChangeHealth(amount);

            while (amount > 0 && !IsMaxSegments)
            {
                var startingHealth = Mathf.Min(amount, _maxHealthPerSegment);
                _healthSegments.Push(new HealthSegment(_maxHealthPerSegment, startingHealth));
                amount -= _maxHealthPerSegment;
            }

            OnHealthChanged.Raise();
        }

        public void FillSegment()
        {
            if (_healthSegments.Count == 0) return;

            if (_nextSegment.CurrentDamage != 0)
            {
                _nextSegment.ChangeHealth(_nextSegment.CurrentDamage);
            }
            else if (!IsMaxSegments)
            {
                _healthSegments.Push(new HealthSegment(_maxHealthPerSegment));
            }

            OnHealthChanged.Raise();
        }

        public void EmptySegment()
        {
            if (_healthSegments.Count == 0) return;

            _healthSegments.Pop();

            OnHealthChanged.Raise();
        }
    }
}