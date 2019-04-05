using Narrative;
using System.Collections;
using System.Text;
using UI.Effects;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Debug;

namespace UI.Components.UnityModifiers
{
    [RequireComponent(typeof(Text))]
    public class TextFader : UIComponent, IScriptedPlaybackListener
    {
        private Text _textBox;

        private string _textToDisplay;

        private float _timestepSeconds;
        private int _numCharsAtATime;
        private float _playbackSpeed;

        private CallbackEffect _fadeEffect;

        private void Awake() => _textBox = GetComponent<Text>();

        private void OnEnable() => Script.Subscribe(this);

        public void AssignText(string textToDisplay) => _textToDisplay = textToDisplay;

        public CallbackEffect FadeIn(float secondsPerLetter, int charactersAtATime = 1)
        {
            _numCharsAtATime = charactersAtATime;
            _timestepSeconds = secondsPerLetter / _numCharsAtATime;

            if (_fadeEffect != null)
            {
                _fadeEffect = null;
                _log.Warning($"Beginning another fade effect while one is still in progress will cause undefined behavior.");
            }

            _fadeEffect = new CallbackEffect(() => StartCoroutine(FadeInUpdate()));
            return _fadeEffect;
        }

        public void SetPlaybackSpeed(int playbackSpeed) => _playbackSpeed = playbackSpeed;

        private IEnumerator FadeInUpdate()
        {
            var time = 0f;
            var builder = new StringBuilder();

            var timeToComplete = _timestepSeconds * (_textToDisplay.Length + _numCharsAtATime);

            while (time < timeToComplete)
            {
                var lastFullyOpaque = (int)((time / _timestepSeconds) - _numCharsAtATime);

                if (lastFullyOpaque >= 0)
                {
                    builder.Append(_textToDisplay.Substring(0, lastFullyOpaque + 1));
                }

                for (var fadingChars = 1; fadingChars < _numCharsAtATime; fadingChars++)
                {
                    var myIndex = lastFullyOpaque + fadingChars;
                    var timestepStart = (lastFullyOpaque + fadingChars) * _timestepSeconds;
                    var timeIntoTimestep = time - timestepStart;
                    var opacity = timeIntoTimestep / (_timestepSeconds * _numCharsAtATime);

                    if (myIndex >= 0 && myIndex < _textToDisplay.Length)
                    {
                        builder.Append(GetBeforeFaderTag(opacity));
                        builder.Append(_textToDisplay[lastFullyOpaque + fadingChars]);
                        builder.Append(GetAfterFaderTag());
                    }
                }

                if (lastFullyOpaque + _numCharsAtATime < _textToDisplay.Length)
                {
                    builder.Append(GetBeforeFaderTag(0));
                    builder.Append(_textToDisplay.Substring(lastFullyOpaque + _numCharsAtATime));
                    builder.Append(GetAfterFaderTag());
                }

                _textBox.text = builder.ToString();
                builder.Clear();

                yield return null;
                time += (Time.deltaTime * _playbackSpeed);
            }

            CompleteFade();
        }

        private void CompleteFade()
        {
            _textBox.text = _textToDisplay;
            _fadeEffect.Complete();
            _fadeEffect = null;
        }

        private string GetBeforeFaderTag(float percentAlpha)
        {
            if (percentAlpha < 0 || percentAlpha > 1)
            {
                _log.Warning($"Alpha must be between 0 and 1.  Clamping `{percentAlpha}`.");
                percentAlpha = Mathf.Clamp01(percentAlpha);
            }

            var alpha = Mathf.RoundToInt(percentAlpha * 255);

            var hexRepresentation = alpha.ToString("X2");
            return $"<color=#FFFFFF{hexRepresentation}>";
        }

        private void OnDisable() => Script.Unsubscribe(this);

        private string GetAfterFaderTag() => "</color>";

        private static readonly Log _log = new Log("TextFader");
    }
}