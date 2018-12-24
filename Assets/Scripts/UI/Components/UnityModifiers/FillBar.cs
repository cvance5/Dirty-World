using UnityEngine;
using UnityEngine.UI;

namespace UI.Components.UnityModifiers
{
    [RequireComponent(typeof(Image))]
    public class FillBar : UIComponent
    {
        private Image _fillImage;

        private float _minimum;
        private float _maximum;
        private float _range;

        private void Awake() => _fillImage = GetComponent<Image>();

        public void SetRange(float minimum, float maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
            _range = _maximum - _minimum;
        }

        public void UpdateValue(float value)
        {
            var distance = value - _minimum;
            _fillImage.fillAmount = value / _range;
        }
    }
}