using UnityEngine;
using UnityEngine.UI;

public class TravelMapLocationButton : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button button;

    [Header("Location Info")]
    [SerializeField] private string locationName = "Rescue Cove";
    [SerializeField] private string sceneName = "RescueScene";

    [Header("Locked / Unlocked")]
    [SerializeField] private bool isUnlocked = true;
    [SerializeField] private string lockedMessage = "This area is not unlocked yet!";

    private void Awake()
    {
        // If we forgot to drag the Button in, get it from this object.
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void Start()
    {
        // When this map section is clicked, run TravelToLocation.
        if (button != null)
        {
            button.onClick.AddListener(TravelToLocation);
        }
    }

    private void TravelToLocation()
    {
        // If the location is locked, do not travel.
        if (!isUnlocked)
        {
            if (TravelMapManager.Instance != null)
            {
                TravelMapManager.Instance.ShowMessage(lockedMessage);
            }

            return;
        }

        // Travel to the chosen scene.
        if (TravelMapManager.Instance != null)
        {
            Debug.Log("Clicked map location: " + locationName);
            TravelMapManager.Instance.TravelToScene(sceneName);
        }
    }
}