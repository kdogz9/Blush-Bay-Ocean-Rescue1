using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BackpackUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button backpackButton;
    [SerializeField] private Button closeButton;

    [Header("Panel")]
    [SerializeField] private GameObject backpackPanel;

    [Header("Notebook Text")]
    [SerializeField] private TMP_Text kelpHeaderText;
    [SerializeField] private TMP_Text kelpInfoText;
    [SerializeField] private TMP_Text seaweedHeaderText;
    [SerializeField] private TMP_Text seaweedInfoText;

    [Header("Notification")]
    [SerializeField] private GameObject notificationBubble;
    [SerializeField] private TMP_Text notificationText;

    [Header("Style")]
    [SerializeField] private Color headingColour = new Color32(200, 90, 205, 255);

    private bool backpackOpen = false;
    private IngredientBackpack backpack;

    private void Start()
    {
        if (backpackPanel != null)
        {
            backpackPanel.SetActive(false);
        }

        if (backpackButton != null)
        {
            backpackButton.onClick.AddListener(ToggleBackpack);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseBackpack);
        }

        SetupStaticText();
        StartCoroutine(ConnectToBackpack());
    }

    private IEnumerator ConnectToBackpack()
    {
        while (IngredientBackpack.Instance == null)
        {
            yield return null;
        }

        backpack = IngredientBackpack.Instance;

        backpack.OnBackpackChanged -= UpdateBackpackUI;
        backpack.OnBackpackChanged += UpdateBackpackUI;

        UpdateBackpackUI();
    }

    private void OnDestroy()
    {
        if (backpack != null)
        {
            backpack.OnBackpackChanged -= UpdateBackpackUI;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleBackpack();
        }
    }

    private void SetupStaticText()
    {
        if (kelpHeaderText != null)
        {
            kelpHeaderText.text = "KELP";
            kelpHeaderText.color = headingColour;
        }

        if (seaweedHeaderText != null)
        {
            seaweedHeaderText.text = "SEAWEED";
            seaweedHeaderText.color = headingColour;
        }
    }

    public void ToggleBackpack()
    {
        if (backpackOpen)
            CloseBackpack();
        else
            OpenBackpack();
    }

    public void OpenBackpack()
    {
        backpackOpen = true;

        if (backpackPanel != null)
        {
            backpackPanel.SetActive(true);
        }

        if (backpack != null)
        {
            backpack.ClearNewItemNotification();
        }

        UpdateBackpackUI();
    }

    public void CloseBackpack()
    {
        backpackOpen = false;

        if (backpackPanel != null)
        {
            backpackPanel.SetActive(false);
        }

        UpdateBackpackUI();
    }

    private void UpdateBackpackUI()
    {
        if (backpack == null)
        {
            backpack = IngredientBackpack.Instance;
        }

        if (backpack == null) return;

        if (kelpInfoText != null)
        {
            kelpInfoText.text =
                "Raw        x" + backpack.KelpAmount +
                "\n" +
                "Clean      x" + backpack.SanitisedKelpAmount;
        }

        if (seaweedInfoText != null)
        {
            seaweedInfoText.text =
                "Raw        x" + backpack.SeaweedAmount +
                "\n" +
                "Clean      x" + backpack.SanitisedSeaweedAmount;
        }

        UpdateNotificationBubble();
    }

    private void UpdateNotificationBubble()
    {
        if (notificationBubble == null) return;

        bool shouldShow =
            backpack != null &&
            backpack.HasNewItems &&
            !backpackOpen;

        notificationBubble.SetActive(shouldShow);

        if (shouldShow && notificationText != null)
        {
            if (!string.IsNullOrEmpty(backpack.LatestNotification))
            {
                notificationText.text = backpack.LatestNotification;
            }
            else
            {
                notificationText.text = "NEW!";
            }
        }
    }
}