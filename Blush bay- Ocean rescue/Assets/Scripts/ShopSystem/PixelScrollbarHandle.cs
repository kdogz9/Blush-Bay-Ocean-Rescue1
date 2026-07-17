using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PixelScrollbarHandle : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("Scrollbar References")]
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private RectTransform slidingArea;
    [SerializeField] private RectTransform visibleHandle;

    [Header("Handle Visual Size")]
    [SerializeField] private Vector2 handleSize = new Vector2(16f, 70f);

    [Header("Direction")]
    [SerializeField] private bool bottomToTop = true;

    private void Awake()
    {
        SetupVisibleHandle();
        UpdateVisibleHandle();
    }

    private void LateUpdate()
    {
        // Keep the visible handle matched to the real scrollbar value.
        UpdateVisibleHandle();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveScrollbarToPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveScrollbarToPointer(eventData);
    }

    private void SetupVisibleHandle()
    {
        if (visibleHandle == null) return;

        visibleHandle.anchorMin = new Vector2(0.5f, 0.5f);
        visibleHandle.anchorMax = new Vector2(0.5f, 0.5f);
        visibleHandle.pivot = new Vector2(0.5f, 0.5f);
        visibleHandle.sizeDelta = handleSize;
        visibleHandle.localScale = Vector3.one;
    }

    private void UpdateVisibleHandle()
    {
        if (scrollbar == null) return;
        if (slidingArea == null) return;
        if (visibleHandle == null) return;

        SetupVisibleHandle();

        float usableHeight = slidingArea.rect.height - visibleHandle.rect.height;

        if (usableHeight < 0f)
        {
            usableHeight = 0f;
        }

        float bottomY = -usableHeight * 0.5f;
        float topY = usableHeight * 0.5f;

        float value = scrollbar.value;

        if (!bottomToTop)
        {
            value = 1f - value;
        }

        float yPosition = Mathf.Lerp(bottomY, topY, value);

        visibleHandle.anchoredPosition = new Vector2(0f, yPosition);
    }

    private void MoveScrollbarToPointer(PointerEventData eventData)
    {
        if (scrollbar == null) return;
        if (slidingArea == null) return;

        Vector2 localPoint;

        bool gotPoint = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            slidingArea,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        if (!gotPoint) return;

        float halfHandleHeight = handleSize.y * 0.5f;

        float bottomY = slidingArea.rect.yMin + halfHandleHeight;
        float topY = slidingArea.rect.yMax - halfHandleHeight;

        float value = Mathf.InverseLerp(bottomY, topY, localPoint.y);

        if (!bottomToTop)
        {
            value = 1f - value;
        }

        scrollbar.value = Mathf.Clamp01(value);

        UpdateVisibleHandle();
    }
}