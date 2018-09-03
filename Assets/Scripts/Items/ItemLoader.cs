using Items;
using Items.ItemActors;
using UnityEngine;

public class ItemLoader : Singleton<ItemLoader>
{
    public GameObject CopperPiece;
    public GameObject SilverPiece;
    public GameObject GoldPiece;
    public GameObject PlatinumPiece;

    public static ItemActor CreateItem(ItemActorTypes type, IntVector2 worldPosition)
    {
        GameObject itemObject;

        switch (type)
        {
            case ItemActorTypes.CopperPiece: itemObject = Instance.CopperPiece; break;
            case ItemActorTypes.SilverPiece: itemObject = Instance.SilverPiece; break;
            case ItemActorTypes.GoldPiece: itemObject = Instance.GoldPiece; break;
            case ItemActorTypes.PlatinumPiece: itemObject = Instance.PlatinumPiece; break;
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
