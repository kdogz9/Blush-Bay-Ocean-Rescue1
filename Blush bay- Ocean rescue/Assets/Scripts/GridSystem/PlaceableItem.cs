using UnityEngine;

public class PlaceableItem : MonoBehaviour
{
    [Header("Item Info")] [SerializeField] private string itemName = "Coral";

    [Header("Shop Price")] [SerializeField]
    private int price = 20;

    [Header("Grid Size")] [SerializeField] private Vector2Int sizeInCells = new Vector2Int(1, 1);
    

    // Other scripts can read the item name.
    public string ItemName => itemName;

    // Other scripts can read the item price.
    public int Price => price;

    // Other scripts can read the item grid footprint.
    public Vector2Int SizeInCells => sizeInCells;
}

    