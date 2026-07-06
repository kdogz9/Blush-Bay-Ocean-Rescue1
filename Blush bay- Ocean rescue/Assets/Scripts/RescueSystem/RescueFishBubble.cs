using UnityEngine;

public class RescueFishBubble : MonoBehaviour
{
    [Header("Fish Data")]
    [SerializeField] private string fishName = "BUBBLES";

    // This fish sprite will appear on the rescue panel and later in the tank
    [SerializeField] private Sprite fishSprite;

    [SerializeField] private int startingHealth = 25;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private string illnessName = "SCRATCHED FIN";

    private bool hasBeenClicked = false;

    private void OnMouseDown()
    {
        // Stops double clicking
        if (hasBeenClicked) return;

        hasBeenClicked = true;

        if (fishSprite == null)
        {
            Debug.LogError("Fish Sprite is missing on RescueFishBubble");
            return;
        }

        if (RescueSceneManager.Instance == null)
        {
            Debug.LogError("No RescueSceneManager found in scene");
            return;
        }

        // Send fish data to the rescue scene manager
        RescueSceneManager.Instance.RescueFish(
            fishName,
            fishSprite,
            startingHealth,
            maxHealth,
            illnessName
        );

        // Hide the clicked bubble
        gameObject.SetActive(false);
    }
}