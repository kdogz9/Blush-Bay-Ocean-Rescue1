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

    [Header("Fish Sprites")]
    [SerializeField] private Sprite normalFishSprite;
    [SerializeField] private Sprite injuredFishSprite;

    [Header("Injured Fish Animation")]
    [SerializeField] private Sprite[] injuredIdleFrames;

    private SpriteRenderer fishSpriteRenderer;

    public Sprite[] InjuredIdleFrames => injuredIdleFrames;

    // Other scripts can read these values
    public string FishName => fishName;
    public string IllnessName => illnessName;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public bool HasFish => hasFish;

    // Fish is ready to release only if there is a fish and health is full
    public bool ReadyToRelease => hasFish && health >= maxHealth;

    // This lets the FishInfoUI and mini game get the correct fish sprite
    public Sprite FishSpriteImage
    {
        get
        {
            if (!hasFish)
            {
                return null;
            }

            // If fish is still injured, show injured sprite
            if (!ReadyToRelease && injuredFishSprite != null)
            {
                return injuredFishSprite;
            }

            // If fish is healed, show normal sprite
            if (normalFishSprite != null)
            {
                return normalFishSprite;
            }

            // Fallback if needed
            if (fishSpriteRenderer != null)
            {
                return fishSpriteRenderer.sprite;
            }

            return null;
        }
    }

    private void Awake()
    {
        // Get the SpriteRenderer from the fish sprite object
        if (fishSprite != null)
        {
            fishSpriteRenderer = fishSprite.GetComponent<SpriteRenderer>();

            // If the SpriteRenderer is on a child object instead, this still finds it
            if (fishSpriteRenderer == null)
            {
                fishSpriteRenderer = fishSprite.GetComponentInChildren<SpriteRenderer>();
            }
        }
    }

    private void Start()
    {
        // Makes sure the fish sprite matches whether the tank has a fish
        UpdateFishSprite();
    }

    public void AddFish(
        string newFishName,
        Sprite newFishSprite,
        int startingHealth,
        int newMaxHealth,
        string newIllnessName,
        Sprite newInjuredFishSprite = null,
        Sprite[] newInjuredIdleFrames = null
    )
    {
        fishName = newFishName;
        illnessName = newIllnessName;
        health = startingHealth;
        maxHealth = newMaxHealth;
        hasFish = true;

        // Store the normal healthy sprite
        normalFishSprite = newFishSprite;

        // Only replace injured sprite if one is provided
        // This means you can still assign the injured sprite manually on the Tank in the Inspector
        if (newInjuredFishSprite != null)
        {
            injuredFishSprite = newInjuredFishSprite;
        }

        // Only replace animation frames if some are provided
        if (newInjuredIdleFrames != null && newInjuredIdleFrames.Length > 0)
        {
            injuredIdleFrames = newInjuredIdleFrames;
        }

        UpdateFishSprite();

        Debug.Log("Fish added to tank: " + fishName);
    }

    public void HealFish(int healAmount)
    {
        if (!hasFish) return;

        health += healAmount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        // This changes from injured sprite to normal sprite once fully healed
        UpdateFishSprite();
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

        if (!hasFish)
        {
            if (fishSpriteRenderer != null)
            {
                fishSpriteRenderer.sprite = null;
            }

            return;
        }

        // Make sure we have the SpriteRenderer
        if (fishSpriteRenderer == null)
        {
            fishSpriteRenderer = fishSprite.GetComponent<SpriteRenderer>();

            if (fishSpriteRenderer == null)
            {
                fishSpriteRenderer = fishSprite.GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (fishSpriteRenderer == null) return;

        // If the fish is injured, show injured sprite inside the tank
        if (!ReadyToRelease && injuredFishSprite != null)
        {
            fishSpriteRenderer.sprite = injuredFishSprite;
        }
        else
        {
            // If the fish is healed, show normal sprite
            fishSpriteRenderer.sprite = normalFishSprite;
        }
    }
}