using UnityEngine;
using TMPro;

public class DonationManager : MonoBehaviour
{
    // This lets other scripts easily find the DonationManager
    public static DonationManager Instance;

    [Header("Donation Settings")]
    [SerializeField] private int totalDonations = 0;
// creates boxes to change the values in the inspector 
    
    [Header("Optional UI Text")]
    [SerializeField] private TMP_Text donationText;

    private void Awake()
    {
        // Store this DonationManager so other scripts can use it
        Instance = this;

        // Update the UI when the game starts
        UpdateDonationUI();
    }

    public void AddDonation(int amount)
    {
        // Add the visitor's donation to the total
        totalDonations += amount;

        // Show the new total in the console for testing
        Debug.Log("Donation received: +" + amount + " | Total donations: " + totalDonations);

        // Update the UI text to have the donation added
        UpdateDonationUI();
    }

    private void UpdateDonationUI()
    {
        donationText.text = totalDonations.ToString();
    }
}