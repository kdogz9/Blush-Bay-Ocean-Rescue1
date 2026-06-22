using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishInfoUI : MonoBehaviour
{
    // Lets other scripts open this UI
    public static FishInfoUI Instance;

    [Header("UI Objects")]
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text fishNameText;

    [SerializeField] private Image fishUIImage;

    [SerializeField] private Slider healthSlider;

    [SerializeField] private Button treatButton;
    [SerializeField] private Button releaseButton;
    [SerializeField] private Button closeButton;

    // The tank we are currently looking at
    private Tank currentTank;

    private void Awake()
    {
        Instance = this;

        // Make the buttons run code when clicked
        treatButton.onClick.AddListener(TreatFish);
        releaseButton.onClick.AddListener(ReleaseFish);
        closeButton.onClick.AddListener(ClosePanel);

        // Hide panel when game starts
        panel.SetActive(false);
    }

    public void OpenPanel(Tank tank)
    {
        // Remember which tank we opened
        currentTank = tank;

        // Show the panel
        panel.SetActive(true);

        // Refresh UI
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Safety check
        if (currentTank == null) return;

        // If the tank is empty
        if (!currentTank.HasFish)
        {
            fishNameText.text = "EMPTY";

            // Hide fish image because there is no fish
            fishUIImage.gameObject.SetActive(false);

            // Hide health bar because there is no health to show
            healthSlider.gameObject.SetActive(false);

            // Keep buttons visible, but grey out Treat and Release
            treatButton.gameObject.SetActive(true);
            releaseButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);

            treatButton.interactable = false;
            releaseButton.interactable = false;
            closeButton.interactable = true;

            return;
        }

        // If the tank has a fish
        fishNameText.text = currentTank.FishName;

        // Show fish image
        fishUIImage.gameObject.SetActive(true);

        // Put the tank fish sprite onto the UI image
        fishUIImage.sprite = currentTank.FishSpriteImage;

        // Show health bar and buttons
        healthSlider.gameObject.SetActive(true);
        treatButton.gameObject.SetActive(true);
        releaseButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);

        // Update health bar
        healthSlider.maxValue = currentTank.MaxHealth;
        healthSlider.value = currentTank.Health;

        // Treat works while fish exists
        treatButton.interactable = true;

        // Release only works when fish is fully healed
        releaseButton.interactable = currentTank.ReadyToRelease;

        // Close always works
        closeButton.interactable = true;
    }

    private void TreatFish()
    {
        currentTank.TreatFish();

        UpdateUI();
    }

    private void ReleaseFish()
    {
        currentTank.ReleaseFish();

        UpdateUI();
    }

    private void ClosePanel()
    {
        panel.SetActive(false);

        currentTank = null;
    }
}