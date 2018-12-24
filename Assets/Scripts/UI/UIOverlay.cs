using UnityEngine;

namespace UI
{
    public abstract class UIOverlay : UIObject
    {
        public CanvasGroup ScreenGroup { get; private set; }

        private void Awake()
        {
            ScreenGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        public override void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}