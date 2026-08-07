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

    [Header("Machine Feedback")]
    [SerializeField] private GameObject whirringEffect;
    [SerializeField] private GameObject readyIcon;

    private bool playerNearby = false;
    private bool isProcessing = false;
    private bool itemReadyToCollect = false;

    private IngredientType cleanIngredientReady;

    public string MachineName => machineName;
    public bool IsProcessing => isProcessing;
    public bool ItemReadyToCollect => itemReadyToCollect;

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (whirringEffect != null)
        {
            whirringEffect.SetActive(false);
        }

        if (readyIcon != null)
        {
            readyIcon.SetActive(false);
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
        // If the machine has finished, pressing E collects the clean item.
        if (itemReadyToCollect)
        {
            CollectCleanIngredient();
            return;
        }

        // If the machine is working, do not open the panel.
        if (isProcessing)
        {
            Debug.Log("Machine is still sanitising.");
            return;
        }

        // Otherwise open the small panel with only the two ingredient buttons.
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
        else
        {
            Debug.LogWarning("Sanitising machine only accepts raw kelp or raw seaweed.");
            return false;
        }

        StartCoroutine(SanitiseRoutine());

        return true;
    }

    private IEnumerator SanitiseRoutine()
    {
        isProcessing = true;
        itemReadyToCollect = false;

        if (whirringEffect != null)
        {
            whirringEffect.SetActive(true);
            Debug.Log("Whirring effect ON.");
        }
        else
        {
            Debug.LogWarning("Whirring Effect is not assigned on SanitisingMachine.");
        }

        if (readyIcon != null)
        {
            readyIcon.SetActive(false);
        }

        Debug.Log("Machine started sanitising.");

        yield return new WaitForSeconds(processingTime);

        isProcessing = false;
        itemReadyToCollect = true;

        if (whirringEffect != null)
        {
            whirringEffect.SetActive(false);
            Debug.Log("Whirring effect OFF.");
        }

        if (readyIcon != null)
        {
            readyIcon.SetActive(true);
            Debug.Log("Ready icon ON.");
        }
        else
        {
            Debug.LogWarning("Ready Icon is not assigned on SanitisingMachine.");
        }

        Debug.Log("Machine finished. Item ready to collect.");
    }

    private void CollectCleanIngredient()
    {
        if (!itemReadyToCollect) return;

        if (IngredientBackpack.Instance != null)
        {
            IngredientBackpack.Instance.AddIngredient(cleanIngredientReady, 1);
        }

        itemReadyToCollect = false;

        if (readyIcon != null)
        {
            readyIcon.SetActive(false);
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