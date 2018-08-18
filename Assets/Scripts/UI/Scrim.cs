using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class Scrim : UIObject
    {
        public SmartEvent<Transform> OnScrimDestroyed = new SmartEvent<Transform>();

        private Image _scrim;

        private void Awake()
        {
            _scrim = GetComponent<Image>();
        }

        public override void SetVisible(bool isVisible)
        {
            _scrim.enabled = isVisible;
        }

        private void OnDestroy()
        {
            OnScrimDestroyed.Raise(transform.parent);
        }
    }
}