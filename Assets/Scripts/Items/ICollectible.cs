using System.Collections.Generic;

namespace Items
{
    public interface ICollectible
    {
        List<Item> CollectedItems { get; }
        void OnCollect();
    }
}