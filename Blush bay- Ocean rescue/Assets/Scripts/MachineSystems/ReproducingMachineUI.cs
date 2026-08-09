using UnityEngine;
using UnityEngine.UI;

public class ReproducingMachineUI : MonoBehaviour
{
    public static ReproducingMachineUI Instance;

    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Buttons")]
    [SerializeField] private Button addCleanKelpButton;
    [SerializeField] private Button addCleanSeaweedButton;
    [SerializeField] private Button closeButton;

    private ReproducingMachine currentMachine;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void Start()
    {
        if (addCleanKelpButton != null)
        {
            addCleanKelpButton.onClick.AddListener(AddCleanKelp);
        }

        if (addCleanSeaweedButton != null)
        {
            addCleanSeaweedButton.onClick.AddListener(AddCleanSeaweed);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
    }

    public void OpenMachine(ReproducingMachine machine)
    {
        currentMachine = machine;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        Debug.Log("Reproducing machine panel opened.");
    }

    private void AddCleanKelp()
    {
        if (currentMachine == null) return;

        bool loaded = currentMachine.TryLoadCleanIngredient(IngredientType.SanitisedKelp);

        if (loaded)
        {
            ClosePanel();
        }
        else
        {
            Debug.Log("Could not add clean kelp.");
        }
    }

    private void AddCleanSeaweed()
    {
        if (currentMachine == null) return;

        bool loaded = currentMachine.TryLoadCleanIngredient(IngredientType.SanitisedSeaweed);

        if (loaded)
        {
            ClosePanel();
        }
        else
        {
            Debug.Log("Could not add clean seaweed.");
        }
    }

    public void ClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        currentMachine = null;

        Debug.Log("Reproducing machine panel closed.");
    }
}