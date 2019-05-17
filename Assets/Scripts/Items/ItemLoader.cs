using Items;
using Items.ItemActors;
using MathConcepts;
using UnityEngine;
using Utilities.UnitySerialization;

public class ItemLoader : Singleton<ItemLoader>
{
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
    [SerializeField]
    private ItemActorTypesGameObjectDictionary _items =
        new ItemActorTypesGameObjectDictionary();
#pragma warning restore IDE0044 // Add readonly modifier

    public static ItemActor CreateItem(ItemActorTypes type, IntVector2 worldPosition)
    {
        if (!Instance._items.TryGetValue(type, out var itemObject))
        {
            _log.Error($"Could not find an object for itemActorType `{type}.");
        }

        itemObject = Instantiate(itemObject);
        itemObject.transform.position = worldPosition;
        itemObject.name = $"[{worldPosition.X}, {worldPosition.Y}]";

        var item = itemObject.GetComponent<ItemActor>();

        _log.ErrorIfNull(item, $"Item of type {type} has not been given a 'item' component.");

        return item;
    }

    private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("ItemLoader");
}
