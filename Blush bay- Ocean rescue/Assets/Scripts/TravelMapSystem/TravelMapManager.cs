using UnityEngine;
using UnityEngine.SceneManagement;

public class TravelMapManager : MonoBehaviour
{
    // This lets other scripts easily find the map manager.
    public static TravelMapManager Instance;

    [Header("Map UI")]
    [SerializeField] private GameObject travelMapUI;

    [Header("Optional Message")]
    [SerializeField] private TMPro.TMP_Text messageText;

    private void Awake()
    {
        // Save this as the main TravelMapManager.
        Instance = this;

        // Hide the map when the scene starts.
        if (travelMapUI != null)
        {
            travelMapUI.SetActive(false);
        }

        // Clear any message text at the start.
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    public void OpenMap()
    {
        // Show the map UI.
        if (travelMapUI != null)
        {
            travelMapUI.SetActive(true);
        }

        // Clear old messages.
        if (messageText != null)
        {
            messageText.text = "";
        }

        Debug.Log("Travel map opened.");
    }

    public void CloseMap()
    {
        // Hide the map UI.
        if (travelMapUI != null)
        {
            travelMapUI.SetActive(false);
        }

        Debug.Log("Travel map closed.");
    }

    public void TravelToScene(string sceneName)
    {
        // Safety check.
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("No scene name was given.");
            return;
        }

        Debug.Log("Travelling to scene: " + sceneName);

        // Load the chosen scene.
        SceneManager.LoadScene(sceneName);
    }

    public void ShowMessage(string message)
    {
        // Show a message on the map, for example:
        // "You need a diving suit first!"
        if (messageText != null)
        {
            messageText.text = message;
        }

        Debug.Log(message);
    }
}