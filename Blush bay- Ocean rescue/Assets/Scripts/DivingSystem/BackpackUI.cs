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

    [Header("Ingredient Text")]
    [SerializeField] private TMP_Text kelpText;
    [SerializeField] private TMP_Text seaweedText;

    [Header("Notification")]
    [SerializeField] private GameObject notificationBubble;
    [SerializeField] private TMP_Text notificationText;

    private bool backpackOpen = false;
    private IngredientBackpack backpack;

    private void Start()
    {
        // Hide the backpack panel at the start.
        if (backpackPanel != null)
        {
            backpackPanel.SetActive(false);
        }

        // Click backpack icon to open/close.
        if (backpackButton != null)
        {
            backpackButton.onClick.AddListener(ToggleBackpack);
        }
        else
        {
            Debug.LogWarning(name + " is missing Backpack Button.");
        }

        // Click close button to close.
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseBackpack);
        }
        else
        {
            Debug.LogWarning(name + " is missing Close Button.");
        }

        // Wait for the IngredientBackpack to exist, then connect to it.
        StartCoroutine(ConnectToBackpack());
    }

    private IEnumerator ConnectToBackpack()
    {
        // Wait until the backpack exists.
        while (IngredientBackpack.Instance == null)
        {
            Debug.LogWarning("BackpackUI is waiting for IngredientBackpack...");
            yield return null;
        }

        backpack = IngredientBackpack.Instance;

        // Listen for changes, such as collecting, sanitising, or removing ingredients.
        backpack.OnBackpackChanged -= UpdateBackpackUI;
        backpack.OnBackpackChanged += UpdateBackpackUI;

        // Update the text straight away.
        UpdateBackpackUI();

        Debug.Log("BackpackUI connected to IngredientBackpack.");
    }

    private void OnDestroy()
    {
        // Stop listening when this UI is destroyed.
        if (backpack != null)
        {
            backpack.OnBackpackChanged -= UpdateBackpackUI;
        }
    }

    private void Update()
    {
        // Optional shortcut: press B to open/close backpack.
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleBackpack();
        }
    }

    public void ToggleBackpack()
    {
        if (backpackOpen)
        {
            CloseBackpack();
        }
        else
        {
            OpenBackpack();
        }
    }

    public void OpenBackpack()
    {
        backpackOpen = true;

        if (backpackPanel != null)
        {
            backpackPanel.SetActive(true);
        }

        // When the player opens the backpack, clear the "new item" notification.
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

        if (backpack == null)
        {
            Debug.LogWarning("No IngredientBackpack found for BackpackUI.");
            return;
        }

        // Kelp text now shows raw kelp and clean kelp.
        if (kelpText == null)
        {
            Debug.LogWarning(name + " is missing Kelp Text.");
        }
        else
        {
            kelpText.text =
                "Raw Kelp x" + backpack.KelpAmount.ToString() +
                "\nClean Kelp x" + backpack.SanitisedKelpAmount.ToString();
        }

        // Seaweed text now shows raw seaweed and clean seaweed.
        if (seaweedText == null)
        {
            Debug.LogWarning(name + " is missing Seaweed Text.");
        }
        else
        {
            seaweedText.text =
                "Raw Seaweed x" + backpack.SeaweedAmount.ToString() +
                "\nClean Seaweed x" + backpack.SanitisedSeaweedAmount.ToString();
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