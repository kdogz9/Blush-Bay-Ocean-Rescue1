using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPopUpManager : MonoBehaviour
{
    public static MoneyPopUpManager Instance;

    [Header("Popup Object In Scene")]
    [SerializeField] private GameObject popupObject;
    [SerializeField] private RectTransform popupRect;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Popup UI")]
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image shellIcon;

    [Header("Animation")]
    [SerializeField] private Vector2 startPosition = new Vector2(0f, -40f);
    [SerializeField] private float moveUpDistance = 35f;
    [SerializeField] private float animationTime = 1.2f;

    private Coroutine popupRoutine;

    private void Awake()
    {
        Instance = this;

        if (popupObject != null)
        {
            popupObject.SetActive(false);
        }
    }

    public void ShowMoneyChange(int amount, bool isGain)
    {
        Debug.Log("Money popup called. Amount: " + amount + " Gain: " + isGain);

        if (popupObject == null)
        {
            Debug.LogWarning("MoneyPopUpManager is missing Popup Object.");
            return;
        }

        if (popupRect == null)
        {
            Debug.LogWarning("MoneyPopUpManager is missing Popup Rect.");
            return;
        }

        if (canvasGroup == null)
        {
            Debug.LogWarning("MoneyPopUpManager is missing Canvas Group.");
            return;
        }

        popupObject.SetActive(true);
        popupObject.transform.SetAsLastSibling();

        popupRect.anchoredPosition = startPosition;
        popupRect.localScale = Vector3.one;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (amountText != null)
        {
            amountText.text = isGain ? "+" + amount.ToString() : "-" + amount.ToString();
            amountText.alpha = 1f;
            amountText.gameObject.SetActive(true);
        }

        if (shellIcon != null)
        {
            shellIcon.gameObject.SetActive(true);

            Color iconColour = shellIcon.color;
            iconColour.a = 1f;
            shellIcon.color = iconColour;
        }

        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
        }

        popupRoutine = StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        Vector2 start = startPosition;
        Vector2 end = startPosition + new Vector2(0f, moveUpDistance);

        float timer = 0f;

        while (timer < animationTime)
        {
            timer += Time.deltaTime;

            float progress = timer / animationTime;

            popupRect.anchoredPosition = Vector2.Lerp(start, end, progress);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        popupObject.SetActive(false);
    }
}