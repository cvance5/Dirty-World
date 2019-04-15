using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Actors
{
    public class Health
    {
        public SmartEvent<int> OnHealthChanged { get; set; } = new SmartEvent<int>();

        public bool IsAlive => _healthSegments.Count != 0;
        public bool IsSegmentFull => IsAlive ? _nextSegment.CurrentDamage == 0 : false;
        public bool IsMaxSegments => _healthSegments.Count == MaxHealthSegments;

        public int MaxHealth => MaxHealthPerSegment * MaxHealthSegments;
        public int CurrentHealth => IsAlive ? (MaxHealthPerSegment * _healthSegments.Count) - _healthSegments.Peek().CurrentDamage : 0;
        public int SegmentsAvailable => _healthSegments.Count;

        public readonly int MaxHealthPerSegment;
        public readonly int MaxHealthSegments;

        private Stack<HealthSegment> _healthSegments = new Stack<HealthSegment>();
        private HealthSegment _nextSegment => IsAlive ? _healthSegments.Peek() : null;

        public Health(int maxHealthPerSegment, int maxHealthSegments)
        {
            MaxHealthPerSegment = maxHealthPerSegment;
            MaxHealthSegments = maxHealthSegments;

            for (var segment = 0; segment < maxHealthSegments; segment++)
            {
                _healthSegments.Push(new HealthSegment(maxHealthPerSegment));
            }
        }

        public int HealthForSegment(int number)
        {
            if (_healthSegments.Count < number)
            {
                throw new InvalidOperationException($"No segment exists for {number}.");
            }
            else if (_healthSegments.Count - 1 == number)
            {
                // This is the top segment
                return _nextSegment.Health;
            }
            else return MaxHealthPerSegment;
        }

        public void Damage(int amount)
        {
            if (_healthSegments.Count == 0) return;

            var actualAmount = Mathf.Min(amount, _nextSegment.Health);
            _nextSegment.ChangeHealth(-amount);

            if (_nextSegment.Health == 0)
            {
                _healthSegments.Pop();
            }

            OnHealthChanged.Raise(-actualAmount);
        }

        public void Heal(int amount, bool allowOverfill = true)
        {
            if (_healthSegments.Count == 0) return;

            int actualAmount;
            if (allowOverfill)
            {
                actualAmount = amount;
            }
            else actualAmount = Mathf.Min(amount, _nextSegment.CurrentDamage);
            amount = _nextSegment.ChangeHealth(amount);

            if (allowOverfill)
            {
                while (amount > 0 && !IsMaxSegments)
                {
                    var startingHealth = Mathf.Min(amount, MaxHealthPerSegment);
                    _healthSegments.Push(new HealthSegment(MaxHealthPerSegment, startingHealth));
                    amount -= MaxHealthPerSegment;
                }
            }

            OnHealthChanged.Raise(actualAmount);
        }

        public void FillSegment()
        {
            if (_healthSegments.Count == 0) return;

            var amountHealed = 0;

            if (_nextSegment.CurrentDamage != 0)
            {
                amountHealed = _nextSegment.CurrentDamage;
                _nextSegment.ChangeHealth(_nextSegment.CurrentDamage);
            }
            else if (!IsMaxSegments)
            {
                amountHealed = MaxHealthPerSegment;
                _healthSegments.Push(new HealthSegment(MaxHealthPerSegment));
            }

            OnHealthChanged.Raise(amountHealed);
        }

        public void EmptySegment()
        {
            if (_healthSegments.Count == 0) return;

            _healthSegments.Pop();

            OnHealthChanged.Raise(-MaxHealthPerSegment);
        }
    }
}