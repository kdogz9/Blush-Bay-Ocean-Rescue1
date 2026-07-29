using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RescueSceneManager : MonoBehaviour
{
    public static RescueSceneManager Instance;

    [Header("Scene Names")]
    [SerializeField] private string aquariumSceneName = "SampleScene";

    [Header("Rescue Panel UI")]
    [SerializeField] private GameObject rescuedFishPanel;
    [SerializeField] private Image fishImage;
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private TMP_Text statsText;

    private void Awake()
    {
        Instance = this;

        // Hide panel when rescue scene starts
        if (rescuedFishPanel != null)
        {
            rescuedFishPanel.SetActive(false);
        }
    }

    public void RescueFish(string fishName, Sprite fishSprite, int startingHealth, int maxHealth, string illnessName)
    {
        if (fishSprite == null)
        {
            Debug.LogError("Cannot rescue fish because fishSprite is null");
            return;
        }

        // Save fish so aquarium can load it into tank
        RescuedFishStorage.SaveFish(
            fishName,
            fishSprite,
            startingHealth,
            maxHealth,
            illnessName
        );

        // Show rescue panel
        rescuedFishPanel.SetActive(true);

        // Show fish image on the panel
        fishImage.sprite = fishSprite;
        fishImage.preserveAspect = true;
        fishImage.gameObject.SetActive(true);

        // Show text
        fishNameText.text = fishName;

        statsText.text =
            // colour sets the heading as a different colour to the rest of the text 
            "<color=#7000DE>ILLNESS:</color> " + illnessName + "\n" +
            "<color=#7000DE>HEALTH:</color> " + startingHealth + "/" + maxHealth + "\n" +
            "<color=#7000DE>STATUS:</color> RESCUED";
    }

    public void ReturnToAquarium()
    {
        SceneManager.LoadScene("AquariumScene");
    }
}