using UI.Effects;

namespace UI.Screens
{
    public class GameOverScreen : UIScreen
    {
        public override void ActivateScreen()
        {
            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                ScreenGroup.Hide(),
                ScreenGroup.FadeTo(1f, 1.5f),
                ScreenGroup.SetInteractable(true)
            );

            sequence.Play(() => OnScreenActivated.Raise(this));
        }

        public void OnRespawnClicked() => DeactivateScreen();

        public void OnUpgradeClicked()
        {
            DeactivateScreen();
            OnScreenDeactivated += ActivateUpgradesScreen;
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

        private static void ActivateUpgradesScreen(UIScreen callingGameOverScreen)
        {
            OnScreenDeactivated -= ActivateUpgradesScreen;
            UIManager.Get<WeaponSelectScreen>();
        }
    }
}