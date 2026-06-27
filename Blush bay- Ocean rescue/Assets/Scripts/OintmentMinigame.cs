using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class OintmentMiniGame : MonoBehaviour
{
    // Lets other scripts start this mini game
    public static OintmentMiniGame Instance;

    [Header("UI Objects")]
    [SerializeField] private GameObject panel;

    [SerializeField] private RectTransform timingBar;
    [SerializeField] private RectTransform calmZone;
    [SerializeField] private RectTransform marker;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resultText;

    [Header("Fish Visual")]
    [SerializeField] private Image miniGameFishImage;
    [SerializeField] private GameObject ointmentEffect;

    [Header("Mini Game Settings")]
    [SerializeField] private float markerSpeed = 220f;

    [SerializeField] private int perfectHealAmount = 35;
    [SerializeField] private int goodHealAmount = 20;
    [SerializeField] private int missHealAmount = 5;

    // This stores the tank/fish we are currently treating
    private Tank currentTank;

    // Is the mini game currently running?
    private bool gameRunning = false;

    // 1 means moving right, -1 means moving left
    private int moveDirection = 1;

    private void Awake()
    {
        // Save this as the main mini game
        Instance = this;

        // Hide the mini game at the start
        panel.SetActive(false);

        // Hide the ointment mark at the start
        if (ointmentEffect != null)
        {
            ointmentEffect.SetActive(false);
        }
    }

    private void Update()
    {
        // Do nothing if the mini game is not running
        if (!gameRunning) return;

        // Move marker left and right
        MoveMarker();

        // Press SPACE to apply ointment
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ApplyOintment();
        }
    }

    public void StartMiniGame(Tank tank)
    {
        // If no tank was given, stop here
        if (tank == null) return;

        // If tank has no fish, stop here
        if (!tank.HasFish) return;

        // If fish is already healed, stop here
        if (tank.ReadyToRelease) return;

        // Remember which tank we are treating
        currentTank = tank;

        // Show the panel
        panel.SetActive(true);

        // Set title and clear old result
        titleText.text = "APPLY OINTMENT";
        resultText.text = "";

        // Show the fish sprite from the tank on the mini game UI
        if (miniGameFishImage != null)
        {
            miniGameFishImage.sprite = currentTank.FishSpriteImage;
            miniGameFishImage.gameObject.SetActive(currentTank.FishSpriteImage != null);
        }

        // Hide ointment mark until player presses SPACE
        if (ointmentEffect != null)
        {
            ointmentEffect.SetActive(false);
        }

        // Put marker at left side of the bar
        float leftSide = -timingBar.rect.width / 2f;
        marker.anchoredPosition = new Vector2(leftSide, marker.anchoredPosition.y);

        // Start moving right
        moveDirection = 1;

        // Start the mini game
        gameRunning = true;
    }

    private void MoveMarker()
    {
        // Half the width of the timing bar
        float halfBarWidth = timingBar.rect.width / 2f;

        // Half the width of the marker
        float halfMarkerWidth = marker.rect.width / 2f;

        // This stops marker going past the bar edge
        float maxX = halfBarWidth - halfMarkerWidth;

        // Move marker
        float newX = marker.anchoredPosition.x + moveDirection * markerSpeed * Time.deltaTime;

        // Bounce off right edge
        if (newX > maxX)
        {
            newX = maxX;
            moveDirection = -1;
        }

        // Bounce off left edge
        if (newX < -maxX)
        {
            newX = -maxX;
            moveDirection = 1;
        }

        // Apply new marker position
        marker.anchoredPosition = new Vector2(newX, marker.anchoredPosition.y);
    }

    private void ApplyOintment()
    {
        // Stop the marker
        gameRunning = false;

        // Show ointment effect on the fish
        ShowOintmentEffect();

        // Marker position
        float markerX = marker.anchoredPosition.x;

        // Calm zone position
        float calmZoneX = calmZone.anchoredPosition.x;

        // Distance from marker to centre of calm zone
        float distanceFromCentre = Mathf.Abs(markerX - calmZoneX);

        // Half of green zone width
        float halfCalmZoneWidth = calmZone.rect.width / 2f;

        // Very centre of the green zone counts as perfect
        float perfectZone = halfCalmZoneWidth * 0.35f;

        if (distanceFromCentre <= perfectZone)
        {
            // Best result
            currentTank.HealFish(perfectHealAmount);
            resultText.text = "PERFECT!";
        }
        else if (distanceFromCentre <= halfCalmZoneWidth)
        {
            // Still inside green zone
            currentTank.HealFish(goodHealAmount);
            resultText.text = "GOOD!";
        }
        else
        {
            // Outside green zone
            currentTank.HealFish(missHealAmount);
            resultText.text = "WRIGGLED!";
        }

        // Update the fish panel health bar
        FishInfoUI.Instance.RefreshUI();

        // Close after short delay
        StartCoroutine(CloseAfterDelay());
    }

    private void ShowOintmentEffect()
    {
        // If no ointment effect was assigned, stop here
        if (ointmentEffect == null) return;

        // Show the ointment blob
        ointmentEffect.SetActive(true);

        // Reset size before the absorb effect starts
        ointmentEffect.transform.localScale = Vector3.one;

        // Start the absorb effect
        StartCoroutine(OintmentAbsorbEffect());
    }

    private IEnumerator OintmentAbsorbEffect()
    {
        // Try to get the Image component from the ointment blob
        Image ointmentImage = ointmentEffect.GetComponent<Image>();

        // If there is no Image component, stop here
        if (ointmentImage == null) yield break;

        // Remember the original colour
        Color startColor = ointmentImage.color;

        // How long the ointment takes to absorb
        float absorbTime = 1.5f;

        // Timer starts at 0
        float timer = 0f;

        while (timer < absorbTime)
        {
            // Increase timer every frame
            timer += Time.deltaTime;

            // This gives us a value from 0 to 1
            float progress = timer / absorbTime;

            // Slowly shrink the ointment blob
            float scale = Mathf.Lerp(1f, 0.2f, progress);
            ointmentEffect.transform.localScale = Vector3.one * scale;

            // Slowly fade the ointment blob out
            float alpha = Mathf.Lerp(1f, 0f, progress);
            ointmentImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // Hide the ointment blob after it has absorbed
        ointmentEffect.SetActive(false);

        // Reset it ready for next time
        ointmentEffect.transform.localScale = Vector3.one;
        ointmentImage.color = startColor;
    }

    private IEnumerator CloseAfterDelay()
    {
        // Let player see the result
        yield return new WaitForSeconds(0.8f);

        // Hide panel
        panel.SetActive(false);

        // Hide ointment effect ready for next time
        if (ointmentEffect != null)
        {
            ointmentEffect.SetActive(false);
        }

        // Forget the current tank
        currentTank = null;
    }
}