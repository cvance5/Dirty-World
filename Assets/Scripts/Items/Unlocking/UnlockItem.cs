namespace Items.Unlocking
{
    public class UnlockItem : NamedItem
    {
        public UnlockTypes UnlockType { get; }

        public UnlockItem(string name, UnlockTypes type)
            : base(name, ItemCategories.Unlock, 1)
        {
            IsUnique = true;
            UnlockType = type;
        }
     }
}