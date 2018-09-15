using Economy;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components.Economy
{
    public class PurchaseItemDisplay : UIComponent
    {
        public SmartEvent<Transaction> OnRequestPurchase = new SmartEvent<Transaction>();

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private Text _purchaseCostLabel = null;
        [SerializeField]
        private Button _puchaseButton = null;
        [SerializeField]
        private Text _puchaseButtonLabel = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private Transaction _transaction;

        private void Awake()
        {
            _puchaseButton.onClick.AddListener(OnClick);
        }

        public void SetTransaction(Transaction transaction, bool canAfford)
        {
            _transaction = transaction;
            _purchaseCostLabel.text = $"{transaction.Price.ToString()}";

            _puchaseButton.interactable = canAfford;

            if (canAfford) _puchaseButtonLabel.text = "Purchase";
            else _puchaseButtonLabel.text = "Can't Afford";
        }

        private void OnClick() => OnRequestPurchase.Raise(_transaction);
    }
}