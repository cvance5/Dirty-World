using UnityEngine;
using UnityEngine.UI;

namespace UI.Components.UnityModifiers
{
    [RequireComponent(typeof(Image))]
    public class FillBar : UIComponent
    {
        public float FillAmount => _fillImage.fillAmount;
        public float CurrentValue => _minimum + (FillAmount * Range);

        public float Range { get; private set; }

        private Image _fillImage;

        private float _minimum;
        private float _maximum;

        private void Awake() => _fillImage = GetComponent<Image>();

        public void SetRange(float minimum, float maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
            Range = _maximum - _minimum;
        }

        public void UpdateValue(float newValue)
        {
            var distance = newValue - _minimum;
            _fillImage.fillAmount = newValue / Range;
        }

        public void SetFill(float fillPercent) => _fillImage.fillAmount = Mathf.Clamp01(fillPercent);
        public void ApplyDelta(float delta) => UpdateValue(CurrentValue + delta);
    }
}