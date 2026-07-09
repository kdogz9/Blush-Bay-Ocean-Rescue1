using System.Collections;
using UnityEngine;

public class VisitorThoughtBubble : MonoBehaviour
{
    [Header("Thought Bubble")]
    [SerializeField] private GameObject heartBubble;

    [Header("Settings")]
    [SerializeField] private float showTime = 2f;

    private Coroutine bubbleRoutine;

    private void Start()
    {
        if (heartBubble != null)
        {
            heartBubble.SetActive(false);
        }
    }

    public void ShowHeartBubble()
    {
        if (heartBubble == null) return;

        if (bubbleRoutine != null)
        {
            StopCoroutine(bubbleRoutine);
        }

        bubbleRoutine = StartCoroutine(ShowHeartRoutine());
    }

    public void HideHeartBubble()
    {
        if (heartBubble == null) return;

        if (bubbleRoutine != null)
        {
            StopCoroutine(bubbleRoutine);
            bubbleRoutine = null;
        }

        heartBubble.SetActive(false);
    }

    private IEnumerator ShowHeartRoutine()
    {
        heartBubble.SetActive(true);

        yield return new WaitForSeconds(showTime);

        heartBubble.SetActive(false);

        bubbleRoutine = null;
    }
}