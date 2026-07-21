using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPopUp : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image shellIcon;

    [Header("Animation")]
    [SerializeField] private float moveUpDistance = 30f;
    [SerializeField] private float animationTime = 1.2f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Play(int amount, bool isGain)
    {
        gameObject.SetActive(true);

        // Make sure the popup starts visible.
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (amountText != null)
        {
            amountText.gameObject.SetActive(true);

            if (isGain)
            {
                amountText.text = "+" + amount.ToString();
            }
            else
            {
                amountText.text = "-" + amount.ToString();
            }

            amountText.alpha = 1f;
        }

        if (shellIcon != null)
        {
            shellIcon.gameObject.SetActive(true);

            Color iconColour = shellIcon.color;
            iconColour.a = 1f;
            shellIcon.color = iconColour;
        }

        StopAllCoroutines();
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = startPosition + new Vector2(0f, moveUpDistance);

        float timer = 0f;

        while (timer < animationTime)
        {
            timer += Time.deltaTime;

            float progress = timer / animationTime;

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, progress);

            // Fade out slowly.
            canvasGroup.alpha = 1f;

            yield return null;
        }

        // Destroy(gameObject);
    }
}