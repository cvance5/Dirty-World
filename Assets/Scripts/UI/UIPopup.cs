namespace UI
{
    public class UIPopup : UIObject
    {
        public bool UseScrim;

        public virtual void Activate() { }

        public override void SetVisible(bool isVisible)
        {
            this.SetActive(isVisible);
        }

        public virtual void Deactivate() { }

        protected void Close()
        {
            UIManager.Instance.Clear(this);
        }
    }
}