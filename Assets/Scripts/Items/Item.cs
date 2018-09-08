namespace Items
{
    public class Item
    {
        public ItemCategories Category { get; }
        public uint Amount { get; }

        public Item(ItemCategories category, uint amount)
        {
            Category = category;
            Amount = amount;
        }
    }
}