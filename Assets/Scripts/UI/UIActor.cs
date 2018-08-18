namespace UI
{
    public abstract class UIActor : UIObject
    {
        public override void SetVisible(bool isVisible)
        {
            gameObject.SetActive(true);
        }

        public abstract void SetData(object sourceData);
    }
}