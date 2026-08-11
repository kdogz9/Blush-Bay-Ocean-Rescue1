using UnityEngine;
using UnityEngine.UI;

public class ReproducingMachineUI : MonoBehaviour
{
    public static ReproducingMachineUI Instance;

    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Add Buttons")]
    [SerializeField] private Button addCleanKelpButton;
    [SerializeField] private Button addCleanSeaweedButton;

    [Header("Machine Control Buttons")]
    [SerializeField] private Button collectButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button startAgainButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button resetButton;
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
            addCleanKelpButton.onClick.AddListener(AddCleanKelp);

        if (addCleanSeaweedButton != null)
            addCleanSeaweedButton.onClick.AddListener(AddCleanSeaweed);

        if (collectButton != null)
            collectButton.onClick.AddListener(CollectItems);

        if (stopButton != null)
            stopButton.onClick.AddListener(StopMachine);

        if (startAgainButton != null)
            startAgainButton.onClick.AddListener(StartAgain);

        if (removeButton != null)
            removeButton.onClick.AddListener(RemoveIngredient);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetMachine);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    public void OpenMachine(ReproducingMachine machine)
    {
        currentMachine = machine;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        UpdateButtons();

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
            UpdateButtons();
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
            UpdateButtons();
        }
    }

    private void CollectItems()
    {
        if (currentMachine == null) return;

        bool collected = currentMachine.CollectProducedItems();

        if (collected)
        {
            ClosePanel();
        }
        else
        {
            UpdateButtons();
        }
    }

    private void StopMachine()
    {
        if (currentMachine == null) return;

        currentMachine.StopReproducing();
        UpdateButtons();
    }

    private void StartAgain()
    {
        if (currentMachine == null) return;

        currentMachine.StartReproducing();
        ClosePanel();
    }

    private void RemoveIngredient()
    {
        if (currentMachine == null) return;

        bool removed = currentMachine.RemoveLoadedIngredient();

        if (removed)
        {
            ClosePanel();
        }
        else
        {
            UpdateButtons();
        }
    }

    private void ResetMachine()
    {
        if (currentMachine == null) return;

        currentMachine.ResetMachine();
        ClosePanel();
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

    private void UpdateButtons()
    {
        if (currentMachine == null) return;

        bool empty = !currentMachine.HasIngredientLoaded;
        bool reproducing = currentMachine.IsReproducing;
        bool ready = currentMachine.ItemReadyToCollect;
        bool stoppedWithIngredient =
            currentMachine.HasIngredientLoaded &&
            !currentMachine.IsReproducing &&
            !currentMachine.ItemReadyToCollect;

        if (addCleanKelpButton != null)
            addCleanKelpButton.gameObject.SetActive(empty);

        if (addCleanSeaweedButton != null)
            addCleanSeaweedButton.gameObject.SetActive(empty);

        if (collectButton != null)
            collectButton.gameObject.SetActive(ready);

        if (stopButton != null)
            stopButton.gameObject.SetActive(reproducing);

        if (startAgainButton != null)
            startAgainButton.gameObject.SetActive(stoppedWithIngredient);

        if (removeButton != null)
            removeButton.gameObject.SetActive(stoppedWithIngredient);

        if (resetButton != null)
            resetButton.gameObject.SetActive(!empty);

        if (closeButton != null)
            closeButton.gameObject.SetActive(true);
    }
}