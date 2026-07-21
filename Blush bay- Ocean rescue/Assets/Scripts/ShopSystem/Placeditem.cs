using System.Collections.Generic;
using UnityEngine;

public class PlacedItem : MonoBehaviour
{
    [Header("Item Info")]
    [SerializeField] private string itemName;
    [SerializeField] private int originalPrice;
    [SerializeField] private int sellPrice;

    private PlacementManager placementManager;
    private List<Vector3Int> occupiedCells = new List<Vector3Int>();

    public string ItemName => itemName;
    public int SellPrice => sellPrice;

    public void Setup(
        string newItemName,
        int newOriginalPrice,
        int newSellPrice,
        List<Vector3Int> newOccupiedCells,
        PlacementManager newPlacementManager
    )
    {
        itemName = newItemName;
        originalPrice = newOriginalPrice;
        sellPrice = newSellPrice;
        occupiedCells = new List<Vector3Int>(newOccupiedCells);
        placementManager = newPlacementManager;
    }

    public List<Vector3Int> GetOccupiedCells()
    {
        return occupiedCells;
    }

    public void Sell()
    {
        if (placementManager != null)
        {
            placementManager.SellPlacedItem(this);
        }
    }
}