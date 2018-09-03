using Items;
using Items.Upgrades;
using System.Collections.Generic;

namespace Player
{
    public class Inventory
    {
        public uint Wealth { get; private set; }

        private readonly List<Upgrade> _upgrades = new List<Upgrade>();
        public List<Upgrade> Upgrades => new List<Upgrade>(_upgrades);

        public Inventory()
        {

        }

        public void Add(Item item)
        {
            switch (item.Category)
            {
                case ItemCategories.Wealth:
                    Wealth += item.Amount;
                    break;

                case ItemCategories.Upgrade:
                    AddUpgrade(item as Upgrade);
                    break;

                default: throw new System.ArgumentException($"Unknown category of item: {item.Category}.");
            }
        }

        private void AddUpgrade(Upgrade upgrade)
        {
            if (_upgrades.Contains(upgrade))
            {
                throw new System.InvalidOperationException($"Player already owns an upgrade by the name of {upgrade.Name}.");
            }

            _upgrades.Add(upgrade);
        }
    }
}