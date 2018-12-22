using UI.Components.UnityModifiers;
using UI.Effects;
using UnityEngine;

namespace UI.Screens
{
    public class PrologueScreen : UIScreen
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private TextFader _textFader = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public override void ActivateScreen()
        {
            _textFader.AssignText("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");

            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                _textFader.FadeIn(.25f, 5),
                ScreenGroup.SetInteractable(true)
            );

            sequence.Play();
        }

        public override void DeactivateScreen()
        {
            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                ScreenGroup.FadeTo(0f, .5f)
            );

            sequence.Play(() =>
            {
                OnScreenDeactivated.Raise(this);
                Destroy(gameObject);
                gameObject.SetActive(false);
            });
        }
    }
}