using UnityEngine;
using UnityEngine.InputSystem;

public class MapGlobeInteractable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";

    [Header("Optional Prompt")]
    [SerializeField] private GameObject interactPrompt;

    // This becomes true when the player is standing near the globe.
    private bool playerNearby = false;

    private void Start()
    {
        // Hide the "Press E" prompt at the start.
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Only allow interaction if the player is nearby.
        if (!playerNearby) return;

        // Press E to open the map.
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenTravelMap();
        }
    }

    private void OpenTravelMap()
    {
        if (TravelMapManager.Instance != null)
        {
            TravelMapManager.Instance.OpenMap();
        }
        else
        {
            Debug.LogWarning("No TravelMapManager found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the thing entering the trigger is the player.
        if (other.CompareTag(playerTag))
        {
            playerNearby = true;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }

            Debug.Log("Player near travel globe.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the thing leaving the trigger is the player.
        if (other.CompareTag(playerTag))
        {
            playerNearby = false;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            Debug.Log("Player left travel globe.");
        }
    }
}