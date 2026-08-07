using UnityEngine;
using UnityEngine.UI;

public class SanitisingMachineUI : MonoBehaviour
{
    public static SanitisingMachineUI Instance;

    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Buttons")]
    [SerializeField] private Button addKelpButton;
    [SerializeField] private Button addSeaweedButton;
    [SerializeField] private Button closeButton;

    private SanitisingMachine currentMachine;

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
        if (addKelpButton != null)
        {
            addKelpButton.onClick.AddListener(AddKelp);
        }

        if (addSeaweedButton != null)
        {
            addSeaweedButton.onClick.AddListener(AddSeaweed);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
    }

    public void OpenMachine(SanitisingMachine machine)
    {
        currentMachine = machine;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        Debug.Log("Sanitising panel opened.");
    }

    private void AddKelp()
    {
        if (currentMachine == null) return;

        bool added = currentMachine.TryAddIngredientToMachine(IngredientType.Kelp);

        if (added)
        {
            ClosePanel();
        }
        else
        {
            Debug.Log("Could not add kelp.");
        }
    }

    private void AddSeaweed()
    {
        if (currentMachine == null) return;

        bool added = currentMachine.TryAddIngredientToMachine(IngredientType.Seaweed);

        if (added)
        {
            ClosePanel();
        }
        else
        {
            Debug.Log("Could not add seaweed.");
        }
    }

    public void ClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        currentMachine = null;

        Debug.Log("Sanitising panel closed.");
    }
}