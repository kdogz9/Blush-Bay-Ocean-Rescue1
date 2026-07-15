using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button openShopButton;
    [SerializeField] private Button closeShopButton;

    [Header("Placement System")]
    [SerializeField] private PlacementManager placementManager;

    [Header("Shop Items")]
    [SerializeField] private ShopItemButton[] shopItemButtons;

    [Header("Starting Category")]
    [SerializeField] private ShopCategory startingCategory = ShopCategory.Tanks;

    [Header("Optional Message Text")]
    [SerializeField] private TMP_Text shopMessageText;

    private void Start()
    {
        // Hide shop at the start.
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // Connect open shop button.
        if (openShopButton != null)
        {
            openShopButton.onClick.AddListener(OpenShop);
        }

        // Connect close shop button.
        if (closeShopButton != null)
        {
            closeShopButton.onClick.AddListener(CloseShop);
        }

        // If we forgot to assign shop items manually,
        // find them automatically inside the shop panel.
        if (shopItemButtons == null || shopItemButtons.Length == 0)
        {
            shopItemButtons = GetComponentsInChildren<ShopItemButton>(true);
        }

        ShowMessage("");
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        // When the shop opens, show the starting category first.
        ShowCategory(startingCategory);

        ShowMessage("");
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void ShowCategory(ShopCategory categoryToShow)
    {
        // Loop through every shop item card.
        foreach (ShopItemButton itemButton in shopItemButtons)
        {
            if (itemButton == null) continue;

            // Show the item only if it belongs to the selected category.
            bool shouldShow = itemButton.Category == categoryToShow;

            itemButton.SetVisible(shouldShow);
        }

        Debug.Log("Showing shop category: " + categoryToShow);
    }

    public void TryBuyItem(ShopItemData itemData)
    {
        if (itemData == null)
        {
            ShowMessage("Missing item!");
            return;
        }

        if (itemData.PlaceablePrefab == null)
        {
            ShowMessage("Missing prefab!");
            Debug.LogWarning(itemData.ItemName + " has no placeable prefab assigned.");
            return;
        }

        // Check if the player can afford it.
        if (CurrencyManager.Instance != null)
        {
            if (!CurrencyManager.Instance.CanAfford(itemData.Price))
            {
                ShowMessage("Not enough shells!");
                return;
            }
        }

        // Start grid placement.
        // Money is still spent later when the player actually places the item.
        if (placementManager != null)
        {
            placementManager.BeginPlacement(itemData.PlaceablePrefab);

            ShowMessage("Choose a place!");

            // Close shop so the player can place the furniture.
            CloseShop();
        }
        else
        {
            ShowMessage("Placement missing!");
            Debug.LogWarning("No PlacementManager assigned to ShopManager.");
        }
    }

    private void ShowMessage(string message)
    {
        if (shopMessageText != null)
        {
            shopMessageText.text = message;
        }

        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log(message);
        }
    }
}