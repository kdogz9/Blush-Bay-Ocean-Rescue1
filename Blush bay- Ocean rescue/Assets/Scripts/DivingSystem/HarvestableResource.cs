using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HarvestableResource : MonoBehaviour
{
    [Header("Ingredient")]
    [SerializeField] private IngredientType ingredientType = IngredientType.Kelp;

    [Header("Amount Given")]
    [SerializeField] private int amountToGive = 1;

    [Header("Sprites")]
    [SerializeField] private Sprite fullSprite;
    [SerializeField] private Sprite cutSprite;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColour = Color.white;
    [SerializeField] private Color readyColour = new Color(1.2f, 1.2f, 1.2f, 1f);
    
    [Header("Regrowth")]
    [SerializeField] private bool canRegrow = true;
    [SerializeField] private float regrowTime = 10f;
// time for the sprite to regrow 

    [Header("Prompt")]
    [SerializeField] private GameObject interactPrompt;

    private SpriteRenderer spriteRenderer;

    private bool playerNearby = false;
    private bool hasBeenHarvested = false;

    private void Awake()
    {
        // Get the sprite renderer from this plant.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Start with the full plant sprite.
        if (spriteRenderer != null && fullSprite != null)
        {
            spriteRenderer.sprite = fullSprite;
        }

        // Hide the E prompt at the start.
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Do nothing if the player is not near the plant.
        if (!playerNearby) return;

        // Do nothing if the plant has already been cut.
        if (hasBeenHarvested) return;

        // Press E to harvest the plant.
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Harvest();
        }
    }

    private void Harvest()
    {
        hasBeenHarvested = true;

        // Add kelp or seaweed to the backpack.
        if (IngredientBackpack.Instance != null)
        {
            IngredientBackpack.Instance.AddIngredient(ingredientType, amountToGive);
        }
        else
        {
            Debug.LogWarning("No IngredientBackpack found in the scene.");
        }

        // Change the plant to its cut sprite.
        if (spriteRenderer != null && cutSprite != null)
        {
            spriteRenderer.sprite = cutSprite;
        }

        // Hide the prompt after cutting.
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColour;
        }

        Debug.Log("Harvested " + ingredientType + " x" + amountToGive);

        // Start regrowth.
        if (canRegrow)
        {
            StartCoroutine(RegrowAfterDelay());
        }
    }

    private IEnumerator RegrowAfterDelay()
    {
        // Wait before the plant grows back.
        yield return new WaitForSeconds(regrowTime);

        hasBeenHarvested = false;

        // Change back to the full sprite.
        if (spriteRenderer != null && fullSprite != null)
        {
            spriteRenderer.sprite = fullSprite;
        }

        Debug.Log(ingredientType + " has regrown.");

        // If the player is still standing nearby, show the prompt again.
        if (playerNearby && interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (!hasBeenHarvested && interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }

            // Slightly brighten the plant when it can be collected.
            if (spriteRenderer != null && !hasBeenHarvested)
            {
                spriteRenderer.color = readyColour;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            // Return plant to normal colour when player leaves.
            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColour;
            }
        }
    }
}