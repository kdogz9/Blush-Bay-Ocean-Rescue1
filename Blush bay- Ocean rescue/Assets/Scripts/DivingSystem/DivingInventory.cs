using UnityEngine;
using TMPro;

public class DivingInventory : MonoBehaviour
{
    // This lets other scripts easily find the inventory
    public static DivingInventory Instance;

    [Header("Ingredient Amounts")]
    [SerializeField] private int kelpAmount = 0;
    [SerializeField] private int seaweedAmount = 0;

    [Header("UI Text")]
    [SerializeField] private TMP_Text kelpText;
    [SerializeField] private TMP_Text seaweedText;

    private void Awake()
    {
        // Save this as the main diving inventory
        Instance = this;
    }

    private void Start()
    {
        // Make sure the UI starts with the correct numbers
        UpdateUI();
    }

    public void AddKelp(int amount)
    {
        kelpAmount += amount;

        Debug.Log("Collected kelp. Total kelp: " + kelpAmount);

        UpdateUI();
    }

    public void AddSeaweed(int amount)
    {
        seaweedAmount += amount;

        Debug.Log("Collected seaweed. Total seaweed: " + seaweedAmount);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (kelpText != null)
        {
            kelpText.text = "x" + kelpAmount.ToString();
        }

        if (seaweedText != null)
        {
            seaweedText.text = "x" + seaweedAmount.ToString();
        }
    }
}