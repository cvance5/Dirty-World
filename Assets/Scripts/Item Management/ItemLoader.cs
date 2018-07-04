using UnityEngine;

public class ItemLoader : Singleton<ItemLoader>
{
    public GameObject GoldPiece;

    public static GameObject CreateItem(ItemTypes type, IntVector2 worldPosition)
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

        return itemObject;
    }

}
