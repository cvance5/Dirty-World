using DG.Tweening;
using UnityEngine;

namespace UI.Effects
{
    public static class EffectExtensions
    {
        public static UIEffect SetInteractable(this CanvasGroup canvasGroup, bool isInteractable) =>
            new InstantEffect(() => canvasGroup.interactable = isInteractable);

        public static UIEffect Hide(this CanvasGroup canvasGroup) =>
            new InstantEffect(() => canvasGroup.alpha = 0);

        public static UIEffect FadeTo(this CanvasGroup canvasGroup, float alpha, float duration) =>
             new TweenEffect(canvasGroup.DOFade(alpha, duration));
    }
}