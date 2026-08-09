using UnityEngine;

public class MachineCleaningEffect : MonoBehaviour
{
    [Header("Object To Move")]
    [SerializeField] private Transform machineVisual;

    [Header("Cleaning Visuals")]
    [SerializeField] private GameObject cleaningBubbles;
    [SerializeField] private GameObject readyIcon;

    [Header("Gentle Bobbing")]
    [SerializeField] private float bobSpeed = 4f;
    [SerializeField] private float bobAmount = 0.05f;

    private Vector3 startPosition;
    private bool isCleaning = false;

    private void Awake()
    {
        if (machineVisual == null)
        {
            machineVisual = transform;
        }

        startPosition = machineVisual.localPosition;

        if (cleaningBubbles != null)
        {
            cleaningBubbles.SetActive(false);
        }

        if (readyIcon != null)
        {
            readyIcon.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isCleaning) return;

        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        machineVisual.localPosition = startPosition + new Vector3(0f, yOffset, 0f);
    }

    public void StartCleaningEffect()
    {
        isCleaning = true;

        if (cleaningBubbles != null)
        {
            cleaningBubbles.SetActive(true);
        }

        if (readyIcon != null)
        {
            readyIcon.SetActive(false);
        }

        Debug.Log("Cleaning started: bubbles on, machine bobbing.");
    }

    public void StopCleaningEffect()
    {
        isCleaning = false;

        machineVisual.localPosition = startPosition;

        if (cleaningBubbles != null)
        {
            cleaningBubbles.SetActive(false);
        }

        Debug.Log("Cleaning stopped: bubbles off, machine stopped.");
    }

    public void ShowReadyIcon()
    {
        if (readyIcon != null)
        {
            readyIcon.SetActive(true);
            Debug.Log("Ready icon shown.");
        }
        else
        {
            Debug.LogWarning("Ready Icon is not assigned on MachineCleaningEffect.");
        }
    }

    public void HideReadyIcon()
    {
        if (readyIcon != null)
        {
            readyIcon.SetActive(false);
        }
    }
}