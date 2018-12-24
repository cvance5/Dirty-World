using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIManager : Singleton<UIManager>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly to allow Unity Serialization
        [SerializeField]
        private List<UIScreen> _screens = new List<UIScreen>();
        public static UIScreen ActiveScreen = null;

        [SerializeField]
        private List<UIOverlay> _overlays = new List<UIOverlay>();
        public static Dictionary<Type, UIOverlay> ActiveOverlays = new Dictionary<Type, UIOverlay>();

        [SerializeField]
        private List<UIPopup> _popups = new List<UIPopup>();
        private Stack<UIPopup> _popupStack = new Stack<UIPopup>();
        public static UIPopup ActivePopup => Instance._popupStack.Count > 0 ? Instance._popupStack.Peek() : null;

        [Header("Layers")]
        [SerializeField]
        private RectTransform _baseLayer = null;
        [SerializeField]
        private RectTransform _screenLayer = null;
        [SerializeField]
        private RectTransform _overlayLayer = null;
        [SerializeField]
        private RectTransform _popupLayer = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public static RectTransform BaseLayer => Instance._baseLayer;
        public static RectTransform ScreenLayer => Instance._screenLayer;
        public static RectTransform OverlayLayer => Instance._overlayLayer;
        public static RectTransform PopupLayer => Instance._popupLayer;

        public static T Get<T>() where T : UIObject
        {
            T uiObject;
            var typeOfT = typeof(T);

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

        public static void Show(UIObject objectToShow)
        {
            var type = objectToShow.GetType();

            if (typeof(UIPopup).IsAssignableFrom(type))
                UpdatePopupStack(objectToShow as UIPopup);
            else
                throw new InvalidCastException("Type of " + type.ToString() + "  is not a showable UI object!");
        }

        public static T Create<T>() where T : UIObject
        {
            T uiObject;
            var typeOfT = typeof(T);

            if (typeof(UIOverlay).IsAssignableFrom(typeOfT))
                uiObject = CreateOverlay(typeOfT) as T;
            else if (typeof(UIPopup).IsAssignableFrom(typeOfT))
                uiObject = CreatePopup(typeOfT) as T;
            else
                throw new InvalidCastException("Type of " + typeOfT.ToString() + "  is not a creatable UI object!");

            uiObject.SetVisible(false);

            return uiObject;
        }

        public static void Clear(UIObject objectToClear)
        {
            var type = objectToClear.GetType();

            if (typeof(UIPopup).IsAssignableFrom(type))
                ClearPopup(type);
            else
                throw new InvalidCastException("Type of " + type.ToString() + "  is not a removable UI object!");
        }

        private static void SetOverlayVisibility(UIOverlay overlay, bool isVisible) => overlay.SetVisible(isVisible);

        private static UIScreen GetScreen(Type type)
        {
            UIScreen selectedScreen = null;

            if (ActiveScreen == null)
            {
                selectedScreen = CreateScreen(type);
            }
            else if (ActiveScreen.GetType() != type)
            {
                ActiveScreen.SetVisible(false);
                selectedScreen = CreateScreen(type);
            }
            else ActiveScreen.SetVisible(true);

            if (selectedScreen != null)
                SetActiveScreen(selectedScreen);

            return selectedScreen;
        }

        private static UIOverlay GetOverlay(Type type)
        {
            if (!ActiveOverlays.TryGetValue(type, out var selectedOverlay))
            {
                selectedOverlay = CreateOverlay(type);
            }
            else SetOverlayVisibility(selectedOverlay, true);

            return selectedOverlay;
        }

        private static UIPopup GetPopup(Type type)
        {
            var selectedPopup = CreatePopup(type);
            UpdatePopupStack(selectedPopup);

            if (selectedPopup.UseScrim)
            {
                Scrimmer.ScrimOver(Instance._popupLayer);
            }

            return selectedPopup;
        }

        private static void SetActiveScreen(UIScreen screen)
        {
            if (ActiveScreen != null)
            {
                ActiveScreen.SetVisible(false);
                ActiveScreen.transform.SetParent(null, false);
            }

            ActiveScreen = screen;
            ActiveScreen.SetVisible(true);
            ActiveScreen.transform.SetParent(Instance._screenLayer, false);
            ActiveScreen.ActivateScreen();
        }

        private static void SetActiveOverlay(Type type, UIOverlay overlay)
        {
            if (ActiveOverlays.TryGetValue(type, out var existingOverlay))
            {
                throw new InvalidOperationException($"Cannot set two of the same overlay active at a time.  Type: `{type}.");
            }

            ActiveOverlays.Add(type, overlay);
            overlay.SetVisible(true);
            overlay.transform.SetParent(OverlayLayer, false);
        }

        private static void UpdatePopupStack(UIPopup newPopup = null)
        {
            if (newPopup != null)
            {
                newPopup.transform.SetParent(Instance._popupLayer, false);
                Instance._popupStack.Push(newPopup);
            }
            else
            {
                var oldPopup = Instance._popupStack.Pop();
                Destroy(oldPopup.gameObject);
            }

            foreach (var popup in Instance._popupStack)
                popup.SetVisible(false);

            if (ActivePopup != null)
            {
                ActivePopup.SetVisible(true);
                ActivePopup.Activate();
                Scrimmer.ScrimOver(Instance._popupLayer);
            }
            else
            {
                Scrimmer.ClearScrim(Instance._popupLayer);
            }
        }

        private static UIScreen CreateScreen(Type type)
        {
            var selectedScreen = Instance._screens.Find(screen => screen.GetType() == type);

            if (selectedScreen != null)
            {
                selectedScreen = Instantiate(selectedScreen.gameObject).GetComponent(type) as UIScreen;
            }
            else throw new ArgumentOutOfRangeException("Could not find a screen of type " + type.ToString() + " in the screens listed!");

            return selectedScreen;
        }

        private static UIOverlay CreateOverlay(Type type)
        {
            var selectedOverlay = Instance._overlays.Find(overlay => overlay.GetType() == type);
            if (selectedOverlay != null)
            {
                selectedOverlay = Instantiate(selectedOverlay.gameObject).GetComponent(type) as UIOverlay;
            }
            else throw new ArgumentOutOfRangeException("Could not find an overlay of type " + type.ToString() + " in the overlays listed!");

            SetActiveOverlay(type, selectedOverlay);

            return selectedOverlay;
        }

        private static UIPopup CreatePopup(Type type)
        {
            UIPopup selectedPopup = null;
            foreach (var popup in Instance._popups)
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

        private static UIActor CreateActor(Type type)
        {
            var newObject = new GameObject();
            newObject.transform.Reset();
            var selectedActor = newObject.AddComponent(type) as UIActor;

            return selectedActor;
        }

        private static void ClearPopup(Type type)
        {
            if (type == typeof(UIPopup) || type == ActivePopup.GetType())
                UpdatePopupStack();
            else
                throw new ArgumentException("Can't clear a popup of type " + type + " because it is not displayed.");
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("UIManager");
    }
}