using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : UIObject
    {
        public static SmartEvent<UIScreen> OnScreenActivated = new SmartEvent<UIScreen>();
        public static SmartEvent<UIScreen> OnScreenDeactivated = new SmartEvent<UIScreen>();

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