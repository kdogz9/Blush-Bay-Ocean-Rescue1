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
            OpenMachinePanel();
        }
    }

    private void OpenMachinePanel()
    {
        if (ReproducingMachineUI.Instance != null)
        {
            ReproducingMachineUI.Instance.OpenMachine(this);
        }
        else
        {
            Debug.LogWarning("No ReproducingMachineUI found in the scene.");
        }
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

        if (cleanIngredient != IngredientType.SanitisedKelp &&
            cleanIngredient != IngredientType.SanitisedSeaweed)
        {
            Debug.LogWarning("Reproducing machine only accepts clean kelp or clean seaweed.");
            return false;
        }

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

    public void StartReproducing()
    {
        if (!hasIngredientLoaded)
        {
            Debug.Log("Cannot start. No ingredient loaded.");
            return;
        }

        if (isReproducing)
        {
            Debug.Log("Machine is already reproducing.");
            return;
        }

        if (itemReadyToCollect)
        {
            Debug.Log("Collect the ready items first.");
            return;
        }

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
        reproduceCoroutine = null;

        if (cleaningEffect != null)
        {
            cleaningEffect.StopCleaningEffect();
            cleaningEffect.ShowReadyIcon();
        }

        Debug.Log(machineName + " has produced x" + amountProduced + " " + ingredientBeingReproduced);
    }

    public bool CollectProducedItems()
    {
        if (!itemReadyToCollect)
        {
            Debug.Log("Nothing ready to collect.");
            return false;
        }

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

        // Automatically starts again because the ingredient is still loaded.
        StartReproducing();

        return true;
    }

    public void StopReproducing()
    {
        if (!isReproducing)
        {
            Debug.Log("Machine is not currently reproducing.");
            return;
        }

        if (reproduceCoroutine != null)
        {
            StopCoroutine(reproduceCoroutine);
            reproduceCoroutine = null;
        }

        isReproducing = false;
        itemReadyToCollect = false;

        if (cleaningEffect != null)
        {
            cleaningEffect.StopCleaningEffect();
            cleaningEffect.HideReadyIcon();
        }

        Debug.Log(machineName + " stopped. Ingredient is still inside.");
    }

    public bool RemoveLoadedIngredient()
    {
        if (!hasIngredientLoaded)
        {
            Debug.Log("No ingredient to remove.");
            return false;
        }

        if (isReproducing)
        {
            Debug.Log("Stop the machine before removing the ingredient.");
            return false;
        }

        if (itemReadyToCollect)
        {
            Debug.Log("Collect ready items before removing the ingredient.");
            return false;
        }

        if (IngredientBackpack.Instance != null)
        {
            IngredientBackpack.Instance.AddIngredient(ingredientBeingReproduced, 1);
        }

        ClearMachine();

        Debug.Log("Removed loaded ingredient and returned x1 " + ingredientBeingReproduced);

        return true;
    }

    public void ResetMachine()
    {
        if (reproduceCoroutine != null)
        {
            StopCoroutine(reproduceCoroutine);
            reproduceCoroutine = null;
        }

        ClearMachine();

        Debug.Log(machineName + " has been reset.");
    }

    private void ClearMachine()
    {
        hasIngredientLoaded = false;
        isReproducing = false;
        itemReadyToCollect = false;

        if (cleaningEffect != null)
        {
            cleaningEffect.StopCleaningEffect();
            cleaningEffect.HideReadyIcon();
        }

        SetMachineEmptyVisual();
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