using Items;

namespace Economy
{
    public class Transaction
    {
        public Item Price { get; }
        public Item Reward { get; }

        public Transaction(Item price, Item reward)
        {
            Price = price;
            Reward = reward;
        }
    }
}