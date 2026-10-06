using System.Collections.Generic;
using UnityEngine;

public class TimeDetectiveGameManager : MonoBehaviour
{
    public static TimeDetectiveGameManager Instance;

    [Header("UI Reference")]
    public UIManager uiManager;

    [Header("Visual Asset Sprites")]
    public Sprite schoolAssemblySprite;
    public Sprite movieCinemaSprite;
    public Sprite clockDialSprite;
    public Sprite hourHandSprite;
    public Sprite minuteHandSprite;
    public Sprite pinSprite;

    [Header("Game State")]
    public int currentQuestionIndex = 0;
    public int score = 0;
    public int totalStarsEarned = 0;
    public int attemptsOnCurrentQuestion = 0;
    public bool isSingleLevelMode = false;

    [Header("Questions Dataset")]
    public List<QuestionData> questions = new List<QuestionData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadScriptQuestions();
    }

    [ContextMenu("Reset All Progress")]
    public void ResetAllProgress()
    {
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.DeleteKey($"TimeDet_Unlocked_{i}");
            PlayerPrefs.DeleteKey($"TimeDet_Stars_{i}");
        }
        PlayerPrefs.Save();
        Debug.Log("Reset all Time Detective case unlocks and star progress!");
    }

    private void Start()
    {
        if (uiManager != null)
        {
            uiManager.ShowMainMenu();
        }
    }

    private void LoadScriptQuestions()
    {
        questions.Clear();

        // Question 1
        // The hour hand points to 3 and the minute hand points to 12. What time is it?
        // A. 3:00 (Ans), B. 12:03, C. 3:30
        questions.Add(new QuestionData(
            1,
            "The hour hand points to 3 and the minute hand points to 12. What time is it?",
            QuestionVisualMode.AnalogClock,
            3, 0, "03:00",
            new string[] { "3:00", "12:03", "3:30" },
            0,
            "Spot on! When the long minute hand points up to 12, it is an o'clock time! Since the short hour hand is at 3, it is 3:00!",
            "Look where the short hour hand points, and where the long minute hand points!"
        ));

        // Question 2
        // The digital clock shows 5:30. How do we say this time?
        // A. Half past 5 (Ans), B. Half past 6, C. 5 o'clock
        questions.Add(new QuestionData(
            2,
            "The digital clock shows 5:30. How do we say this time?",
            QuestionVisualMode.DigitalClock,
            5, 30, "05:30",
            new string[] { "Half past 5", "Half past 6", "5 o'clock" },
            0,
            "Brilliant deduction! 30 minutes is half of an hour, so 5:30 is called 'Half past 5'!",
            "30 minutes means half of an hour has passed!"
        ));

        // Question 3
        // The minute hand points to 6 and the hour hand points to 7. What time is shown?
        // A. 7:00, B. 7:30 (Ans), C. 8:30
        questions.Add(new QuestionData(
            3,
            "The minute hand points to 6 and the hour hand points to 7. What time is shown?",
            QuestionVisualMode.AnalogClock,
            7, 30, "07:30",
            new string[] { "7:00", "7:30", "8:30" },
            1,
            "Great clue finding! The minute hand on 6 means 30 minutes (half past). Since the hour hand points to 7, it is 7:30!",
            "Look at the short hour hand pointing to 7, and the long minute hand pointing down at 6!"
        ));

        // Question 4
        // Which digital time shows 9 o'clock? (Analog clock visual at 9:00, choices are digital times)
        // A. 9:30, B. 8:00, C. 9:00 (Ans)
        questions.Add(new QuestionData(
            4,
            "Which digital time shows 9 o'clock?",
            QuestionVisualMode.AnalogClock,
            9, 0, "09:00",
            new string[] { "9:30", "8:00", "9:00" },
            2,
            "Awesome job! An 'o'clock' time has :00 minutes on a digital display, so 9 o'clock is 9:00!",
            "Remember: an 'o'clock' time always ends with :00!"
        ));

        // Question 5
        // Which time comes 30 minutes after 2:00? (Clock displays 2:30, matching Option B: 2:30)
        // A. 2:15, B. 2:30 (Ans), C. 3:00
        questions.Add(new QuestionData(
            5,
            "Which time comes 30 minutes after 2:00?",
            QuestionVisualMode.AnalogClock,
            2, 30, "02:30",
            new string[] { "2:15", "2:30", "3:00" },
            1,
            "Terrific work! Starting at 2:00 and counting 30 minutes ahead brings the clock right to 2:30 (half past 2)!",
            "Look at the clock showing 2:30! That is 30 minutes after 2:00!"
        ));

        // Question 6
        // The school assembly starts at 8:00 a.m. Which clock should show the same time? (Image)
        // A. Hour hand at 8, minute hand at 12 (Ans), B. Hour hand at 12, minute hand at 8, C. Hour hand at 8, minute hand at 6
        questions.Add(new QuestionData(
            6,
            "The school assembly starts at 8:00 a.m. Which clock should show the same time?",
            QuestionVisualMode.SchoolAssembly,
            8, 0, "08:00",
            new string[] { "Hour hand at 8, minute hand at 12", "Hour hand at 12, minute hand at 8", "Hour hand at 8, minute hand at 6" },
            0,
            "Ding-dong! The school bell rings at 8:00 sharp! The short hour hand points at 8 and the long minute hand points straight up at 12!",
            "Short hand = hours (8), Long hand = minutes (:00 points to 12)!"
        ));

        // Question 7
        // What time is half past 10?
        // A. 10:00, B. 10:30 (Ans), C. 11:30
        questions.Add(new QuestionData(
            7,
            "What time is half past 10?",
            QuestionVisualMode.AnalogClock,
            10, 30, "10:30",
            new string[] { "10:00", "10:30", "11:30" },
            1,
            "Super detective skills! 'Half past' always means 30 minutes past the hour, so half past 10 is 10:30!",
            "'Half past' means 30 minutes after the hour!"
        ));

        // Question 8
        // A movie starts at 4:00 p.m. and ends at 5:00 p.m. How long does the movie last?
        // A. 30 minutes, B. 1 hour (Ans), C. 2 hours
        questions.Add(new QuestionData(
            8,
            "A movie starts at 4:00 p.m. and ends at 5:00 p.m. How long does the movie last?",
            QuestionVisualMode.MovieCinema,
            5, 0, "05:00",
            new string[] { "30 minutes", "1 hour", "2 hours" },
            1,
            "Popcorn time! From 4:00 to 5:00 is exactly 1 full hour (60 minutes)!",
            "Count how many hours pass from 4:00 to 5:00!"
        ));

        // Question 9
        // Which time comes 1 hour after 6:30? (Clock displays 7:30, matching Option B: 7:30)
        // A. 7:00, B. 7:30 (Ans), C. 8:30
        questions.Add(new QuestionData(
            9,
            "Which time comes 1 hour after 6:30?",
            QuestionVisualMode.AnalogClock,
            7, 30, "07:30",
            new string[] { "7:00", "7:30", "8:30" },
            1,
            "Fantastic deduction! Adding 1 hour means the hour changes from 6 to 7, while the minutes stay at 30, giving 7:30!",
            "Look at the clock showing 7:30! That is 1 hour after 6:30!"
        ));

        // Question 10
        // The clock shows 12:30. Which statement is correct? (Image)
        // A. It is half past 11., B. It is half past 12. (Ans), C. It is 12 o'clock.
        questions.Add(new QuestionData(
            10,
            "The clock shows 12:30. Which statement is correct?",
            QuestionVisualMode.AnalogClock,
            12, 30, "12:30",
            new string[] { "It is half past 11.", "It is half past 12.", "It is 12 o'clock." },
            1,
            "Case closed, Master Detective! The clock shows 12 hours and 30 minutes, which is 'Half past 12'!",
            "Check both the hour and the 30 minutes on the clock!"
        ));
    }

    public void StartGameFromFirst()
    {
        currentQuestionIndex = 0;
        score = 0;
        totalStarsEarned = 0;
        isSingleLevelMode = false;
        if (uiManager != null && uiManager.progressBarFill != null)
        {
            uiManager.progressBarFill.fillAmount = 0f;
        }
        LoadQuestion(currentQuestionIndex);
    }

    public void StartGameAtLevel(int levelIndex)
    {
        currentQuestionIndex = Mathf.Clamp(levelIndex, 0, questions.Count - 1);
        isSingleLevelMode = true;
        LoadQuestion(currentQuestionIndex);
    }

    private void LoadQuestion(int index)
    {
        attemptsOnCurrentQuestion = 0;
        if (index >= 0 && index < questions.Count)
        {
            uiManager.DisplayQuestion(questions[index], index, questions.Count, score);
        }
        else
        {
            FinishGame();
        }
    }

    public void SubmitAnswer(int chosenOptionIndex)
    {
        if (currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count) return;

        attemptsOnCurrentQuestion++;
        QuestionData q = questions[currentQuestionIndex];
        bool isCorrect = (chosenOptionIndex == q.correctOptionIndex);

        if (isCorrect)
        {
            AudioManager.Instance?.PlayCorrect();
            int pointsEarned = (attemptsOnCurrentQuestion == 1) ? 100 : (attemptsOnCurrentQuestion == 2 ? 60 : 30);
            int starsEarned = (attemptsOnCurrentQuestion == 1) ? 3 : (attemptsOnCurrentQuestion == 2 ? 2 : 1);
            int prevScore = score;
            score += pointsEarned;
            totalStarsEarned += starsEarned;

            // Immediately animate the score counter and progression bar with juice as soon as the answer is submitted
            if (uiManager != null)
            {
                uiManager.AnimateScoreIncrease(prevScore, score);
                uiManager.AnimateProgressBar(currentQuestionIndex + 1, questions.Count);
            }

            // Save high score and stars for level
            int prevStars = PlayerPrefs.GetInt($"TimeDet_Stars_{currentQuestionIndex}", 0);
            if (starsEarned > prevStars)
            {
                PlayerPrefs.SetInt($"TimeDet_Stars_{currentQuestionIndex}", starsEarned);
            }
            // Unlock next level
            if (currentQuestionIndex + 1 < questions.Count)
            {
                PlayerPrefs.SetInt($"TimeDet_Unlocked_{currentQuestionIndex + 1}", 1);
            }
            PlayerPrefs.Save();

            string correctAns = (q.options != null && q.correctOptionIndex < q.options.Length) ? q.options[q.correctOptionIndex] : "";
            uiManager.AnimateAnswerResult(chosenOptionIndex, true, q.correctOptionIndex, () =>
            {
                uiManager.ShowFeedbackModal(true, q.explanation, correctAns);
            });
        }
        else
        {
            AudioManager.Instance?.PlayWrong();
            // Animate progress bar fill to reflect that this case has been answered as well
            if (uiManager != null)
            {
                uiManager.AnimateProgressBar(currentQuestionIndex + 1, questions.Count);
            }
            string correctAns = (q.options != null && q.correctOptionIndex < q.options.Length) ? q.options[q.correctOptionIndex] : "";
            uiManager.AnimateAnswerResult(chosenOptionIndex, false, q.correctOptionIndex, () =>
            {
                uiManager.ShowFeedbackModal(false, q.explanation, correctAns);
            });
        }
    }

    public void RequestHint()
    {
        if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Count)
        {
            AudioManager.Instance?.PlayTick();
            uiManager.mascot?.Think(questions[currentQuestionIndex].detectiveClue);
        }
    }

    public void ProceedToNextQuestion()
    {
        uiManager.HideFeedbackModal();

        if (isSingleLevelMode)
        {
            uiManager.ShowLevelSelect();
            return;
        }

        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            LoadQuestion(currentQuestionIndex);
        }
        else
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        int maxStars = questions.Count * 3;
        uiManager.ShowVictoryScreen(score, totalStarsEarned, maxStars);
    }
}
