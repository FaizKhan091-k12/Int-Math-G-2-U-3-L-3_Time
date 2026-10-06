using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MascotDetective : MonoBehaviour
{
    public static MascotDetective Instance;

    [Header("Sprite / Visuals")]
    public Image mascotImage;
    public Sprite idleSprite;
    public Sprite thinkSprite;
    public Sprite cheerSprite;

    [Header("Speech Bubble")]
    public RectTransform speechBubbleRoot;
    public TextMeshProUGUI speechText;
    public TextMeshProUGUI speechHeader;
    public RectTransform speechBubbleTail;

    [Header("Bubble Sizing & Layout")]
    public float minBubbleWidth = 280f;
    public float maxBubbleWidth = 400f;
    public float minBubbleHeight = 90f;
    public float paddingHorizontal = 42f;
    public float paddingVertical = 30f;
    public float headerSpacing = 24f;

    private RectTransform rectTransform;
    private Vector2 baseAnchoredPos;
    private Vector3 baseScale;
    private Tween idleTween;
    private Tween speechBubbleTween;
    private Tween hideDelayTween;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            baseAnchoredPos = rectTransform.anchoredPosition;
            baseScale = rectTransform.localScale;
        }

        SetupSpeechBubbleLayout();
    }

    public void SetupSpeechBubbleLayout()
    {
        if (speechBubbleRoot == null) return;

        // Bottom pivot so bubble naturally expands UPWARDS into open space above Toby
        speechBubbleRoot.pivot = new Vector2(0.55f, 0f);

        // If parented to the mascot, anchor just above Toby's hat without colliding with choice buttons
        if (speechBubbleRoot.parent == transform)
        {
            speechBubbleRoot.anchorMin = speechBubbleRoot.anchorMax = new Vector2(0.5f, 1.05f);
            speechBubbleRoot.anchoredPosition = new Vector2(-15f, 12f);
        }

        // Configure speech text for clean word-wrapping and Grade 2 readability
        if (speechText != null)
        {
            speechText.textWrappingMode = TextWrappingModes.Normal;
            speechText.overflowMode = TextOverflowModes.Overflow;
            speechText.alignment = TextAlignmentOptions.Center;
            speechText.fontSize = Mathf.Clamp(speechText.fontSize, 18f, 24f);
        }

        // Try to bind speechHeader if unassigned
        if (speechHeader == null && speechBubbleRoot != null)
        {
            Transform headerTrans = speechBubbleRoot.Find("BubbleHeader");
            if (headerTrans != null) speechHeader = headerTrans.GetComponent<TextMeshProUGUI>();
        }

        // Add click listener so tapping bubble dismisses it immediately
        Button bubbleBtn = speechBubbleRoot.GetComponent<Button>();
        if (bubbleBtn == null)
        {
            bubbleBtn = speechBubbleRoot.gameObject.AddComponent<Button>();
            ColorBlock cb = bubbleBtn.colors;
            cb.highlightedColor = new Color(0.96f, 0.98f, 1f, 1f);
            cb.pressedColor = new Color(0.90f, 0.94f, 1f, 1f);
            bubbleBtn.colors = cb;
        }
        bubbleBtn.onClick.RemoveAllListeners();
        bubbleBtn.onClick.AddListener(HideBubble);

        speechBubbleRoot.localScale = Vector3.zero;
        speechBubbleRoot.gameObject.SetActive(false);
    }

    private void Start()
    {
        StartIdleAnimation();
    }

    public void StartIdleAnimation()
    {
        if (rectTransform == null) return;
        if (idleTween != null && idleTween.IsActive()) idleTween.Kill();

        if (mascotImage != null && idleSprite != null)
        {
            mascotImage.sprite = idleSprite;
        }

        rectTransform.anchoredPosition = baseAnchoredPos;
        rectTransform.localScale = baseScale;

        // Friendly breathing & slight bobbing motion
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOScaleY(baseScale.y * 1.04f, 1.0f).SetEase(Ease.InOutSine));
        seq.Join(rectTransform.DOScaleX(baseScale.x * 0.97f, 1.0f).SetEase(Ease.InOutSine));
        seq.Join(rectTransform.DOAnchorPosY(baseAnchoredPos.y + 8f, 1.0f).SetEase(Ease.InOutSine));
        seq.Append(rectTransform.DOScaleY(baseScale.y, 1.0f).SetEase(Ease.InOutSine));
        seq.Join(rectTransform.DOScaleX(baseScale.x, 1.0f).SetEase(Ease.InOutSine));
        seq.Join(rectTransform.DOAnchorPosY(baseAnchoredPos.y, 1.0f).SetEase(Ease.InOutSine));
        seq.SetLoops(-1, LoopType.Restart);

        idleTween = seq;
    }

    public void Think(string clueText = "")
    {
        if (mascotImage != null && thinkSprite != null)
        {
            mascotImage.sprite = thinkSprite;
        }

        if (rectTransform != null)
        {
            rectTransform.DOKill();
            rectTransform.DOPunchRotation(new Vector3(0, 0, -8f), 0.6f, 5, 0.6f)
                .OnComplete(() =>
                {
                    StartIdleAnimation();
                });
        }

        if (!string.IsNullOrEmpty(clueText))
        {
            Say(clueText, 5.0f);
        }
    }

    public void Celebrate(string cheerText = "Case Solved! Brilliant!")
    {
        if (mascotImage != null && cheerSprite != null)
        {
            mascotImage.sprite = cheerSprite;
        }

        if (rectTransform != null)
        {
            rectTransform.DOKill();
            rectTransform.DOJumpAnchorPos(baseAnchoredPos, 28f, 2, 0.7f)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        StartIdleAnimation();
                    });
                });
            rectTransform.DOPunchScale(Vector3.one * 0.18f, 0.5f, 6, 0.8f);
        }

        Say(cheerText, 4.5f);
    }

    public void Say(string text, float duration = 4.5f)
    {
        if (speechBubbleRoot == null || speechText == null) return;

        if (speechBubbleTween != null && speechBubbleTween.IsActive()) speechBubbleTween.Kill();
        if (hideDelayTween != null && hideDelayTween.IsActive()) hideDelayTween.Kill();

        SetupSpeechBubbleLayout();

        speechText.text = text;
        speechText.textWrappingMode = TextWrappingModes.Normal;
        speechText.overflowMode = TextOverflowModes.Overflow;

        // Dynamic measurement based on text length to eliminate clipping
        float maxAvailableTextWidth = maxBubbleWidth - paddingHorizontal;
        Vector2 preferred = speechText.GetPreferredValues(text, maxAvailableTextWidth, 0f);

        float calculatedWidth = Mathf.Clamp(preferred.x + paddingHorizontal, minBubbleWidth, maxBubbleWidth);
        bool hasHeader = speechHeader != null && speechHeader.gameObject.activeSelf;
        float extraHeader = hasHeader ? headerSpacing : 0f;
        float calculatedHeight = Mathf.Max(preferred.y + paddingVertical + extraHeader, minBubbleHeight);

        speechBubbleRoot.sizeDelta = new Vector2(calculatedWidth, calculatedHeight);

        // Adjust text rect inside bubble with comfortable padding and header clearance
        RectTransform textRT = speechText.rectTransform;
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        float topMargin = -(paddingVertical * 0.45f + extraHeader);
        float bottomMargin = paddingVertical * 0.45f;
        textRT.offsetMin = new Vector2(paddingHorizontal * 0.5f, bottomMargin);
        textRT.offsetMax = new Vector2(-paddingHorizontal * 0.5f, topMargin);

        if (hasHeader)
        {
            RectTransform headerRT = speechHeader.rectTransform;
            headerRT.anchorMin = new Vector2(0f, 1f);
            headerRT.anchorMax = new Vector2(1f, 1f);
            headerRT.pivot = new Vector2(0.5f, 1f);
            headerRT.sizeDelta = new Vector2(calculatedWidth - paddingHorizontal, 22f);
            headerRT.anchoredPosition = new Vector2(0f, -paddingVertical * 0.35f);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(speechBubbleRoot);

        speechBubbleRoot.gameObject.SetActive(true);
        speechBubbleRoot.localScale = Vector3.zero;

        // Juicy pop-up animation
        speechBubbleTween = speechBubbleRoot.DOScale(Vector3.one, 0.35f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                hideDelayTween = DOVirtual.DelayedCall(duration, () =>
                {
                    HideBubble();
                });
            });
    }

    public void HideBubble()
    {
        if (speechBubbleRoot == null || !speechBubbleRoot.gameObject.activeSelf) return;

        if (speechBubbleTween != null && speechBubbleTween.IsActive()) speechBubbleTween.Kill();
        if (hideDelayTween != null && hideDelayTween.IsActive()) hideDelayTween.Kill();

        speechBubbleTween = speechBubbleRoot.DOScale(Vector3.zero, 0.22f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                speechBubbleRoot.gameObject.SetActive(false);
            });
    }
}
