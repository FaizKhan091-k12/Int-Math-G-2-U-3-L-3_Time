using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using TMPro;
using DG.Tweening;

public class LevelButtonUI : MonoBehaviour
{
    public Button button;
    public ProceduralImage cardBg;
    public ProceduralImage badgeBg;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Image[] starImages;
    public Sprite filledStarSprite;
    public Sprite emptyStarSprite;
    public GameObject lockOverlay;
    public Image lockIconImage;

    private int levelIndex;
    private bool isUnlocked;

    private static readonly Color[] badgeColors = new Color[]
    {
        new Color(0.18f, 0.52f, 0.88f), // 1. Sky Blue
        new Color(0.92f, 0.52f, 0.15f), // 2. Warm Orange
        new Color(0.12f, 0.68f, 0.55f), // 3. Teal
        new Color(0.58f, 0.32f, 0.82f), // 4. Purple
        new Color(0.88f, 0.30f, 0.35f), // 5. Coral Red
        new Color(0.18f, 0.72f, 0.38f), // 6. Emerald Green
        new Color(0.95f, 0.68f, 0.10f), // 7. Golden Ochre
        new Color(0.25f, 0.40f, 0.65f), // 8. Navy Blue
        new Color(0.85f, 0.25f, 0.60f), // 9. Rose Pink
        new Color(0.80f, 0.65f, 0.12f)  // 10. Royal Gold
    };

    public void Setup(int index, string subtitle, int earnedStars, bool unlocked, System.Action<int> onClick)
    {
        levelIndex = index;
        isUnlocked = unlocked;

        if (titleText != null) titleText.text = $"CASE {index + 1}";
        if (subtitleText != null)
        {
            subtitleText.text = isUnlocked ? subtitle : "LOCKED";
            subtitleText.color = isUnlocked ? new Color(0.12f, 0.16f, 0.25f) : new Color(0.55f, 0.60f, 0.70f);
        }

        if (badgeBg != null)
        {
            Color c = badgeColors[index % badgeColors.Length];
            badgeBg.color = isUnlocked ? c : new Color(0.5f, 0.55f, 0.65f);
        }

        if (cardBg != null)
        {
            cardBg.color = isUnlocked ? new Color(1f, 1f, 1f, 0.98f) : new Color(0.88f, 0.90f, 0.94f, 0.95f);
        }

        if (starImages != null && starImages.Length >= 3)
        {
            for (int i = 0; i < 3; i++)
            {
                if (starImages[i] != null)
                {
                    bool earned = isUnlocked && (i < earnedStars);
                    starImages[i].sprite = earned ? filledStarSprite : emptyStarSprite;
                    starImages[i].color = earned ? Color.white : new Color(1f, 1f, 1f, 0.35f);
                }
            }
        }

        if (lockOverlay != null) lockOverlay.SetActive(!isUnlocked);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (isUnlocked)
                {
                    onClick?.Invoke(levelIndex);
                }
                else
                {
                    // Locked wobble feedback
                    transform.DOKill(true);
                    transform.DOShakePosition(0.35f, new Vector3(8f, 0f, 0f), 15, 90, false, true);
                    AudioManager.Instance?.PlayWrong();
                    MascotDetective.Instance?.Think($"Case {levelIndex + 1} is locked! Clear Case {levelIndex} first!");
                }
            });
        }
    }
}
