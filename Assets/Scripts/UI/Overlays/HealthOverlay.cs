using UI.Components.UnityModifiers;
using UnityEngine;
using WorldObjects.Actors;

namespace UI.Overlays
{
    public class HealthOverlay : UIOverlay
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly for Unity Serialization
        [SerializeField]
        private FillBar _healthFill = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private Health _health;

        public void AssignHealth(Health health)
        {
            _health = health;
            _health.OnHealthChanged += OnHealthChanged;

            _healthFill.SetRange(0, _health.MaxHealth);
        }

        private void OnHealthChanged(int delta)
        {
            _healthFill.UpdateValue(_health.CurrentHealth);
        }
    }
}