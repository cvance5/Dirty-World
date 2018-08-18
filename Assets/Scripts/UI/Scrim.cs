using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class Scrim : UIObject
    {
        public SmartEvent<Transform> OnScrimDestroyed = new SmartEvent<Transform>();

        private Image _scrim;
        private RectTransform _transform;

        public void Initialize()
        {
            _scrim = GetComponent<Image>();
            _scrim.color = Color.black;

            _transform = GetComponent<RectTransform>();
            _transform.anchorMin = Vector2.zero;
            _transform.anchorMax = Vector2.one;
            _transform.sizeDelta = Vector2.zero;
            _transform.localPosition = Vector2.zero;
        }

        public override void SetVisible(bool isVisible)
        {
            _scrim.enabled = isVisible;
        }

        public void Hide()
        {
            _scrim.color = _scrim.color.SwapAlpha(0);
        }

        public void FadeTo(float alpha, float duration)
        {
            _scrim.DOFade(alpha, duration);
        }

        private void OnDestroy()
        {
            if (!_isApplicationQuitting) OnScrimDestroyed.Raise(transform.parent);
        }
    }
}