using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Shop Display")]
    [SerializeField] private string itemName = "New Item";

    [Header("Category")]
    [SerializeField] private ShopCategory category;

    [Header("Furniture Prefab")]
    [SerializeField] private PlaceableItem placeablePrefab;

    // Other scripts can read the item name.
    public string ItemName => itemName;

    // Other scripts can read what category this item belongs to.
    public ShopCategory Category => category;

    // Other scripts can read which prefab should be placed.
    public PlaceableItem PlaceablePrefab => placeablePrefab;

    // Price comes from the PlaceableItem prefab.
    public int Price
    {
        get
        {
            if (placeablePrefab == null)
            {
                return 0;
            }

            return placeablePrefab.Price;
        }
    }
}