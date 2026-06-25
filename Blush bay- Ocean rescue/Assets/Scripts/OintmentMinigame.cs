using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class OintmentMiniGame : MonoBehaviour
{
    // This lets other scripts start this mini game easily
    public static OintmentMiniGame Instance;

    [Header("UI Objects")]
    [SerializeField] private GameObject panel;

    [SerializeField] private RectTransform timingBar;
    [SerializeField] private RectTransform calmZone;
    [SerializeField] private RectTransform marker;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resultText;

    [Header("Mini Game Settings")]
    [SerializeField] private float markerSpeed = 220f;

    [SerializeField] private int perfectHealAmount = 35;
    [SerializeField] private int goodHealAmount = 20;
    [SerializeField] private int missHealAmount = 5;

    // This stores the tank/fish we are treating
    private Tank currentTank;

    // Is the mini game currently running?
    private bool gameRunning = false;

    // 1 means moving right, -1 means moving left
    private int moveDirection = 1;

    private void Awake()
    {
        // Save this mini game as the main instance
        Instance = this;

        // Hide the mini game when the game starts
        panel.SetActive(false);
    }

    private void Update()
    {
        // If the mini game is not running, do nothing
        if (!gameRunning) return;

        // Move the marker along the timing bar
        MoveMarker();

        // If the player presses SPACE, try to apply ointment
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ApplyOintment();
        }
    }

    public void StartMiniGame(Tank tank)
    {
        // Safety check: if no tank was given, stop here
        if (tank == null) return;

        // If the tank is empty, stop here
        if (!tank.HasFish) return;

        // Remember which tank we are treating
        currentTank = tank;

        // Show the mini game panel
        panel.SetActive(true);

        // Set the text
        titleText.text = "APPLY OINTMENT";
        resultText.text = "";

        // Put the marker at the far left of the bar
        float leftSide = -timingBar.rect.width / 2f;
        marker.anchoredPosition = new Vector2(leftSide, marker.anchoredPosition.y);

        // Start moving right
        moveDirection = 1;

        // Start the mini game
        gameRunning = true;
    }

    private void MoveMarker()
    {
        // Half of the timing bar width
        float halfBarWidth = timingBar.rect.width / 2f;

        // Half of the marker width
        float halfMarkerWidth = marker.rect.width / 2f;

        // This keeps the marker inside the bar edges
        float maxX = halfBarWidth - halfMarkerWidth;

        // Work out the marker's new X position
        float newX = marker.anchoredPosition.x + moveDirection * markerSpeed * Time.deltaTime;

        // If the marker reaches the right side, bounce back left
        if (newX > maxX)
        {
            newX = maxX;
            moveDirection = -1;
        }

        // If the marker reaches the left side, bounce back right
        if (newX < -maxX)
        {
            newX = -maxX;
            moveDirection = 1;
        }

        // Apply the new marker position
        marker.anchoredPosition = new Vector2(newX, marker.anchoredPosition.y);
    }

    private void ApplyOintment()
    {
        // Stop the marker when the player presses SPACE
        gameRunning = false;

        // Get the marker position
        float markerX = marker.anchoredPosition.x;

        // Get the calm zone position
        float calmZoneX = calmZone.anchoredPosition.x;

        // Work out how far the marker is from the centre of the calm zone
        float distanceFromCentre = Mathf.Abs(markerX - calmZoneX);

        // Half of the calm zone width
        float halfCalmZoneWidth = calmZone.rect.width / 2f;

        // Very centre of the calm zone = perfect
        float perfectZone = halfCalmZoneWidth * 0.35f;

        if (distanceFromCentre <= perfectZone)
        {
            // Best timing
            currentTank.HealFish(perfectHealAmount);
            resultText.text = "PERFECT APPLICATION!";
        }
        else if (distanceFromCentre <= halfCalmZoneWidth)
        {
            // Still inside the green zone
            currentTank.HealFish(goodHealAmount);
            resultText.text = "GOOD APPLICATION!";
        }
        else
        {
            // Outside the green zone
            currentTank.HealFish(missHealAmount);
            resultText.text = "THE FISH WRIGGLED!";
        }

        // Update the fish info panel health bar
        FishInfoUI.Instance.RefreshUI();

        // Close the mini game after a short delay
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        // Let the player see the result text
        yield return new WaitForSeconds(0.8f);

        // Hide the mini game panel
        panel.SetActive(false);

        // Forget the tank
        currentTank = null;
    }
}