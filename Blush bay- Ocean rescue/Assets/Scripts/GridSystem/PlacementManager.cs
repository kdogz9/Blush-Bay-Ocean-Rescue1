using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlacementManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Grid grid;
    [SerializeField] private Camera mainCamera;

    [Header("Test Item")]
    [SerializeField] private PlaceableItem testItemPrefab;

    [Header("Blocked Placement")]
    [SerializeField] private LayerMask blockedPlacementLayers;

    [Header("Preview Colours")]
    [SerializeField] private Color validPreviewColour = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color invalidPreviewColour = new Color(1f, 0.3f, 0.3f, 0.6f);

    [Header("Selling")]
    [SerializeField] private bool isSellMode = false;

    // This stores the item we are currently trying to place.
    private PlaceableItem currentItemPrefab;

    // This is the ghost/preview version that follows the mouse.
    private PlaceableItem currentPreview;

    // This remembers the current grid cell under the mouse.
    private Vector3Int currentCell;

    // This tells us if we are currently placing an item.
    private bool isPlacing = false;

    // This stores grid cells that already have furniture on them.
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();

    private void Start()
    {
        // If no camera was added in the Inspector, use the main camera.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        // If sell mode is active, handle selling first.
        if (isSellMode)
        {
            HandleSellMode();
            return;
        }

        // TEST:
        // Press P to begin placing the test decoration.
        // Later, the shop button will start placement instead.
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            BeginPlacement(testItemPrefab);
        }

        // If we are not placing anything, stop here.
        if (!isPlacing) return;

        // Keep the preview following the mouse.
        UpdatePreviewPosition();

        // Left click places the item.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceItem();
        }

        // Right click or Escape cancels placement.
        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }

    public void BeginPlacement(PlaceableItem itemPrefab)
    {
        // If no item was given, stop.
        if (itemPrefab == null)
        {
            Debug.LogWarning("No item prefab assigned for placement.");
            return;
        }

        // Stop sell mode if it was active.
        isSellMode = false;

        // If we were already placing something, cancel the old preview first.
        if (isPlacing)
        {
            CancelPlacement();
        }

        // Store the item we want to place.
        currentItemPrefab = itemPrefab;

        // Create the preview item in the scene.
        currentPreview = Instantiate(currentItemPrefab);

        // Turn the object into a preview:
        // colliders off, transparent colour, etc.
        SetPreviewMode(currentPreview, true);

        // We are now in placement mode.
        isPlacing = true;

        Debug.Log("Started placing: " + currentItemPrefab.ItemName);
    }

    public void BeginSellMode()
    {
        // If placement mode is active, stop it first.
        if (isPlacing)
        {
            CancelPlacement();
        }

        isSellMode = true;

        Debug.Log("Sell mode started. Click a placed item to sell it.");
    }

    public void CancelSellMode()
    {
        isSellMode = false;

        Debug.Log("Sell mode cancelled.");
    }

    private void HandleSellMode()
    {
        // Right click or Escape cancels sell mode.
        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelSellMode();
            return;
        }

        // Left click tries to sell the item under the mouse.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Do not sell if clicking on UI.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            TrySellItemUnderMouse();
        }
    }

    private void TrySellItemUnderMouse()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        // Check every collider under the mouse.
        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPosition);

        if (hits.Length == 0)
        {
            Debug.Log("No placed item clicked.");
            return;
        }

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Clicked collider: " + hit.name);

            PlacedItem placedItem = hit.GetComponentInParent<PlacedItem>();

            if (placedItem != null)
            {
                placedItem.Sell();
                return;
            }
        }

        Debug.Log("Clicked object is not a sellable placed item.");
    }

    public void SellPlacedItem(PlacedItem placedItem)
    {
        if (placedItem == null) return;

        // Give the player the FULL item price back.
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddMoney(placedItem.SellPrice);

// Show money refunded popup.
            if (MoneyPopUpManager.Instance != null)
            {
                MoneyPopUpManager.Instance.ShowMoneyChange(placedItem.SellPrice, true);
            }
            else
            {
                Debug.LogWarning("No MoneyPopUpManager found in the scene.");
            }
        }

        // Free the grid cells this item was using.
        List<Vector3Int> cellsToFree = placedItem.GetOccupiedCells();

        foreach (Vector3Int cell in cellsToFree)
        {
            occupiedCells.Remove(cell);
        }

        Debug.Log("Sold " + placedItem.ItemName + " for " + placedItem.SellPrice + " shells.");

        // Remove the item from the scene.
        Destroy(placedItem.gameObject);

        // Exit sell mode after selling one item.
        CancelSellMode();
    }

    private void UpdatePreviewPosition()
    {
        // Find which grid cell the mouse is over.
        currentCell = GetMouseGridCell();

        // Convert that grid cell to a world position.
        Vector3 worldPosition = GetWorldPositionForItem(currentCell, currentItemPrefab);

        // Move the preview to that position.
        currentPreview.transform.position = worldPosition;

        // Check if the item can be placed here.
        bool canPlace = CanPlaceAt(currentCell, currentItemPrefab);

        // Change the preview colour based on whether the position is valid.
        if (canPlace)
        {
            SetPreviewColour(currentPreview, validPreviewColour);
        }
        else
        {
            SetPreviewColour(currentPreview, invalidPreviewColour);
        }
    }

    private void TryPlaceItem()
    {
        // If the mouse is over UI, do not place the item.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Check if this position is valid.
        if (!CanPlaceAt(currentCell, currentItemPrefab))
        {
            Debug.Log("Cannot place item here.");
            return;
        }

        // Check money before placing.
        if (CurrencyManager.Instance != null)
        {
            bool paidSuccessfully = CurrencyManager.Instance.TrySpendMoney(currentItemPrefab.Price);

// If the player cannot afford it, stop here.
            if (!paidSuccessfully)
            {
                Debug.Log("Not enough shells to place this item.");
                return;
            }

// Show money taken popup.
            if (MoneyPopUpManager.Instance != null)
            {
                MoneyPopUpManager.Instance.ShowMoneyChange(currentItemPrefab.Price, false);
            }
            else
            {
                Debug.LogWarning("No MoneyPopUpManager found in the scene.");
            }
        }

        // Get the position where the item should be placed.
        Vector3 placePosition = GetWorldPositionForItem(currentCell, currentItemPrefab);

        // Create the real placed item.
        PlaceableItem placedItem = Instantiate(currentItemPrefab, placePosition, Quaternion.identity);

        // Make sure the real item is not transparent and has its collider on.
        SetPreviewMode(placedItem, false);

        // Get the cells this item is using.
        List<Vector3Int> cellsUsed = GetCellsForItem(currentCell, placedItem);

        // Add selling data to the placed item.
        PlacedItem placedItemData = placedItem.GetComponent<PlacedItem>();

        if (placedItemData == null)
        {
            placedItemData = placedItem.gameObject.AddComponent<PlacedItem>();
        }

        // FULL REFUND:
        // The sell price is the same as the original price.
        int sellPrice = placedItem.Price;

        placedItemData.Setup(
            placedItem.ItemName,
            placedItem.Price,
            sellPrice,
            cellsUsed,
            this
        );

        // Mark the grid cells as occupied so nothing else can be placed there.
        MarkCellsAsOccupied(currentCell, placedItem);

        Debug.Log("Placed item: " + placedItem.ItemName);

        // Stop placement after placing one item.
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        // Destroy the preview object if it exists.
        if (currentPreview != null)
        {
            Destroy(currentPreview.gameObject);
        }

        // Clear placement data.
        currentPreview = null;
        currentItemPrefab = null;
        isPlacing = false;

        Debug.Log("Placement cancelled or finished.");
    }

    private Vector3Int GetMouseGridCell()
    {
        // Read the mouse position on the screen.
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Convert screen position to world position.
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.z)
            )
        );

        // Keep placement on the 2D plane.
        mouseWorldPosition.z = 0f;

        // Convert the world position into a grid cell.
        return grid.WorldToCell(mouseWorldPosition);
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Read the mouse position on the screen.
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Convert screen position to world position.
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.z)
            )
        );

        // Keep it on the 2D plane.
        mouseWorldPosition.z = 0f;

        return mouseWorldPosition;
    }

    private Vector3 GetWorldPositionForItem(Vector3Int originCell, PlaceableItem item)
    {
        // Get the centre of the grid cell the mouse is over.
        Vector3 cellCentre = grid.GetCellCenterWorld(originCell);

        // Get the size of each grid cell.
        Vector3 cellSize = grid.cellSize;

        // If an item is bigger than 1x1, this keeps it centred across its footprint.
        float offsetX = (item.SizeInCells.x - 1) * cellSize.x * 0.5f;
        float offsetY = (item.SizeInCells.y - 1) * cellSize.y * 0.5f;

        return cellCentre + new Vector3(offsetX, offsetY, 0f);
    }

    private bool CanPlaceAt(Vector3Int originCell, PlaceableItem item)
    {
        // Check every grid cell the item would cover.
        List<Vector3Int> cellsToCheck = GetCellsForItem(originCell, item);

        foreach (Vector3Int cell in cellsToCheck)
        {
            // If another placed item already uses this cell, placement is blocked.
            if (occupiedCells.Contains(cell))
            {
                return false;
            }
        }

        // Check if the item overlaps anything on a blocked layer.
        Vector3 worldPosition = GetWorldPositionForItem(originCell, item);

        Vector2 checkSize = new Vector2(
            item.SizeInCells.x * grid.cellSize.x * 0.45f,
            item.SizeInCells.y * grid.cellSize.y * 0.45f
        );

        Collider2D blockedHit = Physics2D.OverlapBox(
            worldPosition,
            checkSize,
            0f,
            blockedPlacementLayers
        );

        // If we hit a blocked object, we cannot place here.
        if (blockedHit != null)
        {
            return false;
        }

        // Nothing blocked the placement, so this position is valid.
        return true;
    }

    private List<Vector3Int> GetCellsForItem(Vector3Int originCell, PlaceableItem item)
    {
        List<Vector3Int> cells = new List<Vector3Int>();

        // Example:
        // If item is 1x1, this adds 1 cell.
        // If item is 2x1, this adds 2 cells.
        // If item is 2x2, this adds 4 cells.
        for (int x = 0; x < item.SizeInCells.x; x++)
        {
            for (int y = 0; y < item.SizeInCells.y; y++)
            {
                Vector3Int cell = originCell + new Vector3Int(x, y, 0);
                cells.Add(cell);
            }
        }

        return cells;
    }

    private void MarkCellsAsOccupied(Vector3Int originCell, PlaceableItem item)
    {
        List<Vector3Int> cellsToMark = GetCellsForItem(originCell, item);

        foreach (Vector3Int cell in cellsToMark)
        {
            occupiedCells.Add(cell);
        }
    }

    private void SetPreviewMode(PlaceableItem item, bool previewMode)
    {
        // Turn colliders off for the preview.
        // This stops the preview blocking itself.
        Collider2D[] colliders = item.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = !previewMode;
        }

        // Set colour based on whether this is a preview or real item.
        if (previewMode)
        {
            SetPreviewColour(item, validPreviewColour);
        }
        else
        {
            SetPreviewColour(item, Color.white);
        }
    }

    private void SetPreviewColour(PlaceableItem item, Color colour)
    {
        // Get all SpriteRenderers on this item and its children.
        SpriteRenderer[] spriteRenderers = item.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = colour;
        }
    }
}