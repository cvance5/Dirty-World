using System.Collections.Generic;

namespace Items
{
    public class NamedItem : Item
    {
        public string Name { get; }

        public bool IsUnique { get; protected set; }

        public NamedItem(string name, ItemCategories Category, uint amount)
            : base(Category, amount)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var item = obj as NamedItem;
            return item != null &&
                   Name == item.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = -1278267636;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public override string ToString() => Name;
    }
}