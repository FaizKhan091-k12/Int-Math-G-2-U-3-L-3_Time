using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public RectTransform mainMenuPanel;
    public RectTransform levelSelectPanel;
    public RectTransform gameplayPanel;
    public RectTransform feedbackModalPanel;
    public RectTransform victoryPanel;

    [Header("Main Menu UI")]
    public RectTransform titleCardRect;
    public Button playButton;
    public Button levelSelectButton;
    public Button menuMuteButton;
    public Image menuMuteIcon;
    public RectTransform menuMascotRect;

    [Header("Level Select UI")]
    public RectTransform levelHeaderRect;
    public RectTransform levelBoardRect;
    public TextMeshProUGUI totalStarsText;
    public RectTransform levelGridRoot;
    public Button levelSelectBackButton;
    public GameObject levelButtonPrefab;
    public RectTransform levelMascotRect;

    [Header("Gameplay Top Bar")]
    public TextMeshProUGUI caseCounterText;
    public TextMeshProUGUI scoreText;
    public Image progressBarFill;
    public Button hudHomeButton;
    public Button hudMuteButton;
    public Image hudMuteIcon;
    public Button hintButton;

    [Header("Gameplay Question Area")]
    public RectTransform questionCardRect;
    public TextMeshProUGUI questionNumberBadge;
    public TextMeshProUGUI questionPromptText;
    public ClockUI clockUI;

    [Header("Choice Buttons")]
    public RectTransform[] choiceButtonRects = new RectTransform[3];
    public Button[] choiceButtons = new Button[3];
    public ProceduralImage[] choiceCardBgs = new ProceduralImage[3];
    public TextMeshProUGUI[] choiceLetterTexts = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] choiceOptionTexts = new TextMeshProUGUI[3];

    [Header("Feedback Modal")]
    public RectTransform feedbackCardRect;
    public TextMeshProUGUI feedbackTitleText;
    public TextMeshProUGUI feedbackExplanationText;
    public ProceduralImage feedbackHeaderBg;
    public Button feedbackNextButton;

    [Header("Victory Screen")]
    public RectTransform victoryCardRect;
    public Image victoryBadgeImage;
    public TextMeshProUGUI victoryTitleText;
    public TextMeshProUGUI victoryScoreText;
    public TextMeshProUGUI victoryStarsText;
    public Button victoryPlayAgainButton;
    public Button victoryHomeButton;
    public Sprite badgeGold;
    public Sprite badgeSilver;
    public Sprite badgeBronze;

    [Header("Mascot Reference")]
    public MascotDetective mascot;

    // Colors
    private readonly Color normalCardBg = new Color(1f, 1f, 1f, 0.98f);
    private readonly Color normalBorder = new Color(0.85f, 0.88f, 0.95f, 1f);
    private readonly Color correctCardBg = new Color(0.88f, 0.98f, 0.90f, 1f);
    private readonly Color correctBorder = new Color(0.18f, 0.80f, 0.44f, 1f);
    private readonly Color wrongCardBg = new Color(1f, 0.90f, 0.90f, 1f);
    private readonly Color wrongBorder = new Color(0.92f, 0.30f, 0.24f, 1f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

#if UNITY_EDITOR
        if (badgeGold == null)
            badgeGold = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/badge_gold.png");
        if (badgeSilver == null)
            badgeSilver = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/badge_silver.png");
        if (badgeBronze == null)
            badgeBronze = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/badge_bronze.png");
#endif

        AdjustGameplayLayout();
        SetupFeedbackModalVisuals();
    }

    private void SetupFeedbackModalVisuals()
    {
        if (feedbackCardRect != null)
        {
            feedbackCardRect.sizeDelta = new Vector2(740, 390);
        }

        if (feedbackTitleText != null)
        {
            feedbackTitleText.color = Color.white;
            feedbackTitleText.fontSize = 26;
            feedbackTitleText.alignment = TextAlignmentOptions.Center;
            feedbackTitleText.textWrappingMode = TextWrappingModes.NoWrap;
            RectTransform titleRT = feedbackTitleText.GetComponent<RectTransform>();
            if (titleRT != null)
            {
                titleRT.anchorMin = Vector2.zero;
                titleRT.anchorMax = Vector2.one;
                titleRT.offsetMin = new Vector2(16, 0);
                titleRT.offsetMax = new Vector2(-16, 0);
            }
        }

        if (feedbackExplanationText != null)
        {
            feedbackExplanationText.fontSize = 32;
            feedbackExplanationText.color = new Color(0.18f, 0.22f, 0.32f);
            feedbackExplanationText.alignment = TextAlignmentOptions.Center;
            feedbackExplanationText.textWrappingMode = TextWrappingModes.Normal;
        }
    }

    private void AdjustGameplayLayout()
    {
        // Ensure Choice Buttons and Container leave generous room for Detective Toby and the dialogue bubble
        if (choiceButtonRects != null && choiceButtonRects.Length > 0 && choiceButtonRects[0] != null)
        {
            Transform containerTransform = choiceButtonRects[0].parent;
            if (containerTransform != null)
            {
                RectTransform containerRT = containerTransform.GetComponent<RectTransform>();
                if (containerRT != null)
                {
                    containerRT.anchorMin = containerRT.anchorMax = new Vector2(0.61f, 0.38f);
                    containerRT.sizeDelta = new Vector2(500, 430);
                }
            }

            for (int i = 0; i < choiceButtonRects.Length; i++)
            {
                if (choiceButtonRects[i] != null)
                {
                    choiceButtonRects[i].sizeDelta = new Vector2(550, 100);
                }
            }
        }
    }

    private void Start()
    {
        InitButtonListeners();
        UpdateMuteIcons();
    }

    private void InitButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(() => TimeDetectiveGameManager.Instance.StartGameFromFirst());

        if (levelSelectButton != null)
            levelSelectButton.onClick.AddListener(ShowLevelSelect);

        if (levelSelectBackButton != null)
            levelSelectBackButton.onClick.AddListener(ShowMainMenu);

        if (hudHomeButton != null)
            hudHomeButton.onClick.AddListener(ShowMainMenu);

        if (menuMuteButton != null)
            menuMuteButton.onClick.AddListener(OnMuteToggle);

        if (hudMuteButton != null)
            hudMuteButton.onClick.AddListener(OnMuteToggle);

        if (hintButton != null)
            hintButton.onClick.AddListener(() => TimeDetectiveGameManager.Instance.RequestHint());

        if (feedbackNextButton != null)
            feedbackNextButton.onClick.AddListener(() => TimeDetectiveGameManager.Instance.ProceedToNextQuestion());

        if (victoryPlayAgainButton != null)
            victoryPlayAgainButton.onClick.AddListener(() => TimeDetectiveGameManager.Instance.StartGameFromFirst());

        if (victoryHomeButton != null)
            victoryHomeButton.onClick.AddListener(ShowMainMenu);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int idx = i;
            if (choiceButtons[i] != null)
            {
                choiceButtons[i].onClick.AddListener(() => TimeDetectiveGameManager.Instance.SubmitAnswer(idx));
            }
        }
    }

    private void OnMuteToggle()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
            UpdateMuteIcons();
        }
    }

    private void UpdateMuteIcons()
    {
        bool muted = AudioManager.Instance != null && AudioManager.Instance.IsMuted();
        string statusText = muted ? "SFX: OFF" : "SFX: ON";
        if (menuMuteButton != null)
        {
            var txt = menuMuteButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = statusText;
        }
        if (hudMuteButton != null)
        {
            var txt = hudMuteButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = statusText;
        }
    }

    public void ShowMainMenu()
    {
        SwitchPanel(mainMenuPanel);
        AnimateMainMenuEntry();
    }

    private void AnimateMainMenuEntry()
    {
        if (titleCardRect != null)
        {
            titleCardRect.DOKill();
            titleCardRect.localScale = Vector3.zero;
            titleCardRect.DOScale(Vector3.one, 0.45f).SetDelay(0.06f).SetEase(Ease.OutBack);
        }

        if (menuMascotRect != null)
        {
            menuMascotRect.DOKill();
            menuMascotRect.localScale = Vector3.zero;
            menuMascotRect.DOScale(Vector3.one, 0.45f).SetDelay(0.22f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    if (mascot != null)
                    {
                        mascot.StartIdleAnimation();
                        mascot.Say("Welcome, Time Detective! Ready for some clock mysteries?", 5f);
                    }
                });
        }
        else if (mascot != null)
        {
            mascot.StartIdleAnimation();
            mascot.Say("Welcome, Time Detective! Ready for some clock mysteries?", 5f);
        }

        if (playButton != null)
        {
            playButton.transform.DOKill();
            playButton.transform.localScale = Vector3.zero;
            playButton.transform.DOScale(Vector3.one, 0.45f).SetDelay(0.36f).SetEase(Ease.OutBack);
        }

        if (levelSelectButton != null)
        {
            levelSelectButton.transform.DOKill();
            levelSelectButton.transform.localScale = Vector3.zero;
            levelSelectButton.transform.DOScale(Vector3.one, 0.45f).SetDelay(0.48f).SetEase(Ease.OutBack);
        }

        if (menuMuteButton != null)
        {
            menuMuteButton.transform.DOKill();
            menuMuteButton.transform.localScale = Vector3.zero;
            menuMuteButton.transform.DOScale(Vector3.one, 0.40f).SetDelay(0.60f).SetEase(Ease.OutBack);
        }
    }

    public void ShowLevelSelect()
    {
        SwitchPanel(levelSelectPanel);
        PopulateLevelSelectGrid();
        AnimateLevelSelectEntry();
    }

    private void AnimateLevelSelectEntry()
    {
        if (levelSelectBackButton != null)
        {
            levelSelectBackButton.transform.DOKill();
            levelSelectBackButton.transform.localScale = Vector3.zero;
            levelSelectBackButton.transform.DOScale(Vector3.one, 0.35f).SetDelay(0.05f).SetEase(Ease.OutBack);
        }

        if (levelHeaderRect != null)
        {
            levelHeaderRect.DOKill();
            levelHeaderRect.localScale = Vector3.zero;
            levelHeaderRect.DOScale(Vector3.one, 0.40f).SetDelay(0.12f).SetEase(Ease.OutBack);
        }

        if (levelBoardRect != null)
        {
            levelBoardRect.DOKill();
            levelBoardRect.localScale = Vector3.one * 0.88f;
            levelBoardRect.DOScale(Vector3.one, 0.38f).SetDelay(0.15f).SetEase(Ease.OutBack);
        }

        if (totalStarsText != null)
        {
            totalStarsText.transform.DOKill();
            totalStarsText.transform.localScale = Vector3.zero;
            totalStarsText.transform.DOScale(Vector3.one, 0.40f).SetDelay(0.20f).SetEase(Ease.OutBack);
        }

        if (levelMascotRect != null)
        {
            levelMascotRect.DOKill();
            levelMascotRect.localScale = Vector3.zero;
            levelMascotRect.DOScale(Vector3.one, 0.45f).SetDelay(0.30f).SetEase(Ease.OutBack);
        }

        if (mascot != null)
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                mascot.Think("Clear each case to unlock the next mystery!");
            });
        }
    }

    public void ShowGameplay()
    {
        SwitchPanel(gameplayPanel);
    }

    private void SwitchPanel(RectTransform targetPanel)
    {
        if (mainMenuPanel != null) mainMenuPanel.gameObject.SetActive(mainMenuPanel == targetPanel);
        if (levelSelectPanel != null) levelSelectPanel.gameObject.SetActive(levelSelectPanel == targetPanel);
        if (gameplayPanel != null) gameplayPanel.gameObject.SetActive(gameplayPanel == targetPanel);
        if (feedbackModalPanel != null) feedbackModalPanel.gameObject.SetActive(false);
        if (victoryPanel != null) victoryPanel.gameObject.SetActive(victoryPanel == targetPanel);

        if (targetPanel != null)
        {
            targetPanel.localScale = Vector3.one * 0.95f;
            targetPanel.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad);
        }
    }

    public void DisplayQuestion(QuestionData q, int currentIdx, int totalQuestions, int currentScore)
    {
        ShowGameplay();

        // Top bar update
        if (caseCounterText != null)
            caseCounterText.text = $"Case {currentIdx + 1} / {totalQuestions}";

        if (scoreText != null)
            scoreText.text = $"{currentScore} PTS";

        if (progressBarFill != null)
        {
            float targetFill = Mathf.Clamp01((float)currentIdx / totalQuestions);
            progressBarFill.DOKill();
            progressBarFill.DOFillAmount(targetFill, 0.35f).SetEase(Ease.OutCubic);
        }

        // Question Prompt
        if (questionNumberBadge != null)
            questionNumberBadge.text = $"CASE #{currentIdx + 1}";

        if (questionPromptText != null)
        {
            questionPromptText.text = q.prompt;
            questionCardRect.DOKill();
            questionCardRect.anchoredPosition = new Vector2(0, 30);
            questionCardRect.DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
        }

        // Setup Visuals in ClockUI
        if (clockUI != null)
        {
            switch (q.visualMode)
            {
                case QuestionVisualMode.AnalogClock:
                    clockUI.ShowAnalogMode(q.clockHour, q.clockMinute, false);
                    break;
                case QuestionVisualMode.DigitalClock:
                    clockUI.ShowDigitalMode(q.digitalDisplayString, q.clockHour, q.clockMinute);
                    break;
                case QuestionVisualMode.SchoolAssembly:
                    Sprite schoolSprite = Resources.Load<Sprite>("Art/Sprites/school_assembly");
                    if (schoolSprite == null) schoolSprite = TimeDetectiveGameManager.Instance.schoolAssemblySprite;
                    clockUI.ShowContextMedia(schoolSprite, "School Starts at 8:00 AM", 8, 0);
                    break;
                case QuestionVisualMode.MovieCinema:
                    Sprite movieSprite = Resources.Load<Sprite>("Art/Sprites/movie_cinema");
                    if (movieSprite == null) movieSprite = TimeDetectiveGameManager.Instance.movieCinemaSprite;
                    clockUI.ShowContextMedia(movieSprite, "Movie: 4:00 PM to 5:00 PM", 4, 0);
                    break;
                default:
                    clockUI.ShowAnalogMode(q.clockHour, q.clockMinute, false);
                    break;
            }
        }

        // Setup 3 Option Buttons
        string[] letters = { "A", "B", "C" };
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < q.options.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].interactable = true;
                if (choiceLetterTexts[i] != null) choiceLetterTexts[i].text = letters[i];
                if (choiceOptionTexts[i] != null) choiceOptionTexts[i].text = q.options[i];

                ResetChoiceCardVisual(i);

                // Animate card entry staggered
                if (choiceButtonRects[i] != null)
                {
                    choiceButtonRects[i].DOKill();
                    choiceButtonRects[i].localScale = Vector3.zero;
                    choiceButtonRects[i].DOScale(Vector3.one, 0.35f)
                        .SetDelay(0.1f + i * 0.08f)
                        .SetEase(Ease.OutBack);
                }
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        // Mascot prompt
        if (mascot != null)
        {
            mascot.Think(q.detectiveClue);
        }
    }

    public void AnimateAnswerResult(int selectedIdx, bool isCorrect, int correctIdx, System.Action onComplete)
    {
        // Disable buttons
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].interactable = false;
        }

        if (isCorrect)
        {
            HighlightChoiceCard(selectedIdx, correctCardBg, correctBorder);
            if (choiceButtonRects[selectedIdx] != null)
            {
                choiceButtonRects[selectedIdx].DOPunchScale(Vector3.one * 0.14f, 0.4f, 6, 0.8f);
            }

            if (JuiceEffects.Instance != null && choiceButtonRects[selectedIdx] != null)
            {
                JuiceEffects.Instance.TriggerStarBurst(choiceButtonRects[selectedIdx].position, 14);
                JuiceEffects.Instance.TriggerConfettiBurst(choiceButtonRects[selectedIdx].position, 25);
            }

            if (mascot != null) mascot.Celebrate("Brilliant! You found the right clue!");
        }
        else
        {
            HighlightChoiceCard(selectedIdx, wrongCardBg, wrongBorder);
            if (choiceButtonRects[selectedIdx] != null)
            {
                JuiceEffects.Instance?.ShakeTransform(choiceButtonRects[selectedIdx]);
            }

            // Also highlight the correct one softly so kid learns
            HighlightChoiceCard(correctIdx, correctCardBg, correctBorder);

            if (mascot != null) mascot.Think("Good try! Let's inspect the explanation together!");
        }

        DOVirtual.DelayedCall(1.2f, () => onComplete?.Invoke());
    }

    public void AnimateScoreIncrease(int fromScore, int toScore)
    {
        if (scoreText == null) return;

        // Juice punch on Score Badge
        Transform badgeTransform = scoreText.transform.parent != null ? scoreText.transform.parent : scoreText.transform;
        badgeTransform.DOKill(true);
        badgeTransform.DOPunchScale(Vector3.one * 0.25f, 0.5f, 6, 0.7f);

        // Smooth dynamic count-up tween
        DOTween.Kill(scoreText);
        int currentDisplay = fromScore;
        DOTween.To(() => currentDisplay, val =>
        {
            currentDisplay = val;
            scoreText.text = $"{currentDisplay} PTS";
        }, toScore, 0.65f).SetEase(Ease.OutCubic).SetTarget(scoreText);
    }

    public void AnimateProgressBar(int completedQuestions, int totalQuestions)
    {
        if (progressBarFill == null || totalQuestions <= 0) return;

        float targetFill = Mathf.Clamp01((float)completedQuestions / totalQuestions);
        progressBarFill.DOKill();
        progressBarFill.DOFillAmount(targetFill, 0.5f).SetEase(Ease.OutCubic);

        // Flash highlight on the progress bar fill for juicy responsive feedback
        Color originalColor = new Color(0.2f, 0.82f, 0.45f); // vibrant mint green
        Color brightHighlight = new Color(0.55f, 1f, 0.75f, 1f);
        progressBarFill.color = brightHighlight;
        progressBarFill.DOColor(originalColor, 0.4f).SetEase(Ease.OutQuad);

        // Juice punch on Progress Bar background container
        if (progressBarFill.transform.parent != null)
        {
            Transform bgTransform = progressBarFill.transform.parent;
            bgTransform.DOKill(true);
            bgTransform.DOPunchScale(new Vector3(0.06f, 0.15f, 0f), 0.35f, 5, 0.6f);
        }
    }

    private string FormatFeedbackText(bool isCorrect, string explanation, string correctAnswer)
    {
        if (isCorrect)
        {
            return explanation;
        }

        // Celebratory praise openers to strip when the student answered incorrectly
        string[] praisesToRemove = new string[]
        {
            "Spot on!",
            "Brilliant deduction!",
            "Great clue finding!",
            "Awesome job!",
            "Terrific work!",
            "Ding-dong! The school bell rings at 8:00 sharp!",
            "Ding-dong!",
            "Super detective skills!",
            "Popcorn time!",
            "Fantastic deduction!",
            "Case closed, Master Detective!",
            "Hooray!",
            "Well done!",
            "Good job!",
            "Great job!",
            "Excellent work!"
        };

        string clean = explanation != null ? explanation.Trim() : "";
        foreach (string praise in praisesToRemove)
        {
            if (clean.StartsWith(praise, System.StringComparison.OrdinalIgnoreCase))
            {
                clean = clean.Substring(praise.Length).Trim();
                break;
            }
        }

        // Clean any leading punctuation/space left over
        while (clean.StartsWith("!") || clean.StartsWith(".") || clean.StartsWith("-") || clean.StartsWith(":"))
        {
            clean = clean.Substring(1).Trim();
        }

        if (!string.IsNullOrEmpty(clean) && char.IsLower(clean[0]))
        {
            clean = char.ToUpper(clean[0]) + clean.Substring(1);
        }

        if (!string.IsNullOrEmpty(correctAnswer))
        {
            string answerDisplay = correctAnswer.TrimEnd('.');
            return $"Not quite! The correct answer is {answerDisplay}.\n{clean}";
        }
        else
        {
            return $"Not quite! {clean}";
        }
    }

    public void ShowFeedbackModal(bool isCorrect, string explanation, string correctAnswer = null)
    {
        if (feedbackModalPanel == null) return;

        // Dismiss mascot dialogue bubble so the modal stays clean and unobstructed
        if (mascot != null) mascot.HideBubble();

        feedbackModalPanel.gameObject.SetActive(true);
        if (feedbackTitleText != null)
        {
            feedbackTitleText.text = isCorrect ? "EXCELLENT DETECTIVE WORK!" : "DETECTIVE CASE NOTES";
            feedbackTitleText.color = Color.white;
            feedbackTitleText.fontSize = 26;
            feedbackTitleText.alignment = TextAlignmentOptions.Center;
            feedbackTitleText.textWrappingMode = TextWrappingModes.NoWrap;
        }

        if (feedbackHeaderBg != null)
        {
            feedbackHeaderBg.color = isCorrect ? new Color(0.15f, 0.65f, 0.35f) : new Color(0.90f, 0.50f, 0.15f);
        }

        if (feedbackNextButton != null)
        {
            Image nextBtnImg = feedbackNextButton.GetComponent<Image>();
            if (nextBtnImg != null)
            {
                nextBtnImg.color = isCorrect ? new Color(0.15f, 0.65f, 0.35f) : new Color(0.20f, 0.55f, 0.90f);
            }
        }

        if (feedbackExplanationText != null)
        {
            feedbackExplanationText.text = FormatFeedbackText(isCorrect, explanation, correctAnswer);
            feedbackExplanationText.fontSize = 32;
            feedbackExplanationText.color = new Color(0.18f, 0.22f, 0.32f);
            feedbackExplanationText.alignment = TextAlignmentOptions.Center;
            feedbackExplanationText.textWrappingMode = TextWrappingModes.Normal;
        }

        if (feedbackCardRect != null)
        {
            feedbackCardRect.sizeDelta = new Vector2(760, 420);
            feedbackCardRect.DOKill(true);
            feedbackCardRect.localScale = Vector3.zero;
            feedbackCardRect.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }
    }

    public void HideFeedbackModal()
    {
        if (feedbackModalPanel != null)
        {
            if (feedbackCardRect != null)
            {
                feedbackCardRect.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    feedbackModalPanel.gameObject.SetActive(false);
                });
            }
            else
            {
                feedbackModalPanel.gameObject.SetActive(false);
            }
        }
    }

    public void ShowVictoryScreen(int totalScore, int totalStars, int maxStars)
    {
        SwitchPanel(victoryPanel);

        // Determine performance rank tier
        // Tier 1: Master Detective (Score >= 800 or Stars >= 24)
        // Tier 2: Senior Detective (Score >= 500 or Stars >= 15)
        // Tier 3: Apprentice Detective (Score >= 200 or Stars >= 6)
        // Tier 4: Trainee / Needs Practice (Score < 200 or Stars < 6, including Score 0)

        int rankTier;
        if (totalScore >= 800 || totalStars >= 24) rankTier = 1;
        else if (totalScore >= 500 || totalStars >= 15) rankTier = 2;
        else if (totalScore >= 200 || totalStars >= 6) rankTier = 3;
        else rankTier = 4;

        string rankTitle;
        Color titleColor;
        Color borderColor;
        string evaluationMessage;
        string mascotSpeech;
        string playAgainText;
        Sprite badgeToUse;

        switch (rankTier)
        {
            case 1: // Master Detective
                rankTitle = "MASTER TIME DETECTIVE!";
                titleColor = new Color(0.95f, 0.65f, 0.08f); // Brilliant Gold
                borderColor = new Color(1f, 0.85f, 0.25f);
                evaluationMessage = "Outstanding work! You solved all the clock mysteries!";
                mascotSpeech = "Hooray! You're an official Master Time Detective!";
                playAgainText = "PLAY AGAIN";
                badgeToUse = badgeGold != null ? badgeGold : (victoryBadgeImage != null ? victoryBadgeImage.sprite : null);
                break;

            case 2: // Senior Detective
                rankTitle = "SENIOR TIME DETECTIVE!";
                titleColor = new Color(0.12f, 0.68f, 0.42f); // Emerald / Teal
                borderColor = new Color(0.2f, 0.8f, 0.55f);
                evaluationMessage = "Great detective work! You cracked most of the cases!";
                mascotSpeech = "Well done! With just a little more practice, you'll be a Master Detective!";
                playAgainText = "PLAY AGAIN";
                badgeToUse = badgeSilver != null ? badgeSilver : (badgeGold != null ? badgeGold : (victoryBadgeImage != null ? victoryBadgeImage.sprite : null));
                break;

            case 3: // Apprentice Detective
                rankTitle = "APPRENTICE DETECTIVE";
                titleColor = new Color(0.92f, 0.52f, 0.15f); // Warm Amber
                borderColor = new Color(0.95f, 0.6f, 0.2f);
                evaluationMessage = "Good effort! Keep practicing reading the clock hands!";
                mascotSpeech = "Good try! Practice makes every detective sharper!";
                playAgainText = "TRY AGAIN";
                badgeToUse = badgeBronze != null ? badgeBronze : (badgeGold != null ? badgeGold : (victoryBadgeImage != null ? victoryBadgeImage.sprite : null));
                break;

            default: // Trainee Detective (Score 0 or very low)
                rankTitle = "TIME DETECTIVE IN TRAINING";
                titleColor = new Color(0.35f, 0.45f, 0.62f); // Friendly Slate Blue
                borderColor = new Color(0.55f, 0.65f, 0.80f);
                evaluationMessage = "The clock clues were tricky! Review the hands and try again!";
                mascotSpeech = "Don't give up! Inspect the hour and minute hands, and let's solve them together!";
                playAgainText = "TRY AGAIN";
                // Show puppy mascot thinking with magnifying glass instead of misleading gold achievement badge
                badgeToUse = (mascot != null && mascot.thinkSprite != null) ? mascot.thinkSprite : (victoryBadgeImage != null ? victoryBadgeImage.sprite : null);
                break;
        }

        // Apply Title
        if (victoryTitleText != null)
        {
            victoryTitleText.text = rankTitle;
            victoryTitleText.color = titleColor;
        }

        // Apply Score & Stars
        if (victoryScoreText != null)
        {
            victoryScoreText.text = $"Total Score: {totalScore} Points";
        }

        if (victoryStarsText != null)
        {
            victoryStarsText.text = $"Stars Earned: {totalStars} / {maxStars} Stars";
            victoryStarsText.color = (rankTier == 4) ? new Color(0.50f, 0.55f, 0.65f) : new Color(0.95f, 0.7f, 0.15f);
        }

        // Apply Card Outline if present
        if (victoryCardRect != null)
        {
            Outline outline = victoryCardRect.GetComponent<Outline>();
            if (outline != null) outline.effectColor = borderColor;
        }

        // Apply Evaluation Subtitle Text on Card
        EnsureEvaluationText(evaluationMessage, (rankTier == 4) ? new Color(0.35f, 0.42f, 0.55f) : titleColor);

        // Apply Play Again / Try Again button text & styling
        if (victoryPlayAgainButton != null)
        {
            TextMeshProUGUI btnTMP = victoryPlayAgainButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnTMP != null) btnTMP.text = playAgainText;

            Image btnImg = victoryPlayAgainButton.GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.color = (rankTier == 4) ? new Color(0.20f, 0.65f, 0.85f) : new Color(0.18f, 0.75f, 0.38f);
            }
        }

        // Apply Badge
        if (victoryBadgeImage != null)
        {
            if (badgeToUse != null)
            {
                victoryBadgeImage.sprite = badgeToUse;
            }
            victoryBadgeImage.color = Color.white;
            victoryBadgeImage.preserveAspect = true;

            // Elastic pop animation
            victoryBadgeImage.transform.DOKill();
            victoryBadgeImage.transform.localScale = Vector3.zero;
            victoryBadgeImage.transform.DOScale(Vector3.one, 0.6f).SetDelay(0.25f).SetEase(rankTier <= 2 ? Ease.OutElastic : Ease.OutBack);
        }

        // Card Pop In
        if (victoryCardRect != null)
        {
            victoryCardRect.DOKill();
            victoryCardRect.localScale = Vector3.zero;
            victoryCardRect.DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBack);
        }

        // Audio & Juice based on Rank
        if (rankTier == 1)
        {
            AudioManager.Instance?.PlayFanfare();
            if (JuiceEffects.Instance != null && victoryCardRect != null)
            {
                JuiceEffects.Instance.TriggerConfettiBurst(victoryCardRect.position, 60);
                JuiceEffects.Instance.TriggerStarBurst(victoryBadgeImage != null ? victoryBadgeImage.transform.position : victoryCardRect.position, 16);
            }
            if (mascot != null) mascot.Celebrate(mascotSpeech);
        }
        else if (rankTier == 2)
        {
            AudioManager.Instance?.PlayFanfare();
            if (JuiceEffects.Instance != null && victoryCardRect != null)
            {
                JuiceEffects.Instance.TriggerConfettiBurst(victoryCardRect.position, 30);
            }
            if (mascot != null) mascot.Celebrate(mascotSpeech);
        }
        else if (rankTier == 3)
        {
            AudioManager.Instance?.PlayCorrect();
            if (JuiceEffects.Instance != null && victoryCardRect != null)
            {
                JuiceEffects.Instance.TriggerStarBurst(victoryCardRect.position, 10);
            }
            if (mascot != null) mascot.Say(mascotSpeech, 5f);
        }
        else // Rank 4: Trainee / Score 0
        {
            AudioManager.Instance?.PlayTick();
            // No confetti burst on score 0!
            if (mascot != null) mascot.Think(mascotSpeech);
        }
    }

    private TextMeshProUGUI victoryEvaluationText;

    private void EnsureEvaluationText(string message, Color highlightColor)
    {
        if (victoryCardRect == null) return;

        if (victoryEvaluationText == null)
        {
            Transform existing = victoryCardRect.Find("EvaluationText");
            if (existing != null)
            {
                victoryEvaluationText = existing.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                GameObject evalObj = new GameObject("EvaluationText", typeof(RectTransform));
                evalObj.transform.SetParent(victoryCardRect, false);
                RectTransform evalRT = evalObj.GetComponent<RectTransform>();
                evalRT.anchorMin = evalRT.anchorMax = new Vector2(0.5f, 0.21f);
                evalRT.sizeDelta = new Vector2(680, 48);
                victoryEvaluationText = evalObj.AddComponent<TextMeshProUGUI>();
                if (victoryTitleText != null && victoryTitleText.font != null)
                    victoryEvaluationText.font = victoryTitleText.font;
                victoryEvaluationText.fontSize = 21;
                victoryEvaluationText.fontStyle = FontStyles.Bold;
                victoryEvaluationText.alignment = TextAlignmentOptions.Center;
                victoryEvaluationText.textWrappingMode = TextWrappingModes.Normal;
            }
        }

        if (victoryEvaluationText != null)
        {
            victoryEvaluationText.text = message;
            victoryEvaluationText.color = highlightColor;
        }
    }

    private void ResetChoiceCardVisual(int index)
    {
        if (choiceCardBgs[index] != null)
        {
            choiceCardBgs[index].color = normalCardBg;
        }
    }

    private void HighlightChoiceCard(int index, Color bgColor, Color borderColor)
    {
        if (choiceCardBgs[index] != null)
        {
            choiceCardBgs[index].DOColor(bgColor, 0.25f);
        }
    }

    private void PopulateLevelSelectGrid()
    {
        if (levelGridRoot == null || levelButtonPrefab == null) return;

        // Clear existing
        foreach (Transform child in levelGridRoot)
        {
            Destroy(child.gameObject);
        }

        string[] topics = {
            "O'Clock", "Half Past", "Read Hands", "Digital 9:00",
            "+30 Minutes", "School 8:00", "Half Past 10", "Movie Time",
            "+1 Hour", "Half Past 12"
        };

        int total = TimeDetectiveGameManager.Instance.questions.Count;
        int totalEarnedStars = 0;

        for (int i = 0; i < total; i++)
        {
            GameObject btnObj = Instantiate(levelButtonPrefab, levelGridRoot);
            LevelButtonUI btnUI = btnObj.GetComponent<LevelButtonUI>();

            int earnedStars = PlayerPrefs.GetInt($"TimeDet_Stars_{i}", 0);
            totalEarnedStars += earnedStars;

            // Sequential lock: Case 1 is unlocked, next cases require previous case cleared
            bool unlocked = (i == 0) || (PlayerPrefs.GetInt($"TimeDet_Unlocked_{i}", 0) == 1);
            string topic = (i < topics.Length) ? topics[i] : $"Case {i + 1}";

            if (btnUI != null)
            {
                btnUI.Setup(i, topic, earnedStars, unlocked, (lvlIdx) =>
                {
                    TimeDetectiveGameManager.Instance.StartGameAtLevel(lvlIdx);
                });
            }

            // Staggered pop-in animation one by one
            btnObj.transform.localScale = Vector3.zero;
            btnObj.transform.DOScale(Vector3.one, 0.38f)
                .SetDelay(0.22f + (i * 0.07f))
                .SetEase(Ease.OutBack);
        }

        if (totalStarsText != null)
        {
            totalStarsText.text = $"STARS: {totalEarnedStars} / {total * 3}";
        }
    }
}
