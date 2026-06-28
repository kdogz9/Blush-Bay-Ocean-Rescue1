using UnityEngine;

public class Tank : MonoBehaviour
{
    [Header("Fish Info")]
    [SerializeField] private string fishName = "Bubbles";

    [SerializeField] private int health = 30;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private bool hasFish = true;

    [Header("Visuals")]
    [SerializeField] private GameObject fishSprite;

    // These let other scripts read the fish info
    public string FishName => fishName;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public bool HasFish => hasFish;

    // Fish can only be released when it exists and has full health
    public bool ReadyToRelease => hasFish && health >= maxHealth;

    private void Start()
    {
        // Make sure the fish sprite is shown or hidden correctly when the game starts
        UpdateFishSprite();
    }

    public void HealFish(int healAmount)
    {
        // If there is no fish in the tank, stop here
        if (!hasFish) return;

        // Add healing to the fish
        health += healAmount;

        // Stop the health going above the max health
        health = Mathf.Clamp(health, 0, maxHealth);
    }
    
    

    public Sprite FishSpriteImage
    {
        get
        {
            // If no fish sprite object has been assigned, return nothing
            if (fishSprite == null) return null;

            // Get the SpriteRenderer from the fish object
            SpriteRenderer spriteRenderer = fishSprite.GetComponent<SpriteRenderer>();

            // If there is no SpriteRenderer, return nothing
            if (spriteRenderer == null) return null;

            // Return the sprite being used by this fish
            return spriteRenderer.sprite;
        }
    }
    
    public void ReleaseFish()
    {
        // Only release the fish if it is fully healed
        if (!ReadyToRelease) return;

        // The tank is now empty
        hasFish = false;

        // Hide the fish sprite
        UpdateFishSprite();

        Debug.Log(fishName + " was released into the ocean!");
    }

    private void UpdateFishSprite()
    {
        // If no fish sprite has been added in the Inspector, stop here
        if (fishSprite == null) return;

        // Show the fish if the tank has a fish
        // Hide the fish if the tank is empty
        fishSprite.SetActive(hasFish);
    }
}