using UnityEngine;
using UnityEngine.UI;

public class ShopScrollController : MonoBehaviour
{
    [Header("Scroll View References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private Scrollbar verticalScrollbar;

    [Header("Scroll Settings")]
    [SerializeField] private float scrollSensitivity = 30f;

    private void Awake()
    {
        SetupScrollView();
    }

    private void SetupScrollView()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        if (scrollRect == null)
        {
            Debug.LogWarning("No ScrollRect found on " + name);
            return;
        }

        // Connect the important parts.
        scrollRect.viewport = viewport;
        scrollRect.content = content;
        scrollRect.verticalScrollbar = verticalScrollbar;

        // Shop only needs vertical scrolling.
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        // Keeps the list from scrolling too far past the edges.
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        // Mouse wheel speed.
        scrollRect.scrollSensitivity = scrollSensitivity;

        // Keep the scrollbar visible.
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
    }

    public void RefreshScrollView(bool resetToTop)
    {
        if (content == null) return;

        // Updates the layout after items are hidden/shown.
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Canvas.ForceUpdateCanvases();

        // Put the shop list back to the top.
        if (resetToTop && scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}