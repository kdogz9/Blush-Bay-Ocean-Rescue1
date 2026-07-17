using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button openShopButton;
    [SerializeField] private Button closeShopButton;

    [Header("Category Heading")]
    [SerializeField] private TMP_Text categoryHeadingText;
    
    [Header("Scroll View")]
    [SerializeField] private Transform contentParent;

    [Header("Placement System")]
    [SerializeField] private PlacementManager placementManager;

    [Header("Starting Category")]
    [SerializeField] private ShopCategory startingCategory = ShopCategory.Tanks;

    [Header("Optional Message Text")]
    [SerializeField] private TMP_Text shopMessageText;
    
    [Header("Scroll View Controller")]
    [SerializeField] private ShopScrollController shopScrollController;

    private readonly List<ShopItemButton> shopItems = new List<ShopItemButton>();

    private ShopCategory currentCategory;

    private void Start()
    {
        currentCategory = startingCategory;

        // Find the shop item cards inside Content.
        FindShopItems();

        // Hide shop at game start.
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // Connect open button.
        if (openShopButton != null)
        {
            openShopButton.onClick.AddListener(OpenShop);
        }

        // Connect close button.
        if (closeShopButton != null)
        {
            closeShopButton.onClick.AddListener(CloseShop);
        }

        ShowMessage("");
    }

    private void FindShopItems()
    {
        shopItems.Clear();

        if (contentParent == null)
        {
            Debug.LogWarning("ShopManager: Content Parent is missing.");
            return;
        }

        // true means it can find inactive shop items too.
        ShopItemButton[] foundItems = contentParent.GetComponentsInChildren<ShopItemButton>(true);

        foreach (ShopItemButton item in foundItems)
        {
            shopItems.Add(item);
        }

        Debug.Log("ShopManager found " + shopItems.Count + " shop items.");
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        // Refresh the list when opening the shop.
        FindShopItems();

        // Show the current category.
        ShowCategory(currentCategory);

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
        currentCategory = categoryToShow;

        int itemsShown = 0;

        foreach (ShopItemButton item in shopItems)
        {
            if (item == null) continue;

            bool shouldShow = item.Category == categoryToShow;

            item.SetVisible(shouldShow);

            if (shouldShow)
            {
                itemsShown++;
            }
        }

        // Change the heading text to match the selected category.
        UpdateCategoryHeading(categoryToShow);

        if (contentParent is RectTransform contentRect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        }

        Debug.Log("Showing category: " + categoryToShow + " | Items shown: " + itemsShown);
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

        if (CurrencyManager.Instance != null)
        {
            if (!CurrencyManager.Instance.CanAfford(itemData.Price))
            {
                ShowMessage("Not enough shells!");
                return;
            }
        }

        if (placementManager != null)
        {
            placementManager.BeginPlacement(itemData.PlaceablePrefab);

            ShowMessage("Choose a place!");

            CloseShop();
        }
        else
        {
            ShowMessage("Placement missing!");
            Debug.LogWarning("No PlacementManager assigned to ShopManager.");
        }
    }
    
    private void UpdateCategoryHeading(ShopCategory category)
    {
        if (categoryHeadingText == null) return;

        if (category == ShopCategory.Tanks)
        {
            categoryHeadingText.text = "TANKS";
        }
        else if (category == ShopCategory.Machines)
        {
            categoryHeadingText.text = "MACHINES";
        }
        else if (category == ShopCategory.Decorations)
        {
            categoryHeadingText.text = "DECOR";
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