using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReproducingMachine : MonoBehaviour
{
    [Header("Machine Info")]
    [SerializeField] private string machineName = "Reproducing Machine";

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer machineSpriteRenderer;
    [SerializeField] private Sprite emptyMachineSprite;
    [SerializeField] private Sprite fullMachineSprite;

    [Header("Reproducing Settings")]
    [SerializeField] private float reproduceTime = 60f;
    [SerializeField] private int amountProduced = 5;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject interactPrompt;

    [Header("Cleaning/Bobbing Effect")]
    [SerializeField] private MachineCleaningEffect cleaningEffect;

    private bool playerNearby = false;
    private bool hasIngredientLoaded = false;
    private bool isReproducing = false;
    private bool itemReadyToCollect = false;

    private IngredientType ingredientBeingReproduced;
    private Coroutine reproduceCoroutine;

    public string MachineName => machineName;
    public bool HasIngredientLoaded => hasIngredientLoaded;
    public bool IsReproducing => isReproducing;
    public bool ItemReadyToCollect => itemReadyToCollect;
    public IngredientType IngredientBeingReproduced => ingredientBeingReproduced;

    private void Awake()
    {
        if (cleaningEffect == null)
        {
            cleaningEffect = GetComponent<MachineCleaningEffect>();
        }

        if (machineSpriteRenderer == null)
        {
            machineSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (cleaningEffect != null)
        {
            cleaningEffect.StopCleaningEffect();
            cleaningEffect.HideReadyIcon();
        }

        SetMachineEmptyVisual();
    }

    private void Update()
    {
        if (!playerNearby) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            InteractWithMachine();
        }
    }

    private void InteractWithMachine()
    {
        // If produce is ready, pressing E collects it.
        if (itemReadyToCollect)
        {
            CollectProducedItems();
            return;
        }

        // If it is already reproducing, do not open the panel.
        if (isReproducing)
        {
            Debug.Log(machineName + " is still reproducing.");
            return;
        }

        // If no ingredient has been loaded yet, open the panel.
        if (!hasIngredientLoaded)
        {
            if (ReproducingMachineUI.Instance != null)
            {
                ReproducingMachineUI.Instance.OpenMachine(this);
            }
            else
            {
                Debug.LogWarning("No ReproducingMachineUI found in the scene.");
            }

            return;
        }

        Debug.Log(machineName + " is loaded but not currently reproducing.");
    }

    public bool TryLoadCleanIngredient(IngredientType cleanIngredient)
    {
        if (hasIngredientLoaded || isReproducing || itemReadyToCollect)
        {
            Debug.Log("Machine already has an ingredient loaded.");
            return false;
        }

        if (IngredientBackpack.Instance == null)
        {
            Debug.LogWarning("No IngredientBackpack found.");
            return false;
        }

        // This machine only accepts CLEAN kelp or CLEAN seaweed.
        if (cleanIngredient != IngredientType.SanitisedKelp &&
            cleanIngredient != IngredientType.SanitisedSeaweed)
        {
            Debug.LogWarning("Reproducing machine only accepts clean kelp or clean seaweed.");
            return false;
        }

        // Remove only 1 clean ingredient from the backpack.
        bool removed = IngredientBackpack.Instance.TryRemoveIngredient(cleanIngredient, 1);

        if (!removed)
        {
            Debug.Log("Could not remove " + cleanIngredient + " from backpack.");
            return false;
        }

        ingredientBeingReproduced = cleanIngredient;
        hasIngredientLoaded = true;

        SetMachineFullVisual();

        Debug.Log("Loaded " + cleanIngredient + " into " + machineName);

        StartReproducing();

        return true;
    }

    private void StartReproducing()
    {
        if (!hasIngredientLoaded) return;
        if (isReproducing) return;

        if (reproduceCoroutine != null)
        {
            StopCoroutine(reproduceCoroutine);
        }

        reproduceCoroutine = StartCoroutine(ReproduceRoutine());
    }

    private IEnumerator ReproduceRoutine()
    {
        isReproducing = true;
        itemReadyToCollect = false;

        if (cleaningEffect != null)
        {
            cleaningEffect.HideReadyIcon();
            cleaningEffect.StartCleaningEffect();
        }

        Debug.Log(machineName + " started reproducing " + ingredientBeingReproduced);

        yield return new WaitForSeconds(reproduceTime);

        isReproducing = false;
        itemReadyToCollect = true;

        if (cleaningEffect != null)
        {
            cleaningEffect.StopCleaningEffect();
            cleaningEffect.ShowReadyIcon();
        }

        Debug.Log(machineName + " has produced x" + amountProduced + " " + ingredientBeingReproduced);
    }

    private void CollectProducedItems()
    {
        if (!itemReadyToCollect) return;

        if (IngredientBackpack.Instance != null)
        {
            IngredientBackpack.Instance.AddIngredient(ingredientBeingReproduced, amountProduced);
        }

        itemReadyToCollect = false;

        if (cleaningEffect != null)
        {
            cleaningEffect.HideReadyIcon();
        }

        Debug.Log("Collected x" + amountProduced + " " + ingredientBeingReproduced);

        // Because the original ingredient is still loaded, the machine starts again.
        StartReproducing();
    }

    private void SetMachineEmptyVisual()
    {
        if (machineSpriteRenderer != null && emptyMachineSprite != null)
        {
            machineSpriteRenderer.sprite = emptyMachineSprite;
        }
    }

    private void SetMachineFullVisual()
    {
        if (machineSpriteRenderer != null && fullMachineSprite != null)
        {
            machineSpriteRenderer.sprite = fullMachineSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerNearby = true;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }

            Debug.Log("Player near " + machineName);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerNearby = false;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            Debug.Log("Player left " + machineName);
        }
    }
}