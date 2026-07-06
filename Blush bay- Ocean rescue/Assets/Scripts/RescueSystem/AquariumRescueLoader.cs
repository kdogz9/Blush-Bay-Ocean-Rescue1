using UnityEngine;
using UnityEngine.SceneManagement;

public class AquariumRescueLoader : MonoBehaviour
{
    [Header("Tank To Place Rescued Fish In")]
    [SerializeField] private Tank targetTank;

    [Header("Scene Names")]
    [SerializeField] private string rescueSceneName = "RescueScene";

    private void Start()
    {
        // Check if a fish was rescued in the rescue scene
        if (RescuedFishStorage.HasRescuedFish)
        {
            if (targetTank == null)
            {
                Debug.LogError("No target tank assigned in AquariumRescueLoader");
                return;
            }

            if (RescuedFishStorage.FishSprite == null)
            {
                Debug.LogError("Rescued fish sprite is missing. Check the Fish Sprite field on the rescue bubble.");
                return;
            }

            // Add the rescued fish into the tank
            targetTank.AddFish(
                RescuedFishStorage.FishName,
                RescuedFishStorage.FishSprite,
                RescuedFishStorage.StartingHealth,
                RescuedFishStorage.MaxHealth,
                RescuedFishStorage.IllnessName
            );

            // Clear storage so the fish is not added again every time
            RescuedFishStorage.Clear();
        }
    }

    public void GoToRescueScene()
    {
        SceneManager.LoadScene("RescueScene");
    }
}