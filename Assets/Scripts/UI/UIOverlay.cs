namespace UI
{
    public abstract class UIOverlay : UIObject
    {
        public override void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}