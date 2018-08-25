using UnityEngine;

namespace UI
{
    public abstract class UIScreen : UIObject
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

        public abstract void ActivateScreen();
        public abstract void DeactivateScreen();

        private void OnDestroy()
        {
            if(isActiveAndEnabled)
            {
                DeactivateScreen();
            }            
        }
    }
}