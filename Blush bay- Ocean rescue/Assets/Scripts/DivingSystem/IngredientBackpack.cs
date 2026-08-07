using System;
using UnityEngine;

public class IngredientBackpack : MonoBehaviour
{
    public static IngredientBackpack Instance;

    [Header("Raw Ingredients")]
    [SerializeField] private int kelpAmount = 0;
    [SerializeField] private int seaweedAmount = 0;

    [Header("Sanitised Ingredients")]
    [SerializeField] private int sanitisedKelpAmount = 0;
    [SerializeField] private int sanitisedSeaweedAmount = 0;

    [Header("Notification")]
    [SerializeField] private bool hasNewItems = false;
    [SerializeField] private string latestNotification = "";

    public event Action OnBackpackChanged;

    public int KelpAmount => kelpAmount;
    public int SeaweedAmount => seaweedAmount;
    public int SanitisedKelpAmount => sanitisedKelpAmount;
    public int SanitisedSeaweedAmount => sanitisedSeaweedAmount;

    public bool HasNewItems => hasNewItems;
    public string LatestNotification => latestNotification;

    private void Awake()
    {
        // Stops duplicate backpacks when moving between scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keeps the backpack alive between Aquarium and Diving scenes.
        DontDestroyOnLoad(gameObject);
    }

    public void AddIngredient(IngredientType ingredientType, int amount)
    {
        if (amount <= 0) return;

        if (ingredientType == IngredientType.Kelp)
        {
            kelpAmount += amount;
            latestNotification = "+" + amount + " Kelp";
        }
        else if (ingredientType == IngredientType.Seaweed)
        {
            seaweedAmount += amount;
            latestNotification = "+" + amount + " Seaweed";
        }
        else if (ingredientType == IngredientType.SanitisedKelp)
        {
            sanitisedKelpAmount += amount;
            latestNotification = "+" + amount + " Clean Kelp";
        }
        else if (ingredientType == IngredientType.SanitisedSeaweed)
        {
            sanitisedSeaweedAmount += amount;
            latestNotification = "+" + amount + " Clean Seaweed";
        }

        hasNewItems = true;

        Debug.Log("Backpack added: " + latestNotification);

        OnBackpackChanged?.Invoke();
    }

    public bool TryRemoveIngredient(IngredientType ingredientType, int amount)
    {
        if (amount <= 0) return false;

        if (ingredientType == IngredientType.Kelp)
        {
            if (kelpAmount < amount)
            {
                Debug.Log("Not enough raw kelp.");
                return false;
            }

            kelpAmount -= amount;
        }
        else if (ingredientType == IngredientType.Seaweed)
        {
            if (seaweedAmount < amount)
            {
                Debug.Log("Not enough raw seaweed.");
                return false;
            }

            seaweedAmount -= amount;
        }
        else if (ingredientType == IngredientType.SanitisedKelp)
        {
            if (sanitisedKelpAmount < amount)
            {
                Debug.Log("Not enough clean kelp.");
                return false;
            }

            sanitisedKelpAmount -= amount;
        }
        else if (ingredientType == IngredientType.SanitisedSeaweed)
        {
            if (sanitisedSeaweedAmount < amount)
            {
                Debug.Log("Not enough clean seaweed.");
                return false;
            }

            sanitisedSeaweedAmount -= amount;
        }

        Debug.Log("Backpack removed: " + amount + " " + ingredientType);

        OnBackpackChanged?.Invoke();

        return true;
    }

    public void ClearNewItemNotification()
    {
        hasNewItems = false;
        latestNotification = "";

        OnBackpackChanged?.Invoke();
    }
}