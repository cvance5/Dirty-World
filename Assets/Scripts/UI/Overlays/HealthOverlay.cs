using UI.Components.UnityModifiers;
using UnityEngine;
using WorldObjects.Actors.Player;

namespace UI.Overlays
{
    public class HealthOverlay : UIOverlay
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly for Unity Serialization
        [SerializeField]
        private FillBar _healthFill = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private PlayerData _data;

        public void AssignPlayer(PlayerData data)
        {
            _data = data;
            _data.OnHealthChanged += OnHealthChanged;

            _healthFill.SetRange(0, _data.MaxHealth);
        }

        private void OnHealthChanged(int newHealth) => _healthFill.UpdateValue(newHealth);

    }
}