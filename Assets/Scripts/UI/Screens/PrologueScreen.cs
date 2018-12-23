using Narrative;
using UI.Components.UnityModifiers;
using UI.Effects;
using UnityEngine;

namespace UI.Screens
{
    public class PrologueScreen : UIScreen
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private ScriptKey _fadeInKey = null;

        [SerializeField]
        private TextFader _textFader = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public override void ActivateScreen()
        {
            _textFader.AssignText(Script.Read(_fadeInKey));

            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                _textFader.FadeIn(.35f, 5),
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