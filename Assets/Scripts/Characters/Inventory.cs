using Economy;
using Items;
using Items.Unlocking;
using System;
using System.Collections.Generic;

namespace Characters
{
    public class Inventory
    {
        public uint Wealth { get; private set; }

        private Dictionary<UnlockTypes, List<UnlockItem>> _unlockedItems;
        public Dictionary<UnlockTypes, List<UnlockItem>> UnlockedItems => new Dictionary<UnlockTypes, List<UnlockItem>>(_unlockedItems);

        public Inventory()
        {
            _unlockedItems = new Dictionary<UnlockTypes, List<UnlockItem>>();
            foreach (UnlockTypes unlockType in Enum.GetValues(typeof(UnlockTypes)))
            {
                _unlockedItems.Add(unlockType, new List<UnlockItem>());
            }
        }

        public void Add(Item item)
        {
            switch (item.Category)
            {
                case ItemCategories.Wealth:
                    Wealth += item.Amount;
                    break;

                case ItemCategories.Unlock:
                    AddUnlock(item as UnlockItem);
                    break;

                default: throw new ArgumentException($"Unknown category of item: {item.Category}.");
            }
        }

        public bool CanAfford(Transaction transaction)
        {
            switch (transaction.Price.Category)
            {
                case ItemCategories.Wealth:
                    return Wealth >= transaction.Price.Amount;

                default: throw new ArgumentException($"Cannot determine ability to pay {transaction.Price.Amount} of {transaction.Price.Category}.");
            }
        }

        public void Remove(Item item)
        {
            switch (item.Category)
            {
                case ItemCategories.Wealth:
                    if (Wealth < item.Amount) Wealth = 0;
                    else Wealth -= item.Amount;
                    break;

                case ItemCategories.Unlock:
                    RemoveUnlock(item as UnlockItem);
                    break;
            }
        }

        public bool HasUnlocked(UnlockItem unlockItem) => _unlockedItems[unlockItem.UnlockType].Contains(unlockItem);

        private void AddUnlock(UnlockItem unlockItem)
        {
            if (_unlockedItems[unlockItem.UnlockType].Contains(unlockItem))
            {
                throw new InvalidOperationException($"Character has already unlocked {unlockItem}.");
            }

            _unlockedItems[unlockItem.UnlockType].Add(unlockItem);
        }

        private void RemoveUnlock(UnlockItem unlockItem)
        {
            if (!_unlockedItems[unlockItem.UnlockType].Contains(unlockItem))
            {
                throw new InvalidOperationException($"Character has not unlocked {unlockItem} yet.");
            }

            _unlockedItems[unlockItem.UnlockType].Remove(unlockItem);
        }
    }
}