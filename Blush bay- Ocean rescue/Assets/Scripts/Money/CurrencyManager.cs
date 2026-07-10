using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    // This lets other scripts find the CurrencyManager easily.
    public static CurrencyManager Instance;

    [Header("Starting Money")]
    [SerializeField] private int startingMoney = 100;

    [Header("Current Money")]
    [SerializeField] private int currentMoney;

    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;

    private void Awake()
    {
        // If there is already another CurrencyManager, destroy this one.
        // This prevents duplicates causing problems.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Store this CurrencyManager as the main one.
        Instance = this;

        // Give the player their starting money.
        currentMoney = startingMoney;

        // Update the UI so the player sees the correct starting amount.
        UpdateMoneyUI();
    }

    public int GetCurrentMoney()
    {
        // This lets other scripts check how much money the player has.
        return currentMoney;
    }

    public void AddMoney(int amount)
    {
        // Do not add negative money.
        if (amount <= 0) return;

        // Add the money.
        currentMoney += amount;

        // Update the UI.
        UpdateMoneyUI();

        Debug.Log("Money added: +" + amount + " | Total: " + currentMoney);
    }

    public bool CanAfford(int amount)
    {
        // This checks if the player has enough money.
        return currentMoney >= amount;
    }

    public bool TrySpendMoney(int amount)
    {
        // Do not spend zero or negative money.
        if (amount <= 0) return false;

        // If the player does not have enough money, stop here.
        if (!CanAfford(amount))
        {
            Debug.Log("Not enough shells!");
            return false;
        }

        // Take the money away.
        currentMoney -= amount;

        // Update the UI.
        UpdateMoneyUI();

        Debug.Log("Money spent: -" + amount + " | Total: " + currentMoney);

        // Return true so other scripts know the purchase worked.
        return true;
    }

    private void UpdateMoneyUI()
    {
        // If no text has been assigned, stop here.
        if (moneyText == null) return;

        // Show only the number because the shell icon is already beside it.
        moneyText.text = currentMoney.ToString();
    }
}