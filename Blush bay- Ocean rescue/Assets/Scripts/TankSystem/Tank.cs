using UnityEngine;

public class Tank : MonoBehaviour
{
    [Header("Fish Info")]
    [SerializeField] private string fishName = "BUBBLES";
    [SerializeField] private string illnessName = "SCRATCHED FIN";

    [SerializeField] private int health = 0;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private bool hasFish = false;

    [Header("Visuals")]
    [SerializeField] private GameObject fishSprite;

    // Other scripts can read these values
    public string FishName => fishName;
    public string IllnessName => illnessName;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public bool HasFish => hasFish;

    // Fish is ready to release only if there is a fish and health is full
    public bool ReadyToRelease => hasFish && health >= maxHealth;

    // This lets UI and mini games get the current fish sprite
    public Sprite FishSpriteImage
    {
        get
        {
            if (fishSprite == null) return null;

            SpriteRenderer spriteRenderer = fishSprite.GetComponent<SpriteRenderer>();

            if (spriteRenderer == null) return null;

            return spriteRenderer.sprite;
        }
    }

    private void Start()
    {
        // Makes sure the fish sprite matches whether the tank has a fish
        UpdateFishSprite();
    }

    public void AddFish(string newFishName, Sprite newFishSprite, int startingHealth, int newMaxHealth, string newIllnessName)
    {
        // Set new fish data
        fishName = newFishName;
        illnessName = newIllnessName;
        maxHealth = newMaxHealth;

        // Clamp keeps health between 0 and maxHealth
        health = Mathf.Clamp(startingHealth, 0, maxHealth);

        // Tank now has a fish
        hasFish = true;

        // Change the visible fish sprite
        if (fishSprite != null)
        {
            fishSprite.SetActive(true);

            SpriteRenderer spriteRenderer = fishSprite.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = newFishSprite;
            }
        }

        UpdateFishSprite();

        Debug.Log("Fish added to tank: " + fishName);
    }

    public void HealFish(int healAmount)
    {
        // If tank is empty, do not heal anything
        if (!hasFish) return;

        // Add health
        health += healAmount;

        // Stop health going above max
        health = Mathf.Clamp(health, 0, maxHealth);

        Debug.Log(fishName + " healed to " + health + " / " + maxHealth);
    }

    public void ReleaseFish()
    {
        // Only release if there is a fish and it is fully healed
        if (!ReadyToRelease) return;

        Debug.Log(fishName + " was released into the ocean!");

        // Empty the tank
        hasFish = false;
        health = 0;
        fishName = "EMPTY TANK";
        illnessName = "";

        UpdateFishSprite();
    }

    private void UpdateFishSprite()
    {
        // If no fish sprite object is assigned, stop here
        if (fishSprite == null) return;

        // Show fish only if the tank has a fish
        fishSprite.SetActive(hasFish);
    }
}