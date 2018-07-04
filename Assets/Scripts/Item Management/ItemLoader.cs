using ItemManagement;
using UnityEngine;

public class ItemLoader : Singleton<ItemLoader>
{
    public GameObject GoldPiece;

    public static Item CreateItem(ItemTypes type, IntVector2 worldPosition)
    {
        GameObject itemObject;

        switch (type)
        {
            case ItemTypes.GoldPiece: itemObject = Instance.GoldPiece; break;
            default: throw new System.ArgumentException($"Unknown item type of {type}.");
        }

        itemObject = Instantiate(itemObject);
        itemObject.transform.position = worldPosition;
        itemObject.name = $"[{worldPosition.X}, {worldPosition.Y}]";

        var item = itemObject.GetComponent<Item>();

        Log.ErrorIfNull(item, $"Item of type {type} has not been given a 'item' component.");

        return item;
    }
}
