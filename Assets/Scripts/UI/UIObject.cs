using UnityEngine;

namespace UI
{
    public abstract class UIObject : MonoBehaviour
    {
        protected bool _isApplicationQuitting;

        public abstract void SetVisible(bool isVisible);

        protected void OnApplicationQuit()
        {
            _isApplicationQuitting = true;
        }
    }
}