using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodMachineUI : MonoBehaviour
{
    public static FoodMachineUI Instance;

    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Amount Text")]
    [SerializeField] private TMP_Text kelpAmountText;
    [SerializeField] private TMP_Text seaweedAmountText;

    [Header("Available Text")]
    [SerializeField] private TMP_Text kelpAvailableText;
    [SerializeField] private TMP_Text seaweedAvailableText;

    [Header("Outcome")]
    [SerializeField] private TMP_Text outcomeText;

    [Header("Kelp Buttons")]
    [SerializeField] private Button addKelpButton;
    [SerializeField] private Button removeKelpButton;

    [Header("Seaweed Buttons")]
    [SerializeField] private Button addSeaweedButton;
    [SerializeField] private Button removeSeaweedButton;

    [Header("Action Buttons")]
    [SerializeField] private Button makeFoodButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button closeButton;

    private FoodMachine currentMachine;

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
            addKelpButton.onClick.AddListener(AddKelp);

        if (removeKelpButton != null)
            removeKelpButton.onClick.AddListener(RemoveKelp);

        if (addSeaweedButton != null)
            addSeaweedButton.onClick.AddListener(AddSeaweed);

        if (removeSeaweedButton != null)
            removeSeaweedButton.onClick.AddListener(RemoveSeaweed);

        if (makeFoodButton != null)
            makeFoodButton.onClick.AddListener(MakeFood);

        if (clearButton != null)
            clearButton.onClick.AddListener(ClearIngredients);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    public void OpenMachine(FoodMachine machine)
    {
        currentMachine = machine;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        UpdateUI();

        Debug.Log("Food machine UI opened.");
    }

    private void AddKelp()
    {
        if (currentMachine == null) return;

        currentMachine.AddKelpAmount();
        UpdateUI();
    }

    private void RemoveKelp()
    {
        if (currentMachine == null) return;

        currentMachine.RemoveKelpAmount();
        UpdateUI();
    }

    private void AddSeaweed()
    {
        if (currentMachine == null) return;

        currentMachine.AddSeaweedAmount();
        UpdateUI();
    }

    private void RemoveSeaweed()
    {
        if (currentMachine == null) return;

        currentMachine.RemoveSeaweedAmount();
        UpdateUI();
    }

    private void MakeFood()
    {
        if (currentMachine == null) return;

        bool started = currentMachine.StartMakingFood();

        if (!started)
        {
            UpdateUI();
        }
    }

    private void ClearIngredients()
    {
        if (currentMachine == null) return;

        currentMachine.ClearSelectedIngredients();
        UpdateUI();
    }

    public void ClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        currentMachine = null;

        Debug.Log("Food machine UI closed.");
    }

    private void UpdateUI()
    {
        if (currentMachine == null) return;

        if (kelpAmountText != null)
        {
            kelpAmountText.text = "x" + currentMachine.SelectedCleanKelp.ToString();
        }

        if (seaweedAmountText != null)
        {
            seaweedAmountText.text = "x" + currentMachine.SelectedCleanSeaweed.ToString();
        }

        if (kelpAvailableText != null && IngredientBackpack.Instance != null)
        {
            kelpAvailableText.text = "Available x" + IngredientBackpack.Instance.SanitisedKelpAmount.ToString();
        }

        if (seaweedAvailableText != null && IngredientBackpack.Instance != null)
        {
            seaweedAvailableText.text = "Available x" + IngredientBackpack.Instance.SanitisedSeaweedAmount.ToString();
        }

        if (outcomeText != null)
        {
            outcomeText.text = currentMachine.GetOutcomeText();
        }

        if (makeFoodButton != null)
        {
            makeFoodButton.gameObject.SetActive(currentMachine.CanMakeFood());
        }
    }
}