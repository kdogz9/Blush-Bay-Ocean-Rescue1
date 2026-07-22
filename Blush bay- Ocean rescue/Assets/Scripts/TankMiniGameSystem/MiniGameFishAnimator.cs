using UnityEngine;
using UnityEngine.UI;

public class MiniGameFishAnimator : MonoBehaviour
{
    [Header("UI Image")]
    [SerializeField] private Image fishImage;

    [Header("Sprite Animation")]
    [SerializeField] private float frameSpeed = 0.25f;

    [Header("Breathing Movement")]
    [SerializeField] private float breathingHeight = 4f;
    [SerializeField] private float breathingSpeed = 2f;

    private Sprite[] idleFrames;
    private RectTransform rectTransform;
    private Vector2 startPosition;

    private int currentFrame;
    private float frameTimer;

    private void Awake()
    {
        if (fishImage == null)
        {
            fishImage = GetComponent<Image>();
        }

        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        AnimateSpriteFrames();
        AnimateBreathing();
    }

    public void SetIdleFrames(Sprite[] newIdleFrames)
    {
        idleFrames = newIdleFrames;
        currentFrame = 0;
        frameTimer = 0f;

        if (idleFrames != null && idleFrames.Length > 0 && fishImage != null)
        {
            fishImage.sprite = idleFrames[0];
            fishImage.enabled = true;
        }
    }

    private void AnimateSpriteFrames()
    {
        if (fishImage == null) return;
        if (idleFrames == null || idleFrames.Length == 0) return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= frameSpeed)
        {
            frameTimer = 0f;

            currentFrame++;

            if (currentFrame >= idleFrames.Length)
            {
                currentFrame = 0;
            }

            fishImage.sprite = idleFrames[currentFrame];
        }
    }

    private void AnimateBreathing()
    {
        if (rectTransform == null) return;

        float yOffset = Mathf.Sin(Time.time * breathingSpeed) * breathingHeight;

        rectTransform.anchoredPosition = startPosition + new Vector2(0f, yOffset);
    }
}