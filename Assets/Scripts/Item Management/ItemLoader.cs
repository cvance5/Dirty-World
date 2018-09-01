using ItemManagement;
using UnityEngine;

public class ItemLoader : Singleton<ItemLoader>
{
    public GameObject CopperPiece;
    public GameObject SilverPiece;
    public GameObject GoldPiece;
    public GameObject PlatinumPiece;

    public static ItemActor CreateItem(ItemType type, IntVector2 worldPosition)
    {
        GameObject itemObject;

        switch (type)
        {
            case ItemType.CopperPiece: itemObject = Instance.CopperPiece; break;
            case ItemType.SilverPiece: itemObject = Instance.SilverPiece; break;
            case ItemType.GoldPiece: itemObject = Instance.GoldPiece; break;
            case ItemType.PlatinumPiece: itemObject = Instance.PlatinumPiece; break;
            default: throw new System.ArgumentException($"Unknown item type of {type}.");
        }

        itemObject = Instantiate(itemObject);
        itemObject.transform.position = worldPosition;
        itemObject.name = $"[{worldPosition.X}, {worldPosition.Y}]";

        var item = itemObject.GetComponent<ItemActor>();

        _log.ErrorIfNull(item, $"Item of type {type} has not been given a 'item' component.");

        return item;
    }

    private static readonly Log _log = new Log("ItemLoader");
}
