using Items;
using Items.ItemActors;
using UnityEngine;

public class ItemLoader : Singleton<ItemLoader>
{
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
    [SerializeField]
    private GameObject _copperPiece = null;
    [SerializeField]
    private GameObject _silverPiece = null;
    [SerializeField]
    private GameObject _goldPiece = null;
    [SerializeField]
    private GameObject _platinumPiece = null;
#pragma warning restore IDE0044 // Add readonly modifier

    public static ItemActor CreateItem(ItemActorTypes type, IntVector2 worldPosition)
    {
        GameObject itemObject;

        switch (type)
        {
            case ItemActorTypes.CopperPiece: itemObject = Instance._copperPiece; break;
            case ItemActorTypes.SilverPiece: itemObject = Instance._silverPiece; break;
            case ItemActorTypes.GoldPiece: itemObject = Instance._goldPiece; break;
            case ItemActorTypes.PlatinumPiece: itemObject = Instance._platinumPiece; break;
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
