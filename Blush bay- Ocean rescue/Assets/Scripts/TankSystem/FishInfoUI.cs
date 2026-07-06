using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishInfoUI : MonoBehaviour
{
    public static FishInfoUI Instance;

    [Header("UI Objects")]
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private Image fishUIImage;

    [SerializeField] private Slider healthSlider;

    [SerializeField] private Button treatButton;
    [SerializeField] private Button releaseButton;
    [SerializeField] private Button closeButton;

    private Tank currentTank;

    private void Awake()
    {
        Instance = this;

        treatButton.onClick.AddListener(TreatFish);
        releaseButton.onClick.AddListener(ReleaseFish);
        closeButton.onClick.AddListener(ClosePanel);

        panel.SetActive(false);
    }

    public void OpenPanel(Tank tank)
    {
        currentTank = tank;
        panel.SetActive(true);
        UpdateUI();
    }

    public void RefreshUI()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentTank == null) return;

        // EMPTY TANK UI
        if (!currentTank.HasFish)
        {
            fishNameText.text = "EMPTY TANK";

            if (fishUIImage != null)
            {
                fishUIImage.gameObject.SetActive(false);
                fishUIImage.sprite = null;
            }

            healthSlider.gameObject.SetActive(false);

            treatButton.gameObject.SetActive(true);
            releaseButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);

            treatButton.interactable = false;
            releaseButton.interactable = false;
            closeButton.interactable = true;

            return;
        }

        // FISH IN TANK UI
        fishNameText.text = currentTank.FishName;

        if (fishUIImage != null)
        {
            fishUIImage.sprite = currentTank.FishSpriteImage;
            fishUIImage.preserveAspect = true;
            fishUIImage.gameObject.SetActive(currentTank.FishSpriteImage != null);
        }

        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = currentTank.MaxHealth;
        healthSlider.value = currentTank.Health;

        treatButton.gameObject.SetActive(true);
        releaseButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);

        // Treat is disabled when fish is fully healed
        treatButton.interactable = !currentTank.ReadyToRelease;

        // Release only works when fish is fully healed
        releaseButton.interactable = currentTank.ReadyToRelease;

        closeButton.interactable = true;
    }

    private void TreatFish()
    {
        if (currentTank == null) return;

        // Do not treat empty tank
        if (!currentTank.HasFish) return;

        // Do not treat fully healed fish
        if (currentTank.ReadyToRelease) return;

        OintmentMiniGame.Instance.StartMiniGame(currentTank);
    }

    private void ReleaseFish()
    {
        if (currentTank == null) return;

        currentTank.ReleaseFish();

        UpdateUI();
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
        currentTank = null;
    }
}