using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIManager : Singleton<UIManager>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly to allow Unity Serialization
        [SerializeField]
        private List<UIScreen> _screens;
        private UIScreen _activeScreen;

        [SerializeField]
        private List<UIOverlay> _overlays;
        private UIOverlay _activeOverlay;

        [SerializeField]
        private List<UIPopup> _popups;
        private Stack<UIPopup> _popupStack;
        private UIPopup _activePopup => _popupStack.Count > 0 ? _popupStack.Peek() : null;

        [Header("Layers")]
        [SerializeField]
        private Transform _overlayLayer;
        [SerializeField]
        private Transform _screenLayer;
        [SerializeField]
        private Transform _popupLayer;

        [Space]
        [SerializeField]
        private GameObject _scrim;
#pragma warning restore IDE0044 // Add readonly modifier

        public override void Initialize()
        {
            _activeOverlay = null;
            _activeScreen = null;
            _popupStack = new Stack<UIPopup>();
        }

        public T Get<T>() where T : UIObject
        {
            T uiObject;
            Type typeOfT = typeof(T);

            if (typeof(UIScreen).IsAssignableFrom(typeOfT))
                uiObject = GetScreen(typeOfT) as T;
            else if (typeof(UIOverlay).IsAssignableFrom(typeOfT))
                uiObject = GetOverlay(typeOfT) as T;
            else if (typeof(UIPopup).IsAssignableFrom(typeOfT))
                uiObject = GetPopup(typeOfT) as T;
            else if (typeof(UIActor).IsAssignableFrom(typeOfT))
                uiObject = CreateActor(typeOfT) as T;
            else
                throw new InvalidCastException("Type of " + typeOfT.ToString() + "  is not a UIObject!");

            return uiObject;
        }

        public void Show(UIObject objectToShow)
        {
            Type type = objectToShow.GetType();

            if (typeof(UIOverlay).IsAssignableFrom(type))
                ShowOverlay(objectToShow as UIOverlay);
            else if (typeof(UIPopup).IsAssignableFrom(type))
                UpdatePopupStack(objectToShow as UIPopup);
            else
                throw new InvalidCastException("Type of " + type.ToString() + "  is not a showable UI object!");
        }

        public T Create<T>() where T : UIObject
        {
            T uiObject;
            Type typeOfT = typeof(T);

            if (typeof(UIOverlay).IsAssignableFrom(typeOfT))
                uiObject = CreateOverlay(typeOfT) as T;
            else if (typeof(UIPopup).IsAssignableFrom(typeOfT))
                uiObject = CreatePopup(typeOfT) as T;
            else
                throw new InvalidCastException("Type of " + typeOfT.ToString() + "  is not a creatable UI object!");

            uiObject.SetVisible(false);

            return uiObject;
        }

        public void Clear(UIObject objectToClear)
        {
            Type type = objectToClear.GetType();

            if (typeof(UIPopup).IsAssignableFrom(type))
                ClearPopup(type);
            else
                throw new InvalidCastException("Type of " + type.ToString() + "  is not a removable UI object!");
        }

        public void SetVisibility<T>(bool isVisible)
        {
            Type typeOfT = typeof(T);

            if (typeof(UIOverlay).IsAssignableFrom(typeOfT))
                SetOverlayVisibility(typeOfT, isVisible);
            else
                throw new InvalidCastException("Type of " + typeOfT.ToString() + "  cannot have its visibility set by the manager!  Access the object directly instead.");
        }

        private void SetOverlayVisibility(Type type, bool isVisible)
        {
            if (_activeOverlay.GetType() == type)
                SetActiveOverlay(_activeOverlay);
            else
                _log.Warning("Overlay of type " + type.ToString() + " is not active!");
        }

        private UIScreen GetScreen(Type type)
        {
            UIScreen selectedScreen = null;

            if (_activeScreen == null)
                selectedScreen = CreateScreen(type);
            else if (_activeScreen.GetType() != type)
            {
                _activeScreen.SetVisible(false);
                selectedScreen = CreateScreen(type);
            }
            else
                _activeScreen.SetVisible(true);

            if (selectedScreen != null)
                SetActiveScreen(selectedScreen);

            return selectedScreen;
        }

        private UIOverlay GetOverlay(Type type)
        {
            UIOverlay selectedOverlay = null;

            if (_activeOverlay == null)
                selectedOverlay = CreateOverlay(type);
            else if (_activeOverlay.GetType() != type)
            {
                _activeOverlay.SetVisible(false);
                selectedOverlay = CreateOverlay(type);
            }
            else
                SetOverlayVisibility(type, true);

            if (selectedOverlay != null)
                SetActiveOverlay(selectedOverlay);

            return selectedOverlay;
        }

        private UIPopup GetPopup(Type type)
        {
            UIPopup selectedPopup = CreatePopup(type);
            UpdatePopupStack(selectedPopup);

            _scrim.SetActive(selectedPopup.UseScrim);

            return selectedPopup;
        }

        private void ShowOverlay(UIOverlay overlay)
        {
            if (_activeOverlay != overlay)
                SetActiveOverlay(overlay);
        }

        private void SetActiveScreen(UIScreen screen)
        {
            if (_activeScreen != null)
            {
                _activeScreen.SetVisible(false);
                _activeScreen.transform.SetParent(null, false);
            }

            _activeScreen = screen;
            _activeScreen.SetVisible(true);
            _activeScreen.transform.SetParent(_screenLayer, false);
            _activeScreen.ActivateScreen();
        }

        private void SetActiveOverlay(UIOverlay overlay)
        {
            if (_activeOverlay != null)
            {
                _activeOverlay.SetVisible(false);
            }

            _activeOverlay = overlay;
            _activeOverlay.SetVisible(true);
        }

        private void UpdatePopupStack(UIPopup newPopup = null)
        {
            if (newPopup != null)
            {
                newPopup.transform.SetParent(_popupLayer, false);
                _popupStack.Push(newPopup);
            }
            else
            {
                UIPopup oldPopup = _popupStack.Pop();
                Destroy(oldPopup.gameObject);
            }

            foreach (UIPopup popup in _popupStack)
                popup.SetVisible(false);

            if (_activePopup != null)
            {
                _activePopup.SetVisible(true);
                _activePopup.Activate();
                _scrim.SetActive(_activePopup.UseScrim);
            }
            else
            {
                _scrim.SetActive(false);
            }
        }

        private UIScreen CreateScreen(Type type)
        {
            UIScreen selectedScreen = null;

            foreach (UIScreen screen in _screens)
                if (screen.GetType() == type)
                {
                    selectedScreen = Instantiate(screen.gameObject).GetComponent(type) as UIScreen;
                    break;
                }

            if (selectedScreen == null)
                throw new ArgumentOutOfRangeException("Could not find a screen of type " + type.ToString() + " in the screens listed!");

            return selectedScreen;
        }

        private UIOverlay CreateOverlay(Type type)
        {
            UIOverlay selectedOverlay = null;

            foreach (UIOverlay overlay in _overlays)
                if (overlay.GetType() == type)
                {
                    selectedOverlay = Instantiate(overlay.gameObject).GetComponent(type) as UIOverlay;
                    selectedOverlay.transform.SetParent(_overlayLayer, false);
                    break;
                }

            if (selectedOverlay == null)
                throw new ArgumentOutOfRangeException("Could not find an overlay of type " + type.ToString() + " in the overlays listed!");

            return selectedOverlay;
        }

        private UIPopup CreatePopup(Type type)
        {
            UIPopup selectedPopup = null;
            foreach (UIPopup popup in _popups)
            {
                if (popup.GetType() == type)
                {
                    selectedPopup = Instantiate(popup.gameObject).GetComponent(type) as UIPopup;
                    break;
                }
            }

            if (selectedPopup == null)
                throw new ArgumentOutOfRangeException("Could not find a popup of type " + type.ToString() + " in the popups listed!");

            return selectedPopup;
        }

        private UIActor CreateActor(Type type)
        {
            GameObject newObject = new GameObject();
            newObject.transform.Reset();
            UIActor selectedActor = newObject.AddComponent(type) as UIActor;

            return selectedActor;
        }

        private void ClearPopup(Type type)
        {
            if (type == typeof(UIPopup) || type == _activePopup.GetType())
                UpdatePopupStack();
            else
                throw new ArgumentException("Can't clear a popup of type " + type + " because it is not displayed.");
        }

        private static readonly Log _log = new Log("UIManager");
    }
}