using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneSetupWizard : EditorWindow
{
    [MenuItem("Time Detective/Build Complete Game Scene")]
    public static void BuildScene()
    {
        // 1. Create or open scene
        string scenePath = "Assets/Scenes/SampleScene.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 2. Load Assets
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/bg_detective_hq.png");
        Sprite idleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/detective_idle.png");
        Sprite cheerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/detective_cheer.png");
        Sprite thinkSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/detective_think.png");
        Sprite badgeSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/badge_gold.png");
        Sprite badgeSilverSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/badge_silver.png");
        Sprite badgeBronzeSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/badge_bronze.png");
        Sprite clockDialSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/clock_dial.png");
        Sprite hourHandSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/clock_hand_hour.png");
        Sprite minHandSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/clock_hand_minute.png");
        Sprite pinSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/clock_center_pin.png");
        Sprite starSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/icon_star.png");
        Sprite starEmptySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/icon_star_empty.png");
        Sprite lockSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/icon_lock.png");
        Sprite schoolSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/school_assembly.png");
        Sprite movieSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/movie_cinema.png");

        TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (defaultFont == null)
        {
            string[] fontGuids = AssetDatabase.FindAssets("LiberationSans SDF t:TMP_FontAsset");
            if (fontGuids.Length > 0)
            {
                defaultFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(fontGuids[0]));
            }
        }

        // 3. Camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.14f, 0.22f, 0.35f);
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cameraObj.AddComponent<AudioListener>();

        // 4. EventSystem
        GameObject eventObj = new GameObject("EventSystem");
        eventObj.AddComponent<EventSystem>();
        eventObj.AddComponent<StandaloneInputModule>();

        // 5. Canvas
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        // 6. Background Image
        GameObject bgObj = CreateUIObject("Background", canvasObj.transform);
        StretchFull(bgObj.GetComponent<RectTransform>());
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.sprite = bgSprite;
        bgImg.color = Color.white;
        bgImg.raycastTarget = false;

        // Overlay vignette for crisp contrast
        GameObject bgTintObj = CreateUIObject("BgTint", canvasObj.transform);
        StretchFull(bgTintObj.GetComponent<RectTransform>());
        Image bgTintImg = bgTintObj.AddComponent<Image>();
        bgTintImg.color = new Color(0.05f, 0.1f, 0.2f, 0.35f);
        bgTintImg.raycastTarget = false;

        // 7. Managers Root
        GameObject managersObj = new GameObject("GameManagers");
        AudioManager audioMgr = managersObj.AddComponent<AudioManager>();
        JuiceEffects juiceFX = managersObj.AddComponent<JuiceEffects>();
        juiceFX.fxCanvasTransform = canvasObj.GetComponent<RectTransform>();
        juiceFX.starSprite = starSprite;
        TimeDetectiveGameManager gameMgr = managersObj.AddComponent<TimeDetectiveGameManager>();
        UIManager uiMgr = managersObj.AddComponent<UIManager>();
        gameMgr.uiManager = uiMgr;

        gameMgr.clockDialSprite = clockDialSprite;
        gameMgr.hourHandSprite = hourHandSprite;
        gameMgr.minuteHandSprite = minHandSprite;
        gameMgr.pinSprite = pinSprite;
        gameMgr.schoolAssemblySprite = schoolSprite;
        gameMgr.movieCinemaSprite = movieSprite;

        // Create Level Button Prefab
        GameObject levelBtnPrefab = CreateLevelButtonPrefab(starSprite, starEmptySprite, lockSprite, defaultFont);
        uiMgr.levelButtonPrefab = levelBtnPrefab;

        // Edovu Logo
        GameObject logoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Edovu Logo .prefab");
        if (logoPrefab != null)
        {
            GameObject logoInstance = (GameObject)PrefabUtility.InstantiatePrefab(logoPrefab, canvasObj.transform);
            RectTransform logoRT = logoInstance.GetComponent<RectTransform>();
            if (logoRT != null)
            {
                logoRT.anchorMin = new Vector2(1, 1);
                logoRT.anchorMax = new Vector2(1, 1);
                logoRT.pivot = new Vector2(1, 1);
                logoRT.anchoredPosition = new Vector2(-15, -15);
                logoRT.sizeDelta = new Vector2(100, 100);
            }
        }

        // 8. Panels
        // ----------------- MAIN MENU PANEL -----------------
        GameObject menuPanelObj = CreateUIObject("MainMenuPanel", canvasObj.transform);
        RectTransform menuPanelRT = menuPanelObj.GetComponent<RectTransform>();
        StretchFull(menuPanelRT);
        uiMgr.mainMenuPanel = menuPanelRT;

        // Title Box
        GameObject titleBox = CreateUIObject("TitleCard", menuPanelRT);
        RectTransform titleRT = titleBox.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 0.72f);
        titleRT.anchorMax = new Vector2(0.5f, 0.72f);
        titleRT.sizeDelta = new Vector2(1050, 220);
        titleRT.anchoredPosition = Vector2.zero;
        AddProceduralCard(titleBox, new Color(0.08f, 0.18f, 0.35f, 0.92f), 30f, 0f, Color.clear);
        uiMgr.titleCardRect = titleRT;

        GameObject titleTextObj = CreateUIObject("TitleText", titleBox.transform);
        RectTransform titleTextRT = titleTextObj.GetComponent<RectTransform>();
        titleTextRT.anchorMin = new Vector2(0, 0.35f);
        titleTextRT.anchorMax = new Vector2(1, 1f);
        titleTextRT.offsetMin = titleTextRT.offsetMax = Vector2.zero;
        TextMeshProUGUI titleTMP = titleTextObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) titleTMP.font = defaultFont;
        titleTMP.text = "TIME DETECTIVE";
        titleTMP.fontSize = 72;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.color = new Color(1f, 0.88f, 0.25f, 1f);
        titleTMP.alignment = TextAlignmentOptions.Center;

        GameObject subtitleTextObj = CreateUIObject("SubtitleText", titleBox.transform);
        RectTransform subRT = subtitleTextObj.GetComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0, 0f);
        subRT.anchorMax = new Vector2(1, 0.42f);
        subRT.offsetMin = subRT.offsetMax = Vector2.zero;
        TextMeshProUGUI subTMP = subtitleTextObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) subTMP.font = defaultFont;
        subTMP.text = "READ THE CLOCK & SOLVE THE MYSTERIES!";
        subTMP.fontSize = 32;
        subTMP.fontStyle = FontStyles.Bold;
        subTMP.color = new Color(0.9f, 0.95f, 1f, 1f);
        subTMP.alignment = TextAlignmentOptions.Center;

        // Menu Play Button
        GameObject playBtnObj = CreateButton("PlayButton", menuPanelRT, new Vector2(0.5f, 0.42f), new Vector2(360, 95), 
            new Color(0.18f, 0.78f, 0.38f), "PLAY NOW", 42, defaultFont);
        uiMgr.playButton = playBtnObj.GetComponent<Button>();

        // Menu Level Select Button
        GameObject lvlSelectBtnObj = CreateButton("LevelSelectButton", menuPanelRT, new Vector2(0.5f, 0.28f), new Vector2(320, 80), 
            new Color(0.2f, 0.55f, 0.9f), "CASE FILES", 32, defaultFont);
        uiMgr.levelSelectButton = lvlSelectBtnObj.GetComponent<Button>();

        // Menu Mute Button
        GameObject menuMuteObj = CreateButton("MuteButton", menuPanelRT, new Vector2(0.94f, 0.92f), new Vector2(85, 65), 
            new Color(0.2f, 0.25f, 0.35f, 0.85f), "SFX", 26, defaultFont);
        uiMgr.menuMuteButton = menuMuteObj.GetComponent<Button>();
        uiMgr.menuMuteIcon = menuMuteObj.GetComponent<Image>();

        // Mascot on Menu
        GameObject menuMascotObj = CreateUIObject("MenuMascot", menuPanelRT);
        RectTransform menuMascotRT = menuMascotObj.GetComponent<RectTransform>();
        menuMascotRT.anchorMin = menuMascotRT.anchorMax = new Vector2(0.2f, 0.35f);
        menuMascotRT.sizeDelta = new Vector2(320, 320);
        Image menuMascotImg = menuMascotObj.AddComponent<Image>();
        menuMascotImg.sprite = idleSprite;
        menuMascotImg.preserveAspect = true;
        uiMgr.menuMascotRect = menuMascotRT;

        // ----------------- LEVEL SELECT PANEL -----------------
        GameObject levelPanelObj = CreateUIObject("LevelSelectPanel", canvasObj.transform);
        RectTransform levelPanelRT = levelPanelObj.GetComponent<RectTransform>();
        StretchFull(levelPanelRT);
        uiMgr.levelSelectPanel = levelPanelRT;
        levelPanelObj.SetActive(false);

        // Header Card
        GameObject levelHeader = CreateUIObject("Header", levelPanelRT);
        RectTransform levelHeaderRT = levelHeader.GetComponent<RectTransform>();
        levelHeaderRT.anchorMin = levelHeaderRT.anchorMax = new Vector2(0.5f, 0.91f);
        levelHeaderRT.sizeDelta = new Vector2(680, 85);
        AddProceduralCard(levelHeader, new Color(0.08f, 0.18f, 0.35f, 0.95f), 25f, 0f, Color.clear);
        TextMeshProUGUI levelHeaderTMP = CreateText("HeaderText", levelHeader.transform, "DETECTIVE CASE FILES", 42, Color.white, defaultFont);
        levelHeaderTMP.fontStyle = FontStyles.Bold;
        uiMgr.levelHeaderRect = levelHeaderRT;

        // Total Stars Tally Badge
        GameObject starsBadgeObj = CreateUIObject("TotalStarsBadge", levelPanelRT);
        RectTransform starsBadgeRT = starsBadgeObj.GetComponent<RectTransform>();
        starsBadgeRT.anchorMin = starsBadgeRT.anchorMax = new Vector2(0.86f, 0.91f);
        starsBadgeRT.sizeDelta = new Vector2(240, 68);
        AddProceduralCard(starsBadgeObj, new Color(0.95f, 0.72f, 0.12f, 0.98f), 24f, 0f, Color.clear);
        TextMeshProUGUI starsBadgeTMP = CreateText("StarsText", starsBadgeObj.transform, "STARS: 0 / 30", 24, new Color(0.18f, 0.12f, 0.05f), defaultFont);
        starsBadgeTMP.fontStyle = FontStyles.Bold;
        uiMgr.totalStarsText = starsBadgeTMP;

        // Back button
        GameObject lvlBackBtnObj = CreateButton("BackButton", levelPanelRT, new Vector2(0.08f, 0.91f), new Vector2(150, 65), 
            new Color(0.9f, 0.35f, 0.3f), "BACK", 26, defaultFont);
        uiMgr.levelSelectBackButton = lvlBackBtnObj.GetComponent<Button>();

        // Detective Case Files Board (Fills the screen and anchors the 10 case cards!)
        GameObject boardObj = CreateUIObject("CaseFilesBoard", levelPanelRT);
        RectTransform boardRT = boardObj.GetComponent<RectTransform>();
        boardRT.anchorMin = new Vector2(0.5f, 0.47f);
        boardRT.anchorMax = new Vector2(0.5f, 0.47f);
        boardRT.sizeDelta = new Vector2(1380, 560);
        boardRT.anchoredPosition = Vector2.zero;
        AddProceduralCard(boardObj, new Color(0.07f, 0.13f, 0.24f, 0.92f), 30f, 0f, Color.clear);
        uiMgr.levelBoardRect = boardRT;

        // Level Grid inside the Board
        GameObject levelGridObj = CreateUIObject("LevelGrid", boardObj.transform);
        RectTransform levelGridRT = levelGridObj.GetComponent<RectTransform>();
        StretchFull(levelGridRT);
        levelGridRT.offsetMin = new Vector2(25, 20);
        levelGridRT.offsetMax = new Vector2(-25, -20);
        GridLayoutGroup grid = levelGridObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(245, 160);
        grid.spacing = new Vector2(20, 20);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        uiMgr.levelGridRoot = levelGridRT;

        // Mascot Toby on Level Select
        GameObject lvlMascotObj = CreateUIObject("LevelSelectMascot", levelPanelRT);
        RectTransform lvlMascotRT = lvlMascotObj.GetComponent<RectTransform>();
        lvlMascotRT.anchorMin = lvlMascotRT.anchorMax = new Vector2(0.94f, 0.18f);
        lvlMascotRT.sizeDelta = new Vector2(240, 240);
        Image lvlMascotImg = lvlMascotObj.AddComponent<Image>();
        lvlMascotImg.sprite = thinkSprite;
        lvlMascotImg.preserveAspect = true;
        uiMgr.levelMascotRect = lvlMascotRT;

        // ----------------- GAMEPLAY PANEL -----------------
        GameObject gamePanelObj = CreateUIObject("GameplayPanel", canvasObj.transform);
        RectTransform gamePanelRT = gamePanelObj.GetComponent<RectTransform>();
        StretchFull(gamePanelRT);
        uiMgr.gameplayPanel = gamePanelRT;
        gamePanelObj.SetActive(false);

        // Top Bar
        GameObject topBarObj = CreateUIObject("TopBar", gamePanelRT);
        RectTransform topBarRT = topBarObj.GetComponent<RectTransform>();
        topBarRT.anchorMin = new Vector2(0.02f, 0.90f);
        topBarRT.anchorMax = new Vector2(0.98f, 0.98f);
        topBarRT.offsetMin = topBarRT.offsetMax = Vector2.zero;
        AddProceduralCard(topBarObj, new Color(0.08f, 0.16f, 0.30f, 0.92f), 20f, 2f, new Color(0.3f, 0.5f, 0.8f, 0.8f));

        // Home Button
        GameObject hudHomeObj = CreateButton("HomeBtn", topBarRT, new Vector2(0.04f, 0.5f), new Vector2(100, 52), 
            new Color(0.25f, 0.45f, 0.7f), "MENU", 24, defaultFont);
        uiMgr.hudHomeButton = hudHomeObj.GetComponent<Button>();

        // Case Counter
        GameObject caseCounterObj = CreateUIObject("CaseCounter", topBarRT);
        RectTransform caseCounterRT = caseCounterObj.GetComponent<RectTransform>();
        caseCounterRT.anchorMin = caseCounterRT.anchorMax = new Vector2(0.18f, 0.5f);
        caseCounterRT.sizeDelta = new Vector2(200, 50);
        TextMeshProUGUI caseTMP = caseCounterObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) caseTMP.font = defaultFont;
        caseTMP.text = "Case 1 / 10";
        caseTMP.fontSize = 28;
        caseTMP.fontStyle = FontStyles.Bold;
        caseTMP.color = Color.white;
        caseTMP.alignment = TextAlignmentOptions.Center;
        uiMgr.caseCounterText = caseTMP;

        // Progress Bar
        GameObject progressBg = CreateUIObject("ProgressBarBg", topBarRT);
        RectTransform progressBgRT = progressBg.GetComponent<RectTransform>();
        progressBgRT.anchorMin = progressBgRT.anchorMax = new Vector2(0.48f, 0.5f);
        progressBgRT.sizeDelta = new Vector2(380, 26);
        AddProceduralCard(progressBg, new Color(0.15f, 0.22f, 0.35f, 1f), 13f, 2f, new Color(0.3f, 0.4f, 0.6f, 1f));

        GameObject progressFill = CreateUIObject("ProgressFill", progressBg.transform);
        RectTransform progressFillRT = progressFill.GetComponent<RectTransform>();
        StretchFull(progressFillRT);
        progressFillRT.offsetMin = new Vector2(3, 3);
        progressFillRT.offsetMax = new Vector2(-3, -3);
        Image fillImg = progressFill.AddComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 0.1f;
        fillImg.color = new Color(0.2f, 0.82f, 0.45f);
        uiMgr.progressBarFill = fillImg;

        // Score Badge (With Real Golden Star Sprite Icon)
        GameObject scoreBadge = CreateUIObject("ScoreBadge", topBarRT);
        RectTransform scoreBadgeRT = scoreBadge.GetComponent<RectTransform>();
        scoreBadgeRT.anchorMin = scoreBadgeRT.anchorMax = new Vector2(0.72f, 0.5f);
        scoreBadgeRT.sizeDelta = new Vector2(170, 52);
        AddProceduralCard(scoreBadge, new Color(0.95f, 0.72f, 0.12f, 0.95f), 26f, 2f, Color.white);

        GameObject starIconObj = CreateUIObject("StarIcon", scoreBadge.transform);
        RectTransform starIconRT = starIconObj.GetComponent<RectTransform>();
        starIconRT.anchorMin = starIconRT.anchorMax = new Vector2(0.22f, 0.5f);
        starIconRT.sizeDelta = new Vector2(34, 34);
        Image starIconImg = starIconObj.AddComponent<Image>();
        starIconImg.sprite = starSprite;

        TextMeshProUGUI scoreTMP = CreateText("ScoreText", scoreBadge.transform, "0 PTS", 26, new Color(0.2f, 0.15f, 0.05f), defaultFont);
        scoreTMP.fontStyle = FontStyles.Bold;
        RectTransform scoreTextRT = scoreTMP.GetComponent<RectTransform>();
        scoreTextRT.anchorMin = new Vector2(0.38f, 0f);
        scoreTextRT.anchorMax = new Vector2(0.96f, 1f);
        scoreTextRT.offsetMin = scoreTextRT.offsetMax = Vector2.zero;
        scoreTMP.alignment = TextAlignmentOptions.MidlineLeft;
        uiMgr.scoreText = scoreTMP;

        // Hint Button
        GameObject hintBtnObj = CreateButton("HintBtn", topBarRT, new Vector2(0.85f, 0.5f), new Vector2(110, 52), 
            new Color(0.9f, 0.55f, 0.15f), "HINT", 22, defaultFont);
        uiMgr.hintButton = hintBtnObj.GetComponent<Button>();

        // Mute Button in HUD
        GameObject hudMuteObj = CreateButton("MuteBtn", topBarRT, new Vector2(0.95f, 0.5f), new Vector2(75, 52), 
            new Color(0.3f, 0.4f, 0.55f), "SFX", 22, defaultFont);
        uiMgr.hudMuteButton = hudMuteObj.GetComponent<Button>();
        uiMgr.hudMuteIcon = hudMuteObj.GetComponent<Image>();

        // Question Card
        GameObject questionCardObj = CreateUIObject("QuestionCard", gamePanelRT);
        RectTransform qCardRT = questionCardObj.GetComponent<RectTransform>();
        qCardRT.anchorMin = qCardRT.anchorMax = new Vector2(0.5f, 0.76f);
        qCardRT.sizeDelta = new Vector2(1300, 150);
        AddProceduralCard(questionCardObj, new Color(0.98f, 0.99f, 1f, 0.97f), 26f, 4f, new Color(0.80f, 0.85f, 0.95f, 1f));
        uiMgr.questionCardRect = qCardRT;

        GameObject badgeObj = CreateUIObject("CaseBadge", questionCardObj.transform);
        RectTransform badgeRT = badgeObj.GetComponent<RectTransform>();
        badgeRT.anchorMin = badgeRT.anchorMax = new Vector2(0.08f, 0.5f);
        badgeRT.sizeDelta = new Vector2(130, 48);
        AddProceduralCard(badgeObj, new Color(0.2f, 0.48f, 0.85f), 24f, 0f, Color.clear);
        TextMeshProUGUI badgeTMP = CreateText("Text", badgeObj.transform, "CASE #1", 20, Color.white, defaultFont);
        badgeTMP.fontStyle = FontStyles.Bold;
        uiMgr.questionNumberBadge = badgeTMP;

        GameObject qPromptObj = CreateUIObject("PromptText", questionCardObj.transform);
        RectTransform promptRT = qPromptObj.GetComponent<RectTransform>();
        promptRT.anchorMin = new Vector2(0.16f, 0.05f);
        promptRT.anchorMax = new Vector2(0.98f, 0.95f);
        promptRT.offsetMin = promptRT.offsetMax = Vector2.zero;
        TextMeshProUGUI promptTMP = qPromptObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) promptTMP.font = defaultFont;
        promptTMP.text = "The hour hand points to 3 and the minute hand points to 12. What time is it?";
        promptTMP.fontSize = 32;
        promptTMP.color = new Color(0.12f, 0.15f, 0.22f);
        promptTMP.alignment = TextAlignmentOptions.MidlineLeft;
        uiMgr.questionPromptText = promptTMP;

        // Center Visual Clock Area
        GameObject clockRootObj = CreateUIObject("ClockArea", gamePanelRT);
        RectTransform clockRootRT = clockRootObj.GetComponent<RectTransform>();
        clockRootRT.anchorMin = clockRootRT.anchorMax = new Vector2(0.29f, 0.38f);
        clockRootRT.sizeDelta = new Vector2(440, 440);
        ClockUI clockUI = clockRootObj.AddComponent<ClockUI>();
        uiMgr.clockUI = clockUI;

        // Analog Clock Container
        GameObject analogClockObj = CreateUIObject("AnalogClock", clockRootObj.transform);
        RectTransform analogRT = analogClockObj.GetComponent<RectTransform>();
        StretchFull(analogRT);
        clockUI.analogClockContainer = analogRT;

        // Dial
        GameObject dialObj = CreateUIObject("Dial", analogClockObj.transform);
        RectTransform dialRT = dialObj.GetComponent<RectTransform>();
        dialRT.sizeDelta = new Vector2(420, 420);
        Image dialImg = dialObj.AddComponent<Image>();
        dialImg.sprite = clockDialSprite;
        clockUI.dialImage = dialImg;

        // Minute Hand (Longer, orange/red)
        GameObject minHandObj = CreateUIObject("MinuteHand", analogClockObj.transform);
        RectTransform minHandRT = minHandObj.GetComponent<RectTransform>();
        minHandRT.sizeDelta = new Vector2(48, 202);
        minHandRT.pivot = new Vector2(0.5f, 0.1185f);
        minHandRT.anchoredPosition = Vector2.zero;
        Image minHandImg = minHandObj.AddComponent<Image>();
        minHandImg.sprite = minHandSprite;
        clockUI.minuteHandRect = minHandRT;

        // Hour Hand (Shorter, blue)
        GameObject hourHandObj = CreateUIObject("HourHand", analogClockObj.transform);
        RectTransform hourHandRT = hourHandObj.GetComponent<RectTransform>();
        hourHandRT.sizeDelta = new Vector2(48, 142);
        hourHandRT.pivot = new Vector2(0.5f, 0.1684f);
        hourHandRT.anchoredPosition = Vector2.zero;
        Image hourHandImg = hourHandObj.AddComponent<Image>();
        hourHandImg.sprite = hourHandSprite;
        clockUI.hourHandRect = hourHandRT;

        // Center Pin
        GameObject pinObj = CreateUIObject("CenterPin", analogClockObj.transform);
        RectTransform pinRT = pinObj.GetComponent<RectTransform>();
        pinRT.sizeDelta = new Vector2(46, 46);
        pinRT.anchoredPosition = Vector2.zero;
        Image pinImg = pinObj.AddComponent<Image>();
        pinImg.sprite = pinSprite;
        clockUI.centerPinImage = pinImg;

        // Digital Clock Container (Optional readout or dedicated mode)
        GameObject digitalClockObj = CreateUIObject("DigitalClock", clockRootObj.transform);
        RectTransform digRT = digitalClockObj.GetComponent<RectTransform>();
        digRT.anchorMin = digRT.anchorMax = new Vector2(0.5f, -0.05f);
        digRT.sizeDelta = new Vector2(250, 75);
        AddProceduralCard(digitalClockObj, new Color(0.08f, 0.12f, 0.20f, 0.95f), 18f, 3f, new Color(0.2f, 0.7f, 0.9f, 0.9f));
        TextMeshProUGUI digTMP = CreateText("DigitalText", digitalClockObj.transform, "05:30", 38, new Color(0.2f, 0.95f, 0.6f), defaultFont);
        digTMP.fontStyle = FontStyles.Bold;
        clockUI.digitalClockContainer = digRT;
        clockUI.digitalTimeText = digTMP;

        // Context Media (For school / movie questions)
        GameObject contextMediaObj = CreateUIObject("ContextMedia", clockRootObj.transform);
        RectTransform contextRT = contextMediaObj.GetComponent<RectTransform>();
        StretchFull(contextRT);
        AddProceduralCard(contextMediaObj, Color.white, 24f, 4f, new Color(0.85f, 0.90f, 0.98f));
        GameObject contextImgObj = CreateUIObject("MediaImg", contextMediaObj.transform);
        RectTransform cImgRT = contextImgObj.GetComponent<RectTransform>();
        cImgRT.anchorMin = new Vector2(0.05f, 0.18f);
        cImgRT.anchorMax = new Vector2(0.95f, 0.95f);
        cImgRT.offsetMin = cImgRT.offsetMax = Vector2.zero;
        Image cImg = contextImgObj.AddComponent<Image>();
        cImg.preserveAspect = true;
        TextMeshProUGUI cSubTMP = CreateText("Label", contextMediaObj.transform, "Morning School Assembly", 22, new Color(0.2f, 0.25f, 0.35f), defaultFont);
        RectTransform cSubRT = cSubTMP.GetComponent<RectTransform>();
        cSubRT.anchorMin = new Vector2(0, 0);
        cSubRT.anchorMax = new Vector2(1, 0.18f);
        cSubRT.offsetMin = cSubRT.offsetMax = Vector2.zero;
        clockUI.contextMediaContainer = contextRT;
        clockUI.contextImage = cImg;
        clockUI.contextSubtext = cSubTMP;
        contextMediaObj.SetActive(false);

        // Right Choices Container (Balanced layout leaving clear room for Mascot & Bubble)
        GameObject choicesRootObj = CreateUIObject("ChoicesContainer", gamePanelRT);
        RectTransform choicesRootRT = choicesRootObj.GetComponent<RectTransform>();
        choicesRootRT.anchorMin = choicesRootRT.anchorMax = new Vector2(0.61f, 0.38f);
        choicesRootRT.sizeDelta = new Vector2(500, 430);

        string[] choiceLetters = { "A", "B", "C" };
        Color[] letterBadgeColors = {
            new Color(0.2f, 0.55f, 0.9f),
            new Color(0.9f, 0.5f, 0.15f),
            new Color(0.6f, 0.3f, 0.85f)
        };

        for (int i = 0; i < 3; i++)
        {
            float yPos = 125 - (i * 125);
            GameObject choiceCard = CreateUIObject($"ChoiceCard_{choiceLetters[i]}", choicesRootObj.transform);
            RectTransform cardRT = choiceCard.GetComponent<RectTransform>();
            cardRT.anchorMin = cardRT.anchorMax = new Vector2(0.5f, 0.5f);
            cardRT.sizeDelta = new Vector2(490, 100);
            cardRT.anchoredPosition = new Vector2(0, yPos);

            ProceduralImage pImg = AddProceduralCard(choiceCard, new Color(1f, 1f, 1f, 0.98f), 24f, 3f, new Color(0.85f, 0.88f, 0.95f));
            Button btn = choiceCard.AddComponent<Button>();
            choiceCard.AddComponent<UIButtonJuice>();

            // Letter Badge
            GameObject badgeCir = CreateUIObject("LetterBadge", choiceCard.transform);
            RectTransform cirRT = badgeCir.GetComponent<RectTransform>();
            cirRT.anchorMin = cirRT.anchorMax = new Vector2(0.09f, 0.5f);
            cirRT.sizeDelta = new Vector2(58, 58);
            AddProceduralCard(badgeCir, letterBadgeColors[i], 29f, 0f, Color.clear);
            TextMeshProUGUI letTMP = CreateText("LetterText", badgeCir.transform, choiceLetters[i], 28, Color.white, defaultFont);
            letTMP.fontStyle = FontStyles.Bold;

            // Option Text
            GameObject optTextObj = CreateUIObject("OptionText", choiceCard.transform);
            RectTransform optRT = optTextObj.GetComponent<RectTransform>();
            optRT.anchorMin = new Vector2(0.20f, 0.05f);
            optRT.anchorMax = new Vector2(0.97f, 0.95f);
            optRT.offsetMin = optRT.offsetMax = Vector2.zero;
            TextMeshProUGUI optTMP = optTextObj.AddComponent<TextMeshProUGUI>();
            if (defaultFont != null) optTMP.font = defaultFont;
            optTMP.text = $"Option {choiceLetters[i]}";
            optTMP.fontSize = 26;
            optTMP.color = new Color(0.12f, 0.15f, 0.25f);
            optTMP.alignment = TextAlignmentOptions.MidlineLeft;

            uiMgr.choiceButtonRects[i] = cardRT;
            uiMgr.choiceButtons[i] = btn;
            uiMgr.choiceCardBgs[i] = pImg;
            uiMgr.choiceLetterTexts[i] = letTMP;
            uiMgr.choiceOptionTexts[i] = optTMP;
        }

        // Mascot Companion (Bottom Right)
        GameObject mascotObj = CreateUIObject("DetectiveMascot", gamePanelRT);
        RectTransform mascotRT = mascotObj.GetComponent<RectTransform>();
        mascotRT.anchorMin = mascotRT.anchorMax = new Vector2(0.885f, 0.17f);
        mascotRT.sizeDelta = new Vector2(240, 240);
        Image mascotImg = mascotObj.AddComponent<Image>();
        mascotImg.sprite = idleSprite;
        mascotImg.preserveAspect = true;
        MascotDetective mascot = mascotObj.AddComponent<MascotDetective>();
        mascot.mascotImage = mascotImg;
        mascot.idleSprite = idleSprite;
        mascot.thinkSprite = thinkSprite;
        mascot.cheerSprite = cheerSprite;
        uiMgr.mascot = mascot;

        // Mascot Speech Bubble (Anchored above Toby with bottom pivot so it naturally expands UPWARDS)
        GameObject bubbleObj = CreateUIObject("SpeechBubble", mascotObj.transform);
        RectTransform bubbleRT = bubbleObj.GetComponent<RectTransform>();
        bubbleRT.anchorMin = bubbleRT.anchorMax = new Vector2(0.5f, 1.05f);
        bubbleRT.sizeDelta = new Vector2(360, 120);
        bubbleRT.pivot = new Vector2(0.55f, 0f);
        bubbleRT.anchoredPosition = new Vector2(-15f, 12f);
        AddProceduralCard(bubbleObj, new Color(1f, 1f, 1f, 0.99f), 22f, 3.5f, new Color(0.22f, 0.52f, 0.88f));

        bubbleObj.AddComponent<Button>();
        bubbleObj.AddComponent<UIButtonJuice>();

        // Downward Pointer Tail
        GameObject tailObj = CreateUIObject("BubbleTail", bubbleObj.transform);
        RectTransform tailRT = tailObj.GetComponent<RectTransform>();
        tailRT.anchorMin = tailRT.anchorMax = new Vector2(0.65f, 0f);
        tailRT.pivot = new Vector2(0.5f, 1f);
        tailRT.sizeDelta = new Vector2(20, 12);
        tailRT.anchoredPosition = new Vector2(0, 2);
        AddProceduralCard(tailObj, new Color(0.22f, 0.52f, 0.88f), 4f, 0f, Color.clear);

        // Header Title inside Bubble
        GameObject headerObj = CreateUIObject("BubbleHeader", bubbleObj.transform);
        RectTransform headerRT = headerObj.GetComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0.08f, 1f);
        headerRT.anchorMax = new Vector2(0.92f, 1f);
        headerRT.pivot = new Vector2(0.5f, 1f);
        headerRT.sizeDelta = new Vector2(0, 22f);
        headerRT.anchoredPosition = new Vector2(0, -10f);
        TextMeshProUGUI headerTMP = headerObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) headerTMP.font = defaultFont;
        headerTMP.text = "DETECTIVE TOBY";
        headerTMP.fontSize = 15;
        headerTMP.fontStyle = FontStyles.Bold;
        headerTMP.color = new Color(0.20f, 0.45f, 0.82f);
        headerTMP.alignment = TextAlignmentOptions.Center;

        // Dynamic Speech Text inside Bubble
        GameObject textObj = CreateUIObject("BubbleText", bubbleObj.transform);
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(20f, 14f);
        textRT.offsetMax = new Vector2(-20f, -36f);
        TextMeshProUGUI bubbleTMP = textObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) bubbleTMP.font = defaultFont;
        bubbleTMP.text = "Let's inspect the clock!";
        bubbleTMP.fontSize = 20;
        bubbleTMP.color = new Color(0.12f, 0.16f, 0.25f);
        bubbleTMP.alignment = TextAlignmentOptions.Center;
        bubbleTMP.enableWordWrapping = true;
        bubbleTMP.overflowMode = TextOverflowModes.Overflow;

        mascot.speechBubbleRoot = bubbleRT;
        mascot.speechText = bubbleTMP;
        mascot.speechHeader = headerTMP;
        mascot.speechBubbleTail = tailRT;

        // ----------------- FEEDBACK MODAL PANEL -----------------
        GameObject feedbackPanelObj = CreateUIObject("FeedbackModal", canvasObj.transform);
        RectTransform fbRT = feedbackPanelObj.GetComponent<RectTransform>();
        StretchFull(fbRT);
        Image fbDimImg = feedbackPanelObj.AddComponent<Image>();
        fbDimImg.color = new Color(0.04f, 0.08f, 0.15f, 0.65f);
        uiMgr.feedbackModalPanel = fbRT;

        GameObject fbCardObj = CreateUIObject("FeedbackCard", feedbackPanelObj.transform);
        RectTransform fbCardRT = fbCardObj.GetComponent<RectTransform>();
        fbCardRT.anchorMin = fbCardRT.anchorMax = new Vector2(0.5f, 0.5f);
        fbCardRT.sizeDelta = new Vector2(740, 390);
        AddProceduralCard(fbCardObj, Color.white, 30f, 4f, new Color(0.85f, 0.9f, 0.98f));
        uiMgr.feedbackCardRect = fbCardRT;

        // Header Banner inside Card
        GameObject fbHeaderObj = CreateUIObject("HeaderBanner", fbCardObj.transform);
        RectTransform fbHeaderRT = fbHeaderObj.GetComponent<RectTransform>();
        fbHeaderRT.anchorMin = new Vector2(0, 0.76f);
        fbHeaderRT.anchorMax = new Vector2(1, 1f);
        fbHeaderRT.offsetMin = fbHeaderRT.offsetMax = Vector2.zero;
        ProceduralImage fbHeaderBg = AddProceduralCard(fbHeaderObj, new Color(0.15f, 0.65f, 0.35f), 26f, 0f, Color.clear);
        TextMeshProUGUI fbTitleTMP = CreateText("Title", fbHeaderObj.transform, "EXCELLENT DETECTIVE WORK!", 26, Color.white, defaultFont);
        fbTitleTMP.fontStyle = FontStyles.Bold;
        RectTransform fbTitleRT = fbTitleTMP.GetComponent<RectTransform>();
        StretchFull(fbTitleRT);
        fbTitleRT.offsetMin = new Vector2(16, 0);
        fbTitleRT.offsetMax = new Vector2(-16, 0);
        fbTitleTMP.enableWordWrapping = false;
        uiMgr.feedbackTitleText = fbTitleTMP;
        uiMgr.feedbackHeaderBg = fbHeaderBg;

        // Explanation text
        GameObject fbExpObj = CreateUIObject("ExplanationText", fbCardObj.transform);
        RectTransform fbExpRT = fbExpObj.GetComponent<RectTransform>();
        fbExpRT.anchorMin = new Vector2(0.08f, 0.28f);
        fbExpRT.anchorMax = new Vector2(0.92f, 0.72f);
        fbExpRT.offsetMin = fbExpRT.offsetMax = Vector2.zero;
        TextMeshProUGUI fbExpTMP = fbExpObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) fbExpTMP.font = defaultFont;
        fbExpTMP.text = "When the long minute hand points to 12, it is an o'clock time! Since the short hour hand is at 3, it is 3:00!";
        fbExpTMP.fontSize = 24;
        fbExpTMP.color = new Color(0.18f, 0.22f, 0.32f);
        fbExpTMP.alignment = TextAlignmentOptions.Center;
        fbExpTMP.enableWordWrapping = true;
        uiMgr.feedbackExplanationText = fbExpTMP;

        // Next Button
        GameObject nextBtnObj = CreateButton("NextButton", fbCardObj.transform, new Vector2(0.5f, 0.14f), new Vector2(260, 62), 
            new Color(0.18f, 0.75f, 0.38f), "NEXT CASE >", 26, defaultFont);
        uiMgr.feedbackNextButton = nextBtnObj.GetComponent<Button>();
        feedbackPanelObj.SetActive(false);

        // ----------------- VICTORY PANEL -----------------
        GameObject victoryPanelObj = CreateUIObject("VictoryPanel", canvasObj.transform);
        RectTransform vicRT = victoryPanelObj.GetComponent<RectTransform>();
        StretchFull(vicRT);
        Image vicDimImg = victoryPanelObj.AddComponent<Image>();
        vicDimImg.color = new Color(0.04f, 0.08f, 0.15f, 0.75f);
        uiMgr.victoryPanel = vicRT;

        GameObject vicCardObj = CreateUIObject("VictoryCard", victoryPanelObj.transform);
        RectTransform vicCardRT = vicCardObj.GetComponent<RectTransform>();
        vicCardRT.anchorMin = vicCardRT.anchorMax = new Vector2(0.5f, 0.5f);
        vicCardRT.sizeDelta = new Vector2(780, 620);
        AddProceduralCard(vicCardObj, Color.white, 32f, 6f, new Color(1f, 0.85f, 0.3f));
        uiMgr.victoryCardRect = vicCardRT;

        // Badge Image
        GameObject vicBadgeObj = CreateUIObject("BadgeImage", vicCardObj.transform);
        RectTransform vicBadgeRT = vicBadgeObj.GetComponent<RectTransform>();
        vicBadgeRT.anchorMin = vicBadgeRT.anchorMax = new Vector2(0.5f, 0.74f);
        vicBadgeRT.sizeDelta = new Vector2(220, 220);
        Image vicBadgeImg = vicBadgeObj.AddComponent<Image>();
        vicBadgeImg.sprite = badgeSprite;
        vicBadgeImg.preserveAspect = true;
        uiMgr.victoryBadgeImage = vicBadgeImg;
        uiMgr.badgeGold = badgeSprite;
        uiMgr.badgeSilver = badgeSilverSprite;
        uiMgr.badgeBronze = badgeBronzeSprite;

        // Title
        GameObject vicTitleObj = CreateUIObject("TitleText", vicCardObj.transform);
        RectTransform vicTitleRT = vicTitleObj.GetComponent<RectTransform>();
        vicTitleRT.anchorMin = vicTitleRT.anchorMax = new Vector2(0.5f, 0.48f);
        vicTitleRT.sizeDelta = new Vector2(700, 60);
        TextMeshProUGUI vicTitleTMP = vicTitleObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) vicTitleTMP.font = defaultFont;
        vicTitleTMP.text = "MASTER TIME DETECTIVE!";
        vicTitleTMP.fontSize = 42;
        vicTitleTMP.fontStyle = FontStyles.Bold;
        vicTitleTMP.color = new Color(0.95f, 0.65f, 0.08f);
        vicTitleTMP.alignment = TextAlignmentOptions.Center;
        uiMgr.victoryTitleText = vicTitleTMP;

        // Score Text
        GameObject vicScoreObj = CreateUIObject("ScoreText", vicCardObj.transform);
        RectTransform vicScoreRT = vicScoreObj.GetComponent<RectTransform>();
        vicScoreRT.anchorMin = vicScoreRT.anchorMax = new Vector2(0.5f, 0.38f);
        vicScoreRT.sizeDelta = new Vector2(600, 45);
        TextMeshProUGUI vicScoreTMP = CreateText("Text", vicScoreObj.transform, "Total Score: 1000 Points", 30, new Color(0.18f, 0.22f, 0.32f), defaultFont);
        vicScoreTMP.fontStyle = FontStyles.Bold;
        uiMgr.victoryScoreText = vicScoreTMP;

        // Stars Text
        GameObject vicStarsObj = CreateUIObject("StarsText", vicCardObj.transform);
        RectTransform vicStarsRT = vicStarsObj.GetComponent<RectTransform>();
        vicStarsRT.anchorMin = vicStarsRT.anchorMax = new Vector2(0.5f, 0.29f);
        vicStarsRT.sizeDelta = new Vector2(600, 45);
        TextMeshProUGUI vicStarsTMP = CreateText("Text", vicStarsObj.transform, "Stars Earned: 30 / 30", 28, new Color(0.95f, 0.7f, 0.15f), defaultFont);
        vicStarsTMP.fontStyle = FontStyles.Bold;
        uiMgr.victoryStarsText = vicStarsTMP;

        // Buttons
        GameObject vicPlayAgainObj = CreateButton("PlayAgainBtn", vicCardObj.transform, new Vector2(0.32f, 0.12f), new Vector2(260, 68), 
            new Color(0.18f, 0.75f, 0.38f), "PLAY AGAIN", 24, defaultFont);
        uiMgr.victoryPlayAgainButton = vicPlayAgainObj.GetComponent<Button>();

        GameObject vicHomeBtnObj = CreateButton("HomeBtn", vicCardObj.transform, new Vector2(0.68f, 0.12f), new Vector2(260, 68), 
            new Color(0.2f, 0.55f, 0.9f), "MAIN MENU", 24, defaultFont);
        uiMgr.victoryHomeButton = vicHomeBtnObj.GetComponent<Button>();

        victoryPanelObj.SetActive(false);

        // Reset unlock progress so Case 1 starts as the only open case
        for (int k = 1; k < 15; k++)
        {
            PlayerPrefs.DeleteKey($"TimeDet_Unlocked_{k}");
        }
        PlayerPrefs.Save();

        // 9. Save Scenes
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/SampleScene.unity");
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/TimeDetectiveScene.unity");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Time Detective Complete Scene Built & Saved Successfully!");
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static ProceduralImage AddProceduralCard(GameObject obj, Color bgColor, float radius, float borderWidth = 0f, Color borderColor = default)
    {
        ProceduralImage pImg = obj.AddComponent<ProceduralImage>();
        pImg.color = bgColor;
        pImg.BorderWidth = 0f;
        UniformModifier mod = obj.AddComponent<UniformModifier>();
        mod.Radius = radius;
        return pImg;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize, Color color, TMP_FontAsset font)
    {
        GameObject txtObj = CreateUIObject(name, parent);
        TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
        if (font != null) tmp.font = font;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }

    private static GameObject CreateButton(string name, Transform parent, Vector2 anchorPos, Vector2 size, Color btnColor, string text, float fontSize, TMP_FontAsset font)
    {
        GameObject btnObj = CreateUIObject(name, parent);
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchorPos;
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;

        AddProceduralCard(btnObj, btnColor, 24f, 0f, Color.clear);
        btnObj.AddComponent<Button>();
        btnObj.AddComponent<UIButtonJuice>();

        TextMeshProUGUI txt = CreateText("BtnText", btnObj.transform, text, fontSize, Color.white, font);
        txt.fontStyle = FontStyles.Bold;
        RectTransform txtRT = txt.GetComponent<RectTransform>();
        StretchFull(txtRT);

        return btnObj;
    }

    private static GameObject CreateLevelButtonPrefab(Sprite starSprite, Sprite starEmptySprite, Sprite lockSprite, TMP_FontAsset font)
    {
        Directory.CreateDirectory("Assets/Prefabs");
        string prefabPath = "Assets/Prefabs/LevelButtonPrefab.prefab";

        GameObject root = new GameObject("LevelButton", typeof(RectTransform));
        RectTransform rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(240, 160);

        ProceduralImage rootPImg = AddProceduralCard(root, new Color(1f, 1f, 1f, 0.98f), 24f, 0f, Color.clear);
        Button btn = root.AddComponent<Button>();
        root.AddComponent<UIButtonJuice>();

        LevelButtonUI btnUI = root.AddComponent<LevelButtonUI>();
        btnUI.button = btn;
        btnUI.cardBg = rootPImg;
        btnUI.filledStarSprite = starSprite;
        btnUI.emptyStarSprite = starEmptySprite;

        // Top Header Badge
        GameObject badgeObj = CreateUIObject("HeaderBadge", root.transform);
        RectTransform bRT = badgeObj.GetComponent<RectTransform>();
        bRT.anchorMin = new Vector2(0.06f, 0.65f);
        bRT.anchorMax = new Vector2(0.94f, 0.94f);
        bRT.offsetMin = bRT.offsetMax = Vector2.zero;
        ProceduralImage badgePImg = AddProceduralCard(badgeObj, new Color(0.18f, 0.52f, 0.88f), 14f, 0f, Color.clear);
        btnUI.badgeBg = badgePImg;

        // Title inside badge
        GameObject titleObj = CreateUIObject("TitleText", badgeObj.transform);
        RectTransform tRT = titleObj.GetComponent<RectTransform>();
        StretchFull(tRT);
        TextMeshProUGUI tTMP = titleObj.AddComponent<TextMeshProUGUI>();
        if (font != null) tTMP.font = font;
        tTMP.text = "CASE 1";
        tTMP.fontSize = 22;
        tTMP.fontStyle = FontStyles.Bold;
        tTMP.color = Color.white;
        tTMP.alignment = TextAlignmentOptions.Center;
        btnUI.titleText = tTMP;

        // Subtitle (topic in middle)
        GameObject subObj = CreateUIObject("SubtitleText", root.transform);
        RectTransform sRT = subObj.GetComponent<RectTransform>();
        sRT.anchorMin = new Vector2(0.05f, 0.30f);
        sRT.anchorMax = new Vector2(0.95f, 0.64f);
        sRT.offsetMin = sRT.offsetMax = Vector2.zero;
        TextMeshProUGUI sTMP = subObj.AddComponent<TextMeshProUGUI>();
        if (font != null) sTMP.font = font;
        sTMP.text = "O'Clock";
        sTMP.fontSize = 20;
        sTMP.fontStyle = FontStyles.Bold;
        sTMP.color = new Color(0.12f, 0.16f, 0.25f);
        sTMP.alignment = TextAlignmentOptions.Center;
        btnUI.subtitleText = sTMP;

        // Stars Container at bottom
        btnUI.starImages = new Image[3];
        GameObject starsContainer = CreateUIObject("Stars", root.transform);
        RectTransform stRT = starsContainer.GetComponent<RectTransform>();
        stRT.anchorMin = new Vector2(0, 0.04f);
        stRT.anchorMax = new Vector2(1, 0.28f);
        stRT.offsetMin = stRT.offsetMax = Vector2.zero;

        for (int i = 0; i < 3; i++)
        {
            GameObject starObj = CreateUIObject($"Star_{i}", starsContainer.transform);
            RectTransform sImgRT = starObj.GetComponent<RectTransform>();
            sImgRT.anchorMin = sImgRT.anchorMax = new Vector2(0.26f + (i * 0.24f), 0.5f);
            sImgRT.sizeDelta = new Vector2(32, 32);
            Image sImg = starObj.AddComponent<Image>();
            sImg.sprite = starSprite;
            btnUI.starImages[i] = sImg;
        }

        // Lock Overlay (With real sprite icon, NO UNICODE CHARACTER!)
        GameObject lockObj = CreateUIObject("LockOverlay", root.transform);
        RectTransform lockRT = lockObj.GetComponent<RectTransform>();
        StretchFull(lockRT);
        AddProceduralCard(lockObj, new Color(0.08f, 0.12f, 0.20f, 0.88f), 24f, 0f, Color.clear);

        GameObject lockIconObj = CreateUIObject("LockIcon", lockObj.transform);
        RectTransform lockIconRT = lockIconObj.GetComponent<RectTransform>();
        lockIconRT.anchorMin = lockIconRT.anchorMax = new Vector2(0.5f, 0.5f);
        lockIconRT.sizeDelta = new Vector2(48, 48);
        Image lockIconImg = lockIconObj.AddComponent<Image>();
        if (lockSprite != null) lockIconImg.sprite = lockSprite;
        lockIconImg.preserveAspect = true;
        btnUI.lockIconImage = lockIconImg;

        lockObj.SetActive(false);
        btnUI.lockOverlay = lockObj;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        DestroyImmediate(root);
        return prefab;
    }
}
