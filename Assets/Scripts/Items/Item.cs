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

        public override string ToString()
        {
            return $"{Amount} {Category}";
        }

        public override bool Equals(object obj)
        {
            return obj is Item ? Equals(obj as Item) : false;
        }

        public bool Equals(Item other)
        {
            var itemType = other.GetType();

            if (itemType.IsSubclassOf(typeof(Item)))
            {
                // Any derived item type has more specific equality requirements that are impossible to 
                // compare to a generic, nameless item.  As such, generic nameless items are never equal
                // to a specific derived item.
                return false;
            }
            else return Category == other.Category;
        }

        public override int GetHashCode()
        {
            var hashCode = -1317372766;
            hashCode = hashCode * -1521134295 + Category.GetHashCode();
            hashCode = hashCode * -1521134295 + Amount.GetHashCode();
            return hashCode;
        }
    }
}