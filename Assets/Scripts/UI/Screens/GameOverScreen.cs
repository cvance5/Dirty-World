using UI.Effects;

namespace UI.UIScreens
{
    public class GameOverScreen : UIScreen
    {
        public SmartEvent OnRespawn = new SmartEvent();
        public SmartEvent OnUpgrade = new SmartEvent();

        public override void ActivateScreen()
        {
            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                ScreenGroup.Hide(),
                ScreenGroup.FadeTo(1f, 1.5f),
                ScreenGroup.SetInteractable(true)
            );

            sequence.Play();
        }

        public void OnRespawnClicked()
        {
            OnRespawn.Raise();
            DeactivateScreen();
        }

        public void OnUpgradeClicked() => OnUpgrade.Raise();

        public override void DeactivateScreen()
        {
            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                ScreenGroup.FadeTo(0f, .5f)
            );

            sequence.Play(() => Destroy(gameObject));
            enabled = false;
        }
    }
}