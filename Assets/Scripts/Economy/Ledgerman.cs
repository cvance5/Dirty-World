using Characters;

namespace Economy
{
    public static class Ledgerman
    {
        private static Inventory _inventory => GameManager.Character.Inventory;

        public static bool TryPurchase(Transaction transaction)
        {
            if (_inventory.CanAfford(transaction))
            {
                _inventory.Remove(transaction.Price);
                _inventory.Add(transaction.Reward);

                return true;
            }
            else return false;
        }
    }
}