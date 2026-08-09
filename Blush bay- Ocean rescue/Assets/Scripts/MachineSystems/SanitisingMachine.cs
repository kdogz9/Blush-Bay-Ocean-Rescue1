using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SanitisingMachine : MonoBehaviour
{
    [Header("Machine Info")]
    [SerializeField] private string machineName = "Sanitising Machine";

    [Header("Processing")]
    [SerializeField] private float processingTime = 4f;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject interactPrompt;

    [Header("Cleaning Effect")]
    [SerializeField] private MachineCleaningEffect cleaningEffect;

    private bool playerNearby = false;
    private bool isProcessing = false;
    private bool itemReadyToCollect = false;

    private IngredientType cleanIngredientReady;

    public string MachineName => machineName;
    public bool IsProcessing => isProcessing;
    public bool ItemReadyToCollect => itemReadyToCollect;

    private void Awake()
    {
        if (cleaningEffect == null)
        {
            cleaningEffect = GetComponent<MachineCleaningEffect>();
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
        if (itemReadyToCollect)
        {
            CollectCleanIngredient();
            return;
        }

        if (isProcessing)
        {
            Debug.Log("Sanitising machine is still cleaning.");
            return;
        }

        if (SanitisingMachineUI.Instance != null)
        {
            SanitisingMachineUI.Instance.OpenMachine(this);
        }
        else
        {
            Debug.LogWarning("No SanitisingMachineUI found in the scene.");
        }
    }

    public bool TryAddIngredientToMachine(IngredientType rawIngredient)
    {
        if (isProcessing || itemReadyToCollect)
        {
            Debug.Log("Machine is busy.");
            return false;
        }

        if (IngredientBackpack.Instance == null)
        {
            Debug.LogWarning("No IngredientBackpack found.");
            return false;
        }

        if (rawIngredient != IngredientType.Kelp && rawIngredient != IngredientType.Seaweed)
        {
            Debug.LogWarning("Sanitising machine only accepts raw kelp or raw seaweed.");
            return false;
        }

        bool removedIngredient = IngredientBackpack.Instance.TryRemoveIngredient(rawIngredient, 1);

        if (!removedIngredient)
        {
            Debug.Log("Could not remove " + rawIngredient + " from backpack.");
            return false;
        }

        if (rawIngredient == IngredientType.Kelp)
        {
            cleanIngredientReady = IngredientType.SanitisedKelp;
        }
        else if (rawIngredient == IngredientType.Seaweed)
        {
            cleanIngredientReady = IngredientType.SanitisedSeaweed;
        }

        StartCoroutine(SanitiseRoutine());

        return true;
    }

    private IEnumerator SanitiseRoutine()
    {
        isProcessing = true;
        itemReadyToCollect = false;

        if (cleaningEffect != null)
        {
            cleaningEffect.HideReadyIcon();
            cleaningEffect.StartCleaningEffect();
        }
        else
        {
            Debug.LogWarning("No MachineCleaningEffect assigned.");
        }

        Debug.Log("Machine started cleaning.");

        yield return new WaitForSeconds(processingTime);

        isProcessing = false;
        itemReadyToCollect = true;

        if (cleaningEffect != null)
        {
            cleaningEffect.StopCleaningEffect();
            cleaningEffect.ShowReadyIcon();
        }

        Debug.Log("Machine finished cleaning. Ready to collect.");
    }

    private void CollectCleanIngredient()
    {
        if (!itemReadyToCollect) return;

        if (IngredientBackpack.Instance != null)
        {
            IngredientBackpack.Instance.AddIngredient(cleanIngredientReady, 1);
        }

        itemReadyToCollect = false;

        if (cleaningEffect != null)
        {
            cleaningEffect.HideReadyIcon();
        }

        Debug.Log("Collected clean ingredient: " + cleanIngredientReady);
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

            Debug.Log("Player near sanitising machine.");
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

            Debug.Log("Player left sanitising machine.");
        }
    }
}