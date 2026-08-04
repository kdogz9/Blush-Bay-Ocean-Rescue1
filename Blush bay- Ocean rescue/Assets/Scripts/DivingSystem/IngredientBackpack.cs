using System;
using UnityEngine;

public class IngredientBackpack : MonoBehaviour
{
    // This lets other scripts find the backpack easily.
    public static IngredientBackpack Instance;

    [Header("Ingredient Amounts")]
    [SerializeField] private int kelpAmount = 0;
    [SerializeField] private int seaweedAmount = 0;

    [Header("New Item Notification")]
    [SerializeField] private bool hasNewItems = false;
    [SerializeField] private string latestNotification = "";

    // Other scripts can listen for changes.
    // For example, the backpack UI can update when kelp is collected.
    public event Action OnBackpackChanged;

    public int KelpAmount => kelpAmount;
    public int SeaweedAmount => seaweedAmount;
    public bool HasNewItems => hasNewItems;
    public string LatestNotification => latestNotification;

    private void Awake()
    {
        // If a backpack already exists, destroy this duplicate.
        // This stops Unity creating two backpacks when changing scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Save this as the main backpack.
        Instance = this;

        // Keep this backpack alive when moving between scenes.
        DontDestroyOnLoad(gameObject);
    }

    public void AddIngredient(IngredientType ingredientType, int amount)
    {
        // Do nothing if the amount is 0 or less.
        if (amount <= 0) return;

        if (ingredientType == IngredientType.Kelp)
        {
            kelpAmount += amount;
            latestNotification = "+" + amount.ToString() + " Kelp";
        }
        else if (ingredientType == IngredientType.Seaweed)
        {
            seaweedAmount += amount;
            latestNotification = "+" + amount.ToString() + " Seaweed";
        }

        // Mark that the player has new items in the backpack.
        hasNewItems = true;

        Debug.Log("Backpack received: " + latestNotification);

        // Tell the UI to refresh.
        OnBackpackChanged?.Invoke();
    }

    public int GetIngredientAmount(IngredientType ingredientType)
    {
        if (ingredientType == IngredientType.Kelp)
        {
            return kelpAmount;
        }

        if (ingredientType == IngredientType.Seaweed)
        {
            return seaweedAmount;
        }

        return 0;
    }

    public bool TryRemoveIngredient(IngredientType ingredientType, int amount)
    {
        // This will be useful later for machines.
        // Example: food machine uses 2 kelp.

        if (amount <= 0) return false;

        if (ingredientType == IngredientType.Kelp)
        {
            if (kelpAmount < amount) return false;

            kelpAmount -= amount;
        }
        else if (ingredientType == IngredientType.Seaweed)
        {
            if (seaweedAmount < amount) return false;

            seaweedAmount -= amount;
        }

        Debug.Log("Removed " + amount + " " + ingredientType + " from backpack.");

        OnBackpackChanged?.Invoke();

        return true;
    }

    public void ClearNewItemNotification()
    {
        // This is called when the player opens the backpack.
        hasNewItems = false;
        latestNotification = "";

        OnBackpackChanged?.Invoke();
    }
}