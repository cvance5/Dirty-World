using System.Collections.Generic;

namespace ItemManagement
{
    public interface ICollectible
    {
        Dictionary<CollectibleType, int> CollectedItems { get; }
        void OnCollect();
    }
}