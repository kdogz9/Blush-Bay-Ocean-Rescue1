using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    [Header("Shop Item Data")]
    [SerializeField] private ShopItemData shopItemData;

    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemPriceText;

    [Header("Status Image")]
    [SerializeField] private Image statusImage;
    [SerializeField] private Sprite canBuySprite;
    [SerializeField] private Sprite cannotBuySprite;

    [Header("Item Click Button")]
    [SerializeField] private Button itemButton;

    [Header("Shop Manager")]
    [SerializeField] private ShopManager shopManager;

    // This lets the ShopManager check what category this item belongs to.
    public ShopCategory Category
    {
        get
        {
            if (shopItemData == null)
            {
                return ShopCategory.Decorations;
            }

            return shopItemData.Category;
        }
    }

    private void Start()
    {
        // Fill in the card visuals when the game starts.
        RefreshUI();

        // Make the item card clickable.
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(BuyItem);
        }
    }

    private void Update()
    {
        // Keep the status image updated as money changes.
        UpdatePurchaseStatus();
    }

    public void RefreshUI()
    {
        if (shopItemData == null)
        {
            Debug.LogWarning(name + " has no ShopItemData assigned.");
            return;
        }

        // Set the item name.
        if (itemNameText != null)
        {
            itemNameText.text = shopItemData.ItemName;
        }

        // Set the price text.
        if (itemPriceText != null)
        {
            itemPriceText.text = shopItemData.Price.ToString();
        }

        // Set the icon from the prefab's SpriteRenderer.
        SetIconFromFurniturePrefab();

        // Update if the player can afford it.
        UpdatePurchaseStatus();
    }

    private void SetIconFromFurniturePrefab()
    {
        if (itemIcon == null) return;
        if (shopItemData == null) return;
        if (shopItemData.PlaceablePrefab == null) return;

        // Find the sprite on the furniture prefab.
        SpriteRenderer spriteRenderer = shopItemData.PlaceablePrefab.GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning(shopItemData.ItemName + " has no SpriteRenderer for the shop icon.");
            return;
        }

        // Put that sprite into your UI Image.
        itemIcon.sprite = spriteRenderer.sprite;
        itemIcon.preserveAspect = true;
        itemIcon.enabled = true;
    }

    private void UpdatePurchaseStatus()
    {
        if (statusImage == null) return;
        if (shopItemData == null) return;
        if (CurrencyManager.Instance == null) return;

        // Check if player has enough shells.
        bool canAfford = CurrencyManager.Instance.CanAfford(shopItemData.Price);

        statusImage.gameObject.SetActive(true);

        if (canAfford)
        {
            statusImage.sprite = canBuySprite;
        }
        else
        {
            statusImage.sprite = cannotBuySprite;
        }

        statusImage.preserveAspect = true;
    }

    private void BuyItem()
    {
        Debug.Log("Clicked shop item: " + name);

        if (shopItemData == null) return;

        if (shopManager == null)
        {
            Debug.LogWarning("No ShopManager assigned on " + name);
            return;
        }

        // Ask the shop manager to start buying/placing this item.
        shopManager.TryBuyItem(shopItemData);
    }

    public void SetVisible(bool isVisible)
    {
        // This is how the category system hides/shows shop items.
        gameObject.SetActive(isVisible);
    }
}