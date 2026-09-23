using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FoodMachine : MonoBehaviour
{
    [System.Serializable]
    public class FoodRecipe
    {
        public string recipeName = "Basic Fish Food";

        [Header("Recipe Requirements")]
        public int cleanKelpNeeded = 1;
        public int cleanSeaweedNeeded = 1;

        [Header("Output")]
        public IngredientType outputIngredient = IngredientType.FishFood;
        public int outputAmount = 1;
    }

    [Header("Machine Info")]
    [SerializeField] private string machineName = "Food Machine";

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer machineSpriteRenderer;
    [SerializeField] private Sprite emptyMachineSprite;
    [SerializeField] private Sprite fullMachineSprite;

    [Header("Recipe Options")]
    [SerializeField] private FoodRecipe[] recipes;

    [Header("Mixing")]
    [SerializeField] private float mixingTime = 5f;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject interactPrompt;

    [Header("Bobbing / Bubble Effect")]
    [SerializeField] private MachineCleaningEffect cleaningEffect;

    private bool playerNearby = false;
    private bool isMixing = false;
    private bool foodReadyToCollect = false;

    private int selectedCleanKelp = 0;
    private int selectedCleanSeaweed = 0;

    private FoodRecipe currentRecipe;
    private Coroutine mixingCoroutine;

    public string MachineName => machineName;
    public bool IsMixing => isMixing;
    public bool FoodReadyToCollect => foodReadyToCollect;

    public int SelectedCleanKelp => selectedCleanKelp;
    public int SelectedCleanSeaweed => selectedCleanSeaweed;

    public FoodRecipe CurrentRecipe => currentRecipe;

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

        SetEmptyVisual();
        UpdateCurrentRecipe();
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
        if (foodReadyToCollect)
        {
            CollectFood();
            return;
        }

        if (isMixing)
        {
            Debug.Log(machineName + " is still mixing.");
            return;
        }

        if (FoodMachineUI.Instance != null)
        {
            FoodMachineUI.Instance.OpenMachine(this);
        }
        else
        {
            Debug.LogWarning("No FoodMachineUI found in the scene.");
        }
    }

    public void AddKelpAmount()
    {
        if (isMixing || foodReadyToCollect) return;
        if (IngredientBackpack.Instance == null) return;

        if (selectedCleanKelp >= IngredientBackpack.Instance.SanitisedKelpAmount)
        {
            Debug.Log("No more clean kelp available.");
            return;
        }

        selectedCleanKelp++;
        UpdateCurrentRecipe();
    }

    public void RemoveKelpAmount()
    {
        if (isMixing || foodReadyToCollect) return;

        if (selectedCleanKelp > 0)
        {
            selectedCleanKelp--;
        }

        UpdateCurrentRecipe();
    }

    public void AddSeaweedAmount()
    {
        if (isMixing || foodReadyToCollect) return;
        if (IngredientBackpack.Instance == null) return;

        if (selectedCleanSeaweed >= IngredientBackpack.Instance.SanitisedSeaweedAmount)
        {
            Debug.Log("No more clean seaweed available.");
            return;
        }

        selectedCleanSeaweed++;
        UpdateCurrentRecipe();
    }

    public void RemoveSeaweedAmount()
    {
        if (isMixing || foodReadyToCollect) return;

        if (selectedCleanSeaweed > 0)
        {
            selectedCleanSeaweed--;
        }

        UpdateCurrentRecipe();
    }

    private void UpdateCurrentRecipe()
    {
        currentRecipe = null;

        if (recipes == null || recipes.Length == 0) return;

        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i].cleanKelpNeeded == selectedCleanKelp &&
                recipes[i].cleanSeaweedNeeded == selectedCleanSeaweed)
            {
                currentRecipe = recipes[i];
                return;
            }
        }
    }

    public string GetOutcomeText()
    {
        if (selectedCleanKelp == 0 && selectedCleanSeaweed == 0)
        {
            return "Add clean ingredients";
        }

        if (currentRecipe == null)
        {
            return "No known recipe";
        }

        return currentRecipe.recipeName;
    }

    public bool CanMakeFood()
    {
        return currentRecipe != null && !isMixing && !foodReadyToCollect;
    }

    public bool StartMakingFood()
{
    if (isMixing)
    {
        Debug.Log("Cannot make food because the machine is already mixing.");
        return false;
    }

    if (foodReadyToCollect)
    {
        Debug.Log("Cannot make food because food is already ready to collect.");
        return false;
    }

    if (currentRecipe == null)
    {
        Debug.LogWarning("Cannot make food because there is no matching recipe.");
        return false;
    }

    if (IngredientBackpack.Instance == null)
    {
        Debug.LogWarning("No IngredientBackpack found.");
        return false;
    }

    Debug.Log(
        "Trying to make recipe: " + currentRecipe.recipeName +
        " | Kelp selected: " + selectedCleanKelp +
        " | Seaweed selected: " + selectedCleanSeaweed
    );

    // Remove clean kelp only if the recipe uses kelp.
    if (selectedCleanKelp > 0)
    {
        bool removedKelp = IngredientBackpack.Instance.TryRemoveIngredient(
            IngredientType.SanitisedKelp,
            selectedCleanKelp
        );

        if (!removedKelp)
        {
            Debug.LogWarning("Not enough clean kelp.");
            return false;
        }
    }

    // Remove clean seaweed only if the recipe uses seaweed.
    if (selectedCleanSeaweed > 0)
    {
        bool removedSeaweed = IngredientBackpack.Instance.TryRemoveIngredient(
            IngredientType.SanitisedSeaweed,
            selectedCleanSeaweed
        );

        if (!removedSeaweed)
        {
            // Give the kelp back if kelp was already removed.
            if (selectedCleanKelp > 0)
            {
                IngredientBackpack.Instance.AddIngredient(
                    IngredientType.SanitisedKelp,
                    selectedCleanKelp
                );
            }

            Debug.LogWarning("Not enough clean seaweed.");
            return false;
        }
    }

    SetFullVisual();

    if (FoodMachineUI.Instance != null)
    {
        FoodMachineUI.Instance.ClosePanel();
        Debug.Log("Food machine panel closed.");
    }

    if (mixingCoroutine != null)
    {
        StopCoroutine(mixingCoroutine);
    }

    mixingCoroutine = StartCoroutine(MixingRoutine());

    Debug.Log("Food machine started making: " + currentRecipe.recipeName);

    return true;
}

    private IEnumerator MixingRoutine()
    {
        isMixing = true;
        foodReadyToCollect = false;

        if (cleaningEffect != null)
        {
            cleaningEffect.HideReadyIcon();
            cleaningEffect.StartCleaningEffect();
        }

        Debug.Log(machineName + " started making " + currentRecipe.recipeName);

        yield return new WaitForSeconds(mixingTime);

        isMixing = false;
        foodReadyToCollect = true;
        mixingCoroutine = null;

        if (cleaningEffect != null)
        {
            cleaningEffect.StopCleaningEffect();
            cleaningEffect.ShowReadyIcon();
        }

        Debug.Log(currentRecipe.recipeName + " is ready to collect.");
    }

    public bool CollectFood()
    {
        if (!foodReadyToCollect)
        {
            Debug.Log("No food ready to collect.");
            return false;
        }

        if (currentRecipe == null)
        {
            Debug.LogWarning("No current recipe saved.");
            return false;
        }

        if (IngredientBackpack.Instance != null)
        {
            IngredientBackpack.Instance.AddIngredient(
                currentRecipe.outputIngredient,
                currentRecipe.outputAmount
            );
        }

        Debug.Log("Collected " + currentRecipe.recipeName + " x" + currentRecipe.outputAmount);

        selectedCleanKelp = 0;
        selectedCleanSeaweed = 0;
        currentRecipe = null;
        foodReadyToCollect = false;

        if (cleaningEffect != null)
        {
            cleaningEffect.HideReadyIcon();
        }

        SetEmptyVisual();

        return true;
    }

    public void ClearSelectedIngredients()
    {
        if (isMixing || foodReadyToCollect) return;

        selectedCleanKelp = 0;
        selectedCleanSeaweed = 0;
        UpdateCurrentRecipe();
    }

    private void SetEmptyVisual()
    {
        if (machineSpriteRenderer != null && emptyMachineSprite != null)
        {
            machineSpriteRenderer.sprite = emptyMachineSprite;
        }
    }

    private void SetFullVisual()
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