using System.Collections;
using UnityEngine;

public class DonationBox : MonoBehaviour
{
    [Header("Shell Popup")]
    [SerializeField] private GameObject shellPopup;

    [Header("Popup Settings")]
    [SerializeField] private float popupTime = 0.8f;
    [SerializeField] private float floatUpDistance = 0.4f;

    // This stores the shell's original position so we can reset it every time
    private Vector3 shellStartPosition;

    // This stores the SpriteRenderer so we can fade the shell out
    private SpriteRenderer shellRenderer;

    // This stops multiple popup animations running at the same time
    private Coroutine popupRoutine;

    private void Start()
    {
        // If we have assigned a shell popup in the Inspector
        if (shellPopup != null)
        {
            // Save the starting position of the shell
            shellStartPosition = shellPopup.transform.localPosition;

            // Get the SpriteRenderer from the shell popup
            shellRenderer = shellPopup.GetComponent<SpriteRenderer>();

            // Hide the shell when the game starts
            shellPopup.SetActive(false);
        }
    }

    public void ReceiveDonation(int amount)
    {
        // Add the money to the donation total
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddMoney(amount);
        }
        else
        {
            Debug.LogWarning("No CurrencyManager found in the scene.");
        }

        // Show the shell popup effect
        ShowShellPopup();

        Debug.Log("Donation box received: +" + amount);
    }

    private void ShowShellPopup()
    {
        // If no shell popup has been assigned, stop here
        if (shellPopup == null) return;

        // If a popup animation is already running, stop it first
        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
        }

        // Start the shell popup animation
        popupRoutine = StartCoroutine(ShellPopupRoutine());
    }

    private IEnumerator ShellPopupRoutine()
    {
        // Reset the shell to its starting position
        shellPopup.transform.localPosition = shellStartPosition;

        // Turn the shell on
        shellPopup.SetActive(true);

        // Reset the shell colour so it is fully visible again
        if (shellRenderer != null)
        {
            Color startColour = shellRenderer.color;
            startColour.a = 1f;
            shellRenderer.color = startColour;
        }

        // This timer starts at 0 and counts up to popupTime
        float timer = 0f;

        while (timer < popupTime)
        {
            // Work out how far through the animation we are
            // 0 = start, 1 = finished
            float progress = timer / popupTime;

            // Move the shell upwards over time
            Vector3 newPosition = shellStartPosition + new Vector3(0f, floatUpDistance * progress, 0f);
            shellPopup.transform.localPosition = newPosition;

            // Fade the shell out near the end
            if (shellRenderer != null)
            {
                Color colour = shellRenderer.color;
                colour.a = 1f - progress;
                shellRenderer.color = colour;
            }

            // Increase the timer
            timer += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Hide the shell when the animation finishes
        shellPopup.SetActive(false);

        // Reset the shell position ready for next time
        shellPopup.transform.localPosition = shellStartPosition;

        // Reset the shell alpha ready for next time
        if (shellRenderer != null)
        {
            Color colour = shellRenderer.color;
            colour.a = 1f;
            shellRenderer.color = colour;
        }

        // Clear the routine
        popupRoutine = null;
    }
}