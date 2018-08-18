namespace UI
{
    public abstract class UIScreen : UIObject
    {
        public override void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public abstract void ActivateScreen();
    }
}