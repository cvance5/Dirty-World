using UI.Effects;
using UI.UIScreens.PurchaseUpgradesScreenComponents;
using UnityEngine;

namespace UI.UIScreens
{
    public class PurchaseUpgradesScreen : UIScreen
    {
        [SerializeField]
        private UpgradeFrame _upgradeFrameTemplate;

        public override void ActivateScreen()
        {
            var sequence = new SequenceEffect
            (
                ScreenGroup.SetInteractable(false),
                ScreenGroup.Hide(),
                ScreenGroup.FadeTo(1f, .5f),
                ScreenGroup.SetInteractable(true)
            );

            sequence.Play(FillUpgradeList);
        }

        private void FillUpgradeList()
        {

        }

        public override void DeactivateScreen()
        {
            throw new System.NotImplementedException();
        }
    }
}