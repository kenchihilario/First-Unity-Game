using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CasinoSetupWizard
{
    [MenuItem("Casino/Fix and Auto-Login Now")]
    public static void FixAndAutoLoginMenu()
    {
        PlayerPrefs.SetString("User_Kenchi", "1234");
        PlayerPrefs.SetFloat("Balance_Kenchi", 10000f);
        PlayerPrefs.SetString("LastLoggedInUser", "Kenchi");
        PlayerPrefs.Save();

        var authManagers = Object.FindObjectsByType<AuthManager>(FindObjectsInactive.Include);
        foreach (var am in authManagers)
        {
            am.AutoLogin();
        }

        var playerManagers = Object.FindObjectsByType<PlayerManager>(FindObjectsInactive.Include);
        foreach (var pm in playerManagers)
        {
            pm.balance = 10000f;
        }

        var allGOs = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
        foreach (var go in allGOs)
        {
            if (go.name == "LoggedOutGroup") go.SetActive(false);
            if (go.name == "LoggedInGroup") go.SetActive(true);
            if (go.name == "AuthOverlay") go.SetActive(false);
            if (go.name == "BankOverlay") go.SetActive(false);
            if (go.name == "AccountSettingsOverlay") go.SetActive(false);
        }

        var authUIs = Object.FindObjectsByType<AuthUIManager>(FindObjectsInactive.Include);
        foreach (var ui in authUIs)
        {
            ui.CloseAll();
            if (ui.profileText != null) ui.profileText.text = "<color=#AAAAAA>Kenchi</color>";
        }

        UpdateCurrentSceneThumbs();

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("Casino: Auto-login and thumbnails fixed! Logged in as Kenchi with $10,000 balance.");
    }

    [MenuItem("Casino/Reimport Thumbnail Images")]
    public static void ReimportThumbsMenu()
    {
        EnsureThumbsImportedAsSprites();
        Debug.Log("Casino: Thumbnail images successfully imported as Sprites!");
    }

    [MenuItem("Casino/Rebuild Credible Lobby UI")]
    public static void RebuildCredibleLobbyMenu()
    {
        EnsureThumbsImportedAsSprites();

        var scalers = Object.FindObjectsByType<CanvasScaler>(FindObjectsInactive.Include);
        foreach (var s in scalers)
        {
            s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            s.referenceResolution = new Vector2(1920, 1080);
            s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            s.matchWidthOrHeight = 0.5f;
            EditorUtility.SetDirty(s);
        }

        var allCams = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include);
        foreach (var cam in allCams)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.043f, 0.055f, 0.078f, 1f);
            EditorUtility.SetDirty(cam);
        }

        var allGOs = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
        foreach (var go in allGOs)
        {
            if (go.name == "GameHeader") { Object.DestroyImmediate(go); continue; }
            if (go.name == "BottomNavBar") { go.SetActive(false); EditorUtility.SetDirty(go); }
            if (go.name == "ProfileBtn" && go.transform.parent != null && go.transform.parent.name == "LoggedInGroup") { go.SetActive(false); EditorUtility.SetDirty(go); }
            if (go.name == "LeaderboardBtn") { go.SetActive(false); EditorUtility.SetDirty(go); }
            if (go.name == "LoggedOutGroup") { go.SetActive(true); EditorUtility.SetDirty(go); }
            if (go.name == "LoggedInGroup") { go.SetActive(false); EditorUtility.SetDirty(go); }
        }

        UpdateCurrentSceneThumbs();

        var authUIs = Object.FindObjectsByType<AuthUIManager>(FindObjectsInactive.Include);
        foreach (var aUI in authUIs)
        {
            aUI.WireAllButtons();
            aUI.CloseAll();
            EditorUtility.SetDirty(aUI);
        }

        var lobbyMgr = Object.FindAnyObjectByType<CasinoLobbyManager>();
        if (lobbyMgr != null)
        {
            lobbyMgr.SetupGameHeaderAndBack(lobbyMgr.crashPanel, "CRASH");
            lobbyMgr.SetupGameHeaderAndBack(lobbyMgr.roulettePanel, "ROULETTE");
            lobbyMgr.SetupGameHeaderAndBack(lobbyMgr.slotsPanel, "SLOTS");
            lobbyMgr.SetupGameHeaderAndBack(lobbyMgr.minesPanel, "MINES");
            lobbyMgr.SetupGameHeaderAndBack(lobbyMgr.plinkoPanel, "PLINKO");
            lobbyMgr.SetupGameHeaderAndBack(lobbyMgr.dicePanel, "DICE");
            lobbyMgr.SetupGameHeaderAndBack(lobbyMgr.blackjackPanel, "BLACKJACK");
            lobbyMgr.ShowLobby();
            lobbyMgr.StyleCredibleLobbyUI();
            lobbyMgr.AutoWireButtons();
            lobbyMgr.RefreshThumbnails();
            EditorUtility.SetDirty(lobbyMgr);
        }

        var allInputs = Object.FindObjectsByType<InputField>(FindObjectsInactive.Include);
        foreach (var inp in allInputs)
        {
            if (inp.placeholder is Text pText && pText.text.Contains("$"))
            {
                pText.text = pText.text.Replace("$", "₱");
                EditorUtility.SetDirty(pText);
            }
        }

        var allTexts = Object.FindObjectsByType<Text>(FindObjectsInactive.Include);
        foreach (var t in allTexts)
        {
            if (t.gameObject.name == "HeroTitle")
            {
                t.text = "<b>ORIGINALS & POPULAR GAMES</b>";
                t.fontSize = 25;
                EditorUtility.SetDirty(t);
            }
            else if (t.gameObject.name == "HeroSub")
            {
                t.text = "<b><color=#10B981>Philippine Peso (PHP)</color>  •  <color=#94A3B8>7 Provably Fair Games  •  Fast Gameplay</color></b>";
                t.fontSize = 15;
                EditorUtility.SetDirty(t);
            }
            else if (t.text.Contains("$0.00"))
            {
                t.text = t.text.Replace("$0.00", "₱0.00");
                EditorUtility.SetDirty(t);
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Casino: Rebuilt Clean Modern PHP Lobby UI successfully!");
    }

    [MenuItem("Casino/Build WebGL for Vercel")]
    public static void BuildWebGLForVercel()
    {
        PlayerSettings.WebGL.template = "PROJECT:FullScreen";
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.emscriptenArgs = "--bind";

        string buildDir = System.IO.Path.Combine(Application.dataPath, "..", "WebGLBuild");
        if (!System.IO.Directory.Exists(buildDir))
        {
            System.IO.Directory.CreateDirectory(buildDir);
        }

        string[] scenes = { "Assets/Scenes/SampleScene.unity" };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = buildDir;
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;

        Debug.Log("Casino: Starting WebGL build for Vercel...");
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            string vercelJson = @"{
  ""cleanUrls"": true,
  ""headers"": [
    {
      ""source"": ""/Build/(.*)"",
      ""headers"": [
        { ""key"": ""Cache-Control"", ""value"": ""public, max-age=31536000, immutable"" }
      ]
    }
  ]
}";
            System.IO.File.WriteAllText(System.IO.Path.Combine(buildDir, "vercel.json"), vercelJson);
            Debug.Log($"Casino: WebGL Build succeeded! Ready to deploy to Vercel at: {buildDir}");
            EditorUtility.RevealInFinder(buildDir);
        }
        else
        {
            Debug.LogError($"Casino: WebGL Build failed with {report.summary.totalErrors} errors.");
        }
    }

    [MenuItem("Casino/Update Thumbnails in Current Scene")]
    public static void UpdateCurrentSceneThumbs()
    {
        EnsureThumbsImportedAsSprites();
        var lobbyMgr = Object.FindAnyObjectByType<CasinoLobbyManager>();
        if (lobbyMgr != null)
        {
            lobbyMgr.StyleCredibleLobbyUI();
            lobbyMgr.AutoWireButtons();
            lobbyMgr.RefreshThumbnails();
            EditorUtility.SetDirty(lobbyMgr);
        }

        var allImages = Object.FindObjectsByType<Image>(FindObjectsInactive.Include);
        string[] games = { "CRASH", "ROULETTE", "SLOTS", "MINES", "PLINKO", "DICE", "BLACKJACK" };
        int count = 0;
        foreach (var img in allImages)
        {
            foreach (var g in games)
            {
                if (img.gameObject.name.Equals(g + "Thumb", System.StringComparison.OrdinalIgnoreCase))
                {
                    Sprite sp = LoadGameSprite(g);
                    if (sp != null)
                    {
                        img.sprite = sp;
                        img.type = Image.Type.Simple;
                        img.preserveAspect = true;
                        img.color = Color.white;
                        EditorUtility.SetDirty(img);
                        count++;
                        Debug.Log($"Casino: Updated {g} thumbnail!");
                    }
                    else
                    {
                        Debug.LogError($"Casino: FAILED to load sprite for {g}!");
                    }
                }
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"Casino: Updated {count} thumbnails in current scene with new images!");
    }

    [MenuItem("Casino/Setup Mobile WebGL Casino")]
    public static void SetupCasino()
    {
        EnsureThumbsImportedAsSprites();

        var oldManagers = Object.FindObjectsByType<CrashGameManager>(FindObjectsInactive.Include);
        foreach (var m in oldManagers) Object.DestroyImmediate(m.gameObject);
        var oldR = Object.FindObjectsByType<RouletteGameManager>(FindObjectsInactive.Include);
        foreach (var r in oldR) Object.DestroyImmediate(r.gameObject);
        var oldS = Object.FindObjectsByType<SlotsGameManager>(FindObjectsInactive.Include);
        foreach (var s in oldS) Object.DestroyImmediate(s.gameObject);
        var oldLobbies = Object.FindObjectsByType<CasinoLobbyManager>(FindObjectsInactive.Include);
        foreach (var l in oldLobbies) Object.DestroyImmediate(l.gameObject);
        var oldUIs = Object.FindObjectsByType<CrashUIManager>(FindObjectsInactive.Include);
        foreach (var u in oldUIs) Object.DestroyImmediate(u.gameObject);
        var oldRUI = Object.FindObjectsByType<RouletteUIManager>(FindObjectsInactive.Include);
        foreach (var r in oldRUI) Object.DestroyImmediate(r.gameObject);
        var oldSUI = Object.FindObjectsByType<SlotsUIManager>(FindObjectsInactive.Include);
        foreach (var s in oldSUI) Object.DestroyImmediate(s.gameObject);
        var oldMUI = Object.FindObjectsByType<MinesUIManager>(FindObjectsInactive.Include);
        foreach (var m in oldMUI) Object.DestroyImmediate(m.gameObject);
        var oldDUI = Object.FindObjectsByType<DiceUIManager>(FindObjectsInactive.Include);
        foreach (var d in oldDUI) Object.DestroyImmediate(d.gameObject);
        var oldDGM = Object.FindObjectsByType<DiceGameManager>(FindObjectsInactive.Include);
        foreach (var dg in oldDGM) Object.DestroyImmediate(dg.gameObject);
        var oldBUI = Object.FindObjectsByType<BlackjackUIManager>(FindObjectsInactive.Include);
        foreach (var b in oldBUI) Object.DestroyImmediate(b.gameObject);
        var oldBGM = Object.FindObjectsByType<BlackjackGameManager>(FindObjectsInactive.Include);
        foreach (var bg in oldBGM) Object.DestroyImmediate(bg.gameObject);
        var oldEvents = Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include);
        foreach (var e in oldEvents) Object.DestroyImmediate(e.gameObject);
        var oldCanvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include);
        foreach (var c in oldCanvases) Object.DestroyImmediate(c.gameObject);
        var oldAuth = Object.FindObjectsByType<AuthManager>(FindObjectsInactive.Include);
        foreach (var a in oldAuth) Object.DestroyImmediate(a.gameObject);
        var oldAuthUI = Object.FindObjectsByType<AuthUIManager>(FindObjectsInactive.Include);
        foreach (var a in oldAuthUI) Object.DestroyImmediate(a.gameObject);

        GameObject managers = new GameObject("CasinoManagers");
        managers.AddComponent<PlayerManager>();
        managers.AddComponent<AuthManager>();
        managers.AddComponent<CrashGameManager>();
        managers.AddComponent<RouletteGameManager>();
        managers.AddComponent<SlotsGameManager>();
        managers.AddComponent<MinesGameManager>();
        managers.AddComponent<PlinkoGameManager>();
        managers.AddComponent<DiceGameManager>();
        managers.AddComponent<BlackjackGameManager>();
        managers.AddComponent<CasinoAudio>();
        managers.AddComponent<LeaderboardManager>();
        CasinoLobbyManager lobbyManager = managers.AddComponent<CasinoLobbyManager>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        var inputModule = eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        inputModule.AssignDefaultActions();
        var inputAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>("Assets/InputSystem_Actions.inputactions");
        if (inputAsset != null) {
            inputModule.actionsAsset = inputAsset;
        }

        GameObject canvasGO = new GameObject("CasinoCanvas", typeof(RectTransform));
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (canvasGO.GetComponent<GraphicRaycaster>() == null) canvasGO.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Color bgDark = new Color(0.043f, 0.055f, 0.078f, 1f);
        Color panelColor = new Color(0.06f, 0.08f, 0.12f, 1f);
        Color accentRed = new Color(0.9f, 0.2f, 0.2f, 1f);
        Color accentGreen = new Color(0.063f, 0.725f, 0.506f, 1f);
        Color accentGray = new Color(0.137f, 0.180f, 0.259f, 1f);
        Color accentBlue = new Color(0.145f, 0.388f, 0.922f, 1f);
        Color accentYellow = new Color(0.96f, 0.62f, 0.04f, 1f);

        GameObject topBar = new GameObject("TopBar", typeof(RectTransform));
        topBar.transform.SetParent(canvasGO.transform, false);
        AddImageWithSprite(topBar, panelColor);
        RectTransform topBarRt = topBar.GetComponent<RectTransform>();
        topBarRt.anchorMin = new Vector2(0, 1); topBarRt.anchorMax = new Vector2(1, 1);
        topBarRt.pivot = new Vector2(0.5f, 1); topBarRt.sizeDelta = new Vector2(0, 75);
        topBarRt.anchoredPosition = Vector2.zero;

        Text logoText = CreateText(topBar, "Logo", "<b><size=26><color=#10B981>✦</color> <color=#FFFFFF>KENCHI</color> <color=#10B981>CASINO</color></size></b>", font, 26, new Vector2(40, 0), new Vector2(350, 75));
        logoText.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f); logoText.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
        logoText.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        logoText.alignment = TextAnchor.MiddleLeft;

        GameObject loggedOutGroup = new GameObject("LoggedOutGroup", typeof(RectTransform));
        loggedOutGroup.transform.SetParent(topBar.transform, false);
        AnchorToRight(loggedOutGroup, -40);
        loggedOutGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(320, 50);
        HorizontalLayoutGroup loHlg = loggedOutGroup.AddComponent<HorizontalLayoutGroup>();
        loHlg.childAlignment = TextAnchor.MiddleRight; loHlg.spacing = 14;
        loHlg.childControlWidth = false; loHlg.childControlHeight = false;
        loHlg.childForceExpandWidth = false; loHlg.childForceExpandHeight = false;
        loggedOutGroup.SetActive(false);

        GameObject loginBtnGo = CreateButton(loggedOutGroup, "LoginBtn", "<b>LOG IN</b>", font, Vector2.zero, new Vector2(130, 46), accentGray, 17);
        GameObject registerBtnGo = CreateButton(loggedOutGroup, "RegisterBtn", "<b>REGISTER</b>", font, Vector2.zero, new Vector2(135, 46), accentBlue, 17);

        GameObject loggedInGroup = new GameObject("LoggedInGroup", typeof(RectTransform));
        loggedInGroup.transform.SetParent(topBar.transform, false);
        AnchorToRight(loggedInGroup, -40);
        loggedInGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(680, 50);
        HorizontalLayoutGroup liHlg = loggedInGroup.AddComponent<HorizontalLayoutGroup>();
        liHlg.childAlignment = TextAnchor.MiddleRight; liHlg.spacing = 14;
        liHlg.childControlWidth = false; liHlg.childControlHeight = false;
        liHlg.childForceExpandWidth = false; liHlg.childForceExpandHeight = false;
        loggedInGroup.SetActive(true);

        GameObject balBg = new GameObject("BalanceBg", typeof(RectTransform));
        balBg.transform.SetParent(loggedInGroup.transform, false);
        AddImageWithSprite(balBg, new Color(0.075f, 0.098f, 0.153f, 1f));
        balBg.GetComponent<RectTransform>().sizeDelta = new Vector2(240, 46);
        Text globalBalanceText = CreateText(balBg, "BalanceText", "PHP <b>₱0.00</b>", font, 20, Vector2.zero, new Vector2(230, 46));
        lobbyManager.lobbyBalanceText = globalBalanceText;

        GameObject depositBtnGo = CreateButton(loggedInGroup, "DepositBtn", "<b>+ DEPOSIT</b>", font, Vector2.zero, new Vector2(125, 46), accentGreen, 17);
        GameObject withdrawBtnGo = CreateButton(loggedInGroup, "WithdrawBtn", "<b>WITHDRAW</b>", font, Vector2.zero, new Vector2(115, 46), accentGray, 16);
        GameObject logoutBtnGo = CreateButton(loggedInGroup, "LogoutBtn", "<b>✕</b>", font, Vector2.zero, new Vector2(46, 46), accentGray, 20);

        GameObject profileBtnGo = CreateButton(loggedInGroup, "ProfileBtn", "👤", font, Vector2.zero, new Vector2(46, 46), accentBlue, 20);
        profileBtnGo.SetActive(false);

        GameObject lbBtnGo = CreateButton(loggedInGroup, "LeaderboardBtn", "🏆", font, Vector2.zero, new Vector2(46, 46), accentYellow, 20);
        lbBtnGo.SetActive(false);

        GameObject bottomBar = new GameObject("BottomNavBar", typeof(RectTransform));
        bottomBar.transform.SetParent(canvasGO.transform, false);
        AddImageWithSprite(bottomBar, panelColor);
        RectTransform botRt = bottomBar.GetComponent<RectTransform>();
        botRt.anchorMin = new Vector2(0, 0); botRt.anchorMax = new Vector2(1, 0);
        botRt.pivot = new Vector2(0.5f, 0); botRt.sizeDelta = new Vector2(0, 160);
        botRt.anchoredPosition = Vector2.zero;

        HorizontalLayoutGroup navLayout = bottomBar.AddComponent<HorizontalLayoutGroup>();
        navLayout.childAlignment = TextAnchor.MiddleCenter;
        navLayout.spacing = 10; navLayout.padding = new RectOffset(20, 20, 20, 20);
        navLayout.childControlWidth = true; navLayout.childControlHeight = true;
        navLayout.childForceExpandWidth = true; navLayout.childForceExpandHeight = true;

        GameObject btnHome = CreateNavBtn(bottomBar, "Home", "🏠\n<size=20>Home</size>", font);
        GameObject btnCrash = CreateNavBtn(bottomBar, "Crash", "🚀\n<size=20>Crash</size>", font);
        GameObject btnRoulette = CreateNavBtn(bottomBar, "Roulette", "🎡\n<size=20>Roulette</size>", font);
        GameObject btnSlots = CreateNavBtn(bottomBar, "Slots", "🎰\n<size=20>Slots</size>", font);
        GameObject btnMines = CreateNavBtn(bottomBar, "Mines", "💣\n<size=20>Mines</size>", font);
        GameObject btnPlinko = CreateNavBtn(bottomBar, "Plinko", "👾\n<size=20>Plinko</size>", font);
        GameObject btnDice = CreateNavBtn(bottomBar, "Dice", "🎲\n<size=20>Dice</size>", font);
        GameObject btnBlackjack = CreateNavBtn(bottomBar, "Blackjack", "🃏\n<size=20>BJ</size>", font);
        bottomBar.SetActive(false);

        GameObject contentArea = new GameObject("ContentArea", typeof(RectTransform));
        contentArea.transform.SetParent(canvasGO.transform, false);
        RectTransform caRt = contentArea.GetComponent<RectTransform>();
        caRt.anchorMin = Vector2.zero; caRt.anchorMax = Vector2.one;
        caRt.offsetMin = new Vector2(0, 0);
        caRt.offsetMax = new Vector2(0, -75);
        caRt.localScale = Vector3.one;

        GameObject overlays = new GameObject("Overlays", typeof(RectTransform));
        overlays.transform.SetParent(canvasGO.transform, false);
        FillRect(overlays.GetComponent<RectTransform>());

        GameObject authOverlay = new GameObject("AuthOverlay", typeof(RectTransform));
        authOverlay.transform.SetParent(overlays.transform, false);
        FillRect(authOverlay.GetComponent<RectTransform>());
        AddImageWithSprite(authOverlay, new Color(0,0,0,0.8f));
        authOverlay.SetActive(false);

        GameObject loginPanel = CreatePanel(authOverlay, "LoginPanel", new Vector2(700, 800), panelColor);
        CreateText(loginPanel, "Title", "LOG IN", font, 50, new Vector2(0, 310), new Vector2(600, 80));
        InputField loginPhone = CreateInputField(loginPanel, "PhoneInput", "Phone Number", font, new Vector2(0, 180));
        InputField loginPass = CreateInputField(loginPanel, "PassInput", "Password", font, new Vector2(0, 70));
        loginPass.inputType = InputField.InputType.Password;
        Text loginErr = CreateText(loginPanel, "Error", "", font, 24, new Vector2(0, -10), new Vector2(600, 50));
        loginErr.color = accentRed;
        GameObject loginSubmit = CreateButton(loginPanel, "SubmitBtn", "Log In", font, new Vector2(0, -90), new Vector2(600, 90), accentGreen, 40);
        GameObject guestSubmit = CreateButton(loginPanel, "GuestBtn", "Play as Guest", font, new Vector2(0, -190), new Vector2(600, 80), accentBlue, 32);
        GameObject switchRegBtn = CreateButton(loginPanel, "SwitchRegBtn", "Need an account? Register", font, new Vector2(0, -280), new Vector2(600, 60), accentGray, 26);
        GameObject closeLogin = CreateButton(loginPanel, "CloseBtn", "X", font, new Vector2(300, 350), new Vector2(60, 60), accentGray, 30);

        GameObject registerPanel = CreatePanel(authOverlay, "RegisterPanel", new Vector2(700, 800), panelColor);
        CreateText(registerPanel, "Title", "REGISTER", font, 50, new Vector2(0, 310), new Vector2(600, 80));
        InputField regPhone = CreateInputField(registerPanel, "PhoneInput", "Phone Number", font, new Vector2(0, 180));
        InputField regPass = CreateInputField(registerPanel, "PassInput", "Password", font, new Vector2(0, 70));
        regPass.inputType = InputField.InputType.Password;
        Text regErr = CreateText(registerPanel, "Error", "", font, 24, new Vector2(0, -10), new Vector2(600, 50));
        regErr.color = accentRed;
        GameObject regSubmit = CreateButton(registerPanel, "SubmitBtn", "Register", font, new Vector2(0, -90), new Vector2(600, 90), accentBlue, 40);
        GameObject switchLogBtn = CreateButton(registerPanel, "SwitchLogBtn", "Already have an account? Log In", font, new Vector2(0, -190), new Vector2(600, 60), accentGray, 26);
        GameObject closeReg = CreateButton(registerPanel, "CloseBtn", "X", font, new Vector2(300, 350), new Vector2(60, 60), accentGray, 30);

        GameObject bankOverlay = new GameObject("BankOverlay", typeof(RectTransform));
        bankOverlay.transform.SetParent(overlays.transform, false);
        FillRect(bankOverlay.GetComponent<RectTransform>());
        AddImageWithSprite(bankOverlay, new Color(0,0,0,0.8f));
        bankOverlay.SetActive(false);

        GameObject depPanel = CreatePanel(bankOverlay, "DepositPanel", new Vector2(700, 500), panelColor);
        CreateText(depPanel, "Title", "DEPOSIT", font, 50, new Vector2(0, 180), new Vector2(600, 80));
        InputField depAmount = CreateInputField(depPanel, "AmountInput", "Amount (₱)", font, new Vector2(0, 50));
        depAmount.contentType = InputField.ContentType.DecimalNumber;
        Text bankErrD = CreateText(depPanel, "Error", "", font, 24, new Vector2(0, -50), new Vector2(600, 60));
        bankErrD.color = accentRed;
        GameObject depSubmit = CreateButton(depPanel, "SubmitBtn", "Deposit", font, new Vector2(0, -150), new Vector2(600, 100), accentGreen, 40);
        GameObject closeDep = CreateButton(depPanel, "CloseBtn", "X", font, new Vector2(300, 200), new Vector2(60, 60), accentGray, 30);

        GameObject withPanel = CreatePanel(bankOverlay, "WithdrawPanel", new Vector2(700, 500), panelColor);
        CreateText(withPanel, "Title", "WITHDRAW", font, 50, new Vector2(0, 180), new Vector2(600, 80));
        InputField withAmount = CreateInputField(withPanel, "AmountInput", "Amount (₱)", font, new Vector2(0, 50));
        withAmount.contentType = InputField.ContentType.DecimalNumber;
        Text bankErrW = CreateText(withPanel, "Error", "", font, 24, new Vector2(0, -50), new Vector2(600, 60));
        bankErrW.color = accentRed;
        GameObject withSubmit = CreateButton(withPanel, "SubmitBtn", "Withdraw", font, new Vector2(0, -150), new Vector2(600, 100), accentGray, 40);
        GameObject closeWith = CreateButton(withPanel, "CloseBtn", "X", font, new Vector2(300, 200), new Vector2(60, 60), accentGray, 30);

        GameObject accSettingsOverlay = new GameObject("AccountSettingsOverlay", typeof(RectTransform));
        accSettingsOverlay.transform.SetParent(overlays.transform, false); FillRect(accSettingsOverlay.GetComponent<RectTransform>());
        AddImageWithSprite(accSettingsOverlay, new Color(0,0,0,0.9f)); accSettingsOverlay.SetActive(false);
        GameObject accPanel = CreatePanel(accSettingsOverlay, "AccPanel", new Vector2(800, 500), panelColor); AnchorToCenter(accPanel);
        CreateText(accPanel, "Title", "ACCOUNT SETTINGS", font, 60, new Vector2(0, 150), new Vector2(800, 100));
        Text profTxt = CreateText(accPanel, "ProfileText", "User: Unknown", font, 40, new Vector2(0, 40), new Vector2(800, 60)); profTxt.color = accentBlue;
        Button accLogoutBtn = CreateButton(accPanel, "LogoutBtn", "LOG OUT", font, new Vector2(0, -60), new Vector2(400, 80), accentRed, 40).GetComponent<Button>();
        Button accCloseBtn = CreateButton(accPanel, "CloseBtn", "X", font, new Vector2(350, 200), new Vector2(60, 60), accentRed, 30).GetComponent<Button>();

        AuthUIManager aUI = overlays.AddComponent<AuthUIManager>();
        aUI.authOverlay = authOverlay; aUI.loginPanel = loginPanel; aUI.registerPanel = registerPanel;
        aUI.bankOverlay = bankOverlay; aUI.depositPanel = depPanel; aUI.withdrawPanel = withPanel;
        aUI.loginPhoneInput = loginPhone; aUI.loginPasswordInput = loginPass; aUI.loginErrorText = loginErr;
        aUI.registerPhoneInput = regPhone; aUI.registerPasswordInput = regPass; aUI.registerErrorText = regErr;
        aUI.depositAmountInput = depAmount; aUI.withdrawAmountInput = withAmount;
        aUI.loggedOutGroup = loggedOutGroup; aUI.loggedInGroup = loggedInGroup;
        aUI.accSettingsOverlay = accSettingsOverlay; aUI.profileText = profTxt;

        loginBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowLogin);
        registerBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowRegister);
        depositBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowDeposit);
        withdrawBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowWithdraw);
        profileBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowAccountSettings);
        logoutBtnGo.GetComponent<Button>().onClick.AddListener(aUI.OnLogoutClick);
        loginSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnLoginSubmit);
        guestSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnGuestLogin);
        switchRegBtn.GetComponent<Button>().onClick.AddListener(aUI.ShowRegister);
        switchLogBtn.GetComponent<Button>().onClick.AddListener(aUI.ShowLogin);
        regSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnRegisterSubmit);
        depSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnDepositSubmit);
        withSubmit.GetComponent<Button>().onClick.AddListener(() => { aUI.bankErrorText = bankErrW; aUI.OnWithdrawSubmit(); });
        closeLogin.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeReg.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeDep.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeWith.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        accCloseBtn.onClick.AddListener(aUI.CloseAll);
        accLogoutBtn.onClick.AddListener(() => { AuthManager.Instance.Logout(); aUI.CloseAll(); });

        GameObject lobbyPanel = new GameObject("LobbyPanel", typeof(RectTransform));
        lobbyPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(lobbyPanel, bgDark);
        FillRect(lobbyPanel.GetComponent<RectTransform>());

        GameObject heroBanner = new GameObject("HeroBanner", typeof(RectTransform));
        heroBanner.transform.SetParent(lobbyPanel.transform, false);
        AddImageWithSprite(heroBanner, new Color(0.06f, 0.08f, 0.12f, 1f));
        RectTransform hRt = heroBanner.GetComponent<RectTransform>();
        hRt.anchorMin = new Vector2(0, 1); hRt.anchorMax = new Vector2(1, 1); hRt.pivot = new Vector2(0.5f, 1);
        hRt.offsetMin = new Vector2(60, -170); hRt.offsetMax = new Vector2(-60, -90);

        Text heroTitle = CreateText(heroBanner, "HeroTitle", "<b>ORIGINALS & POPULAR GAMES</b>", font, 25, new Vector2(0, 12), new Vector2(1200, 36));
        heroTitle.color = Color.white;
        CreateText(heroBanner, "HeroSub", "<b><color=#10B981>Philippine Peso (PHP)</color>  •  <color=#94A3B8>7 Provably Fair Games  •  Fast Gameplay</color></b>", font, 15, new Vector2(0, -14), new Vector2(1200, 28));

        GameObject scrollGO = new GameObject("GamesScrollView", typeof(RectTransform));
        scrollGO.transform.SetParent(lobbyPanel.transform, false);
        RectTransform scrollRt = scrollGO.GetComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0, 0); scrollRt.anchorMax = new Vector2(1, 1);
        scrollRt.pivot = new Vector2(0.5f, 0.5f);
        scrollRt.offsetMin = new Vector2(60, 20); scrollRt.offsetMax = new Vector2(-60, -180);

        ScrollRect scrollRect = scrollGO.AddComponent<ScrollRect>();
        scrollRect.horizontal = false; scrollRect.vertical = true;

        GameObject viewport = new GameObject("Viewport", typeof(RectTransform));
        viewport.transform.SetParent(scrollGO.transform, false);
        RectTransform vRt = viewport.GetComponent<RectTransform>();
        FillRect(vRt);
        viewport.AddComponent<Image>().color = new Color(0,0,0,0.01f);
        viewport.AddComponent<Mask>().showMaskGraphic = false;

        GameObject grid = new GameObject("Content", typeof(RectTransform));
        grid.transform.SetParent(viewport.transform, false);
        RectTransform gRt = grid.GetComponent<RectTransform>();
        gRt.anchorMin = new Vector2(0, 1); gRt.anchorMax = new Vector2(1, 1); gRt.pivot = new Vector2(0.5f, 1);
        gRt.offsetMin = new Vector2(0, 0); gRt.offsetMax = new Vector2(0, 0);
        gRt.sizeDelta = new Vector2(0, 700);

        scrollRect.viewport = vRt;
        scrollRect.content = gRt;

        GridLayoutGroup layout = grid.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(380, 260);
        layout.spacing = new Vector2(24, 24);
        layout.padding = new RectOffset(20, 20, 10, 30);
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 4;
        layout.childAlignment = TextAnchor.UpperCenter;

        CreateGameThumb(grid, "CRASH", "Thumbs/crash", font, lobbyManager.StartCrash);
        CreateGameThumb(grid, "ROULETTE", "Thumbs/roulette", font, lobbyManager.StartRoulette);
        CreateGameThumb(grid, "SLOTS", "Thumbs/slots", font, lobbyManager.StartSlots);
        CreateGameThumb(grid, "MINES", "Thumbs/mines", font, lobbyManager.StartMines);
        CreateGameThumb(grid, "PLINKO", "Thumbs/plinko", font, lobbyManager.StartPlinko);
        CreateGameThumb(grid, "DICE", "Thumbs/dice", font, lobbyManager.StartDice);
        CreateGameThumb(grid, "BLACKJACK", "Thumbs/blackjack", font, lobbyManager.StartBlackjack);

        GameObject crashPanel = new GameObject("CrashPanel", typeof(RectTransform));
        crashPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(crashPanel, bgDark);
        FillRect(crashPanel.GetComponent<RectTransform>());
        CreateGameHeaderBar(crashPanel, font, "CRASH", lobbyManager.ShowLobby);
        CreateBetSidebarMobile(crashPanel, font, out InputField cBet, out Button cHalf, out Button cDouble, out Button cAction);
        GameObject graphC = new GameObject("GraphContainer", typeof(RectTransform));
        graphC.transform.SetParent(crashPanel.transform, false);
        RectTransform graphRect = graphC.GetComponent<RectTransform>();
        FillRect(graphRect); graphRect.offsetMax = new Vector2(0, -350);
        Text multText = CreateText(graphC, "MultiplierText", "1.00x", font, 180, new Vector2(0, 100), new Vector2(1000, 300));
        Text crashStatusText = CreateText(graphC, "StatusText", "WAITING...", font, 50, new Vector2(0, -100), new Vector2(1000, 100));

        CrashUIManager cUI = crashPanel.AddComponent<CrashUIManager>();
        cUI.multiplierText = multText; cUI.statusText = crashStatusText; cUI.actionButton = cAction;
        cUI.actionButtonText = cAction.transform.GetChild(0).GetComponent<Text>();
        cUI.backgroundImage = crashPanel.GetComponent<Image>(); cUI.graphContainer = graphC.transform;
        cUI.betAmountInput = cBet; cUI.halfBetBtn = cHalf; cUI.doubleBetBtn = cDouble;

        GameObject rPanel = new GameObject("RoulettePanel", typeof(RectTransform));
        rPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(rPanel, bgDark);
        FillRect(rPanel.GetComponent<RectTransform>());
        CreateGameHeaderBar(rPanel, font, "ROULETTE", lobbyManager.ShowLobby);
        CreateBetSidebarMobile(rPanel, font, out InputField rBet, out Button rHalf, out Button rDouble, out Button rAction);
        Object.DestroyImmediate(rAction.gameObject);

        GameObject rTickerBg = new GameObject("TickerBg", typeof(RectTransform));
        rTickerBg.transform.SetParent(rPanel.transform, false);
        AddImageWithSprite(rTickerBg, panelColor);
        rTickerBg.AddComponent<Mask>().showMaskGraphic = true;
        RectTransform rTickerBgRect = rTickerBg.GetComponent<RectTransform>();
        rTickerBgRect.anchorMin = new Vector2(0.5f, 0.5f); rTickerBgRect.anchorMax = new Vector2(0.5f, 0.5f);
        rTickerBgRect.pivot = new Vector2(0.5f, 0.5f);
        rTickerBgRect.anchoredPosition = new Vector2(0, 100); rTickerBgRect.sizeDelta = new Vector2(1000, 150);

        GameObject tickerContainerGO = new GameObject("TickerContainer", typeof(RectTransform));
        tickerContainerGO.transform.SetParent(rTickerBg.transform, false);
        RectTransform tickerContainerRect = tickerContainerGO.GetComponent<RectTransform>();
        FillRect(tickerContainerRect);

        GameObject centerLine = new GameObject("CenterLine", typeof(RectTransform));
        centerLine.transform.SetParent(rTickerBg.transform, false);
        AddImageWithSprite(centerLine, Color.white);
        RectTransform centerLineRect = centerLine.GetComponent<RectTransform>();
        centerLineRect.anchoredPosition = Vector2.zero; centerLineRect.sizeDelta = new Vector2(6, 180);

        Text rStatus = CreateText(rPanel, "StatusText", "ROLLING...", font, 50, new Vector2(0, 350), new Vector2(1000, 100));
        rStatus.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        rStatus.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        GameObject rBtnGroup = new GameObject("RButtonGroup", typeof(RectTransform));
        rBtnGroup.transform.SetParent(rPanel.transform, false);
        RectTransform rBtnRt = rBtnGroup.GetComponent<RectTransform>();
        rBtnRt.anchorMin = new Vector2(0,1); rBtnRt.anchorMax = new Vector2(1,1);
        rBtnRt.pivot = new Vector2(0.5f, 1); rBtnRt.offsetMin = new Vector2(0,-350); rBtnRt.offsetMax = new Vector2(0,-250);

        GameObject betRed = CreateButton(rBtnGroup, "BetRed", "<b>2X</b>\n<size=24>RED</size>", font, new Vector2(-350, 0), new Vector2(300, 90), accentRed, 35);
        AnchorToCenter(betRed);
        GameObject betGreen = CreateButton(rBtnGroup, "BetGreen", "<b>14X</b>\n<size=24>GREEN</size>", font, new Vector2(0, 0), new Vector2(300, 90), accentGreen, 35);
        AnchorToCenter(betGreen);
        GameObject betBlack = CreateButton(rBtnGroup, "BetBlack", "<b>2X</b>\n<size=24>BLACK</size>", font, new Vector2(350, 0), new Vector2(300, 90), accentGray, 35);
        AnchorToCenter(betBlack);

        RouletteUIManager rUI = rPanel.AddComponent<RouletteUIManager>();
        rUI.statusText = rStatus; rUI.tickerContainer = tickerContainerRect;
        rUI.betRedBtn = betRed.GetComponent<Button>(); rUI.betBlackBtn = betBlack.GetComponent<Button>(); rUI.betGreenBtn = betGreen.GetComponent<Button>();
        rUI.betAmountInput = rBet; rUI.halfBetBtn = rHalf; rUI.doubleBetBtn = rDouble;

        GameObject sPanel = new GameObject("SlotsPanel", typeof(RectTransform));
        sPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(sPanel, bgDark);
        FillRect(sPanel.GetComponent<RectTransform>());
        CreateGameHeaderBar(sPanel, font, "SLOTS", lobbyManager.ShowLobby);
        CreateBetSidebarMobile(sPanel, font, out InputField sBet, out Button sHalf, out Button sDouble, out Button sAction);

        Text sStatus = CreateText(sPanel, "StatusText", "READY TO SPIN!", font, 50, new Vector2(0, 400), new Vector2(1000, 100));
        sStatus.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        sStatus.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        GameObject reelBg = new GameObject("ReelsBg", typeof(RectTransform));
        reelBg.transform.SetParent(sPanel.transform, false);
        AddImageWithSprite(reelBg, panelColor);
        RectTransform reelBgRect = reelBg.GetComponent<RectTransform>();
        reelBgRect.anchorMin = new Vector2(0.5f, 0.5f); reelBgRect.anchorMax = new Vector2(0.5f, 0.5f);
        reelBgRect.pivot = new Vector2(0.5f, 0.5f); reelBgRect.anchoredPosition = new Vector2(0, 100); reelBgRect.sizeDelta = new Vector2(950, 300);

        Color innerBgColor = new Color(0.05f, 0.06f, 0.08f, 1f);
        GameObject reel1Bg = new GameObject("Reel1Bg", typeof(RectTransform));
        reel1Bg.transform.SetParent(reelBg.transform, false);
        AddImageWithSprite(reel1Bg, innerBgColor);
        reel1Bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, 0); reel1Bg.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 240);
        GameObject reel2Bg = new GameObject("Reel2Bg", typeof(RectTransform));
        reel2Bg.transform.SetParent(reelBg.transform, false);
        AddImageWithSprite(reel2Bg, innerBgColor);
        reel2Bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0); reel2Bg.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 240);
        GameObject reel3Bg = new GameObject("Reel3Bg", typeof(RectTransform));
        reel3Bg.transform.SetParent(reelBg.transform, false);
        AddImageWithSprite(reel3Bg, innerBgColor);
        reel3Bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, 0); reel3Bg.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 240);

        Text r1 = CreateText(reel1Bg, "Reel1", "<color=#FF0055>CHERRY</color>", font, 45, Vector2.zero, new Vector2(280, 240));
        Text r2 = CreateText(reel2Bg, "Reel2", "<color=#FF0055>CHERRY</color>", font, 45, Vector2.zero, new Vector2(280, 240));
        Text r3 = CreateText(reel3Bg, "Reel3", "<color=#FF0055>CHERRY</color>", font, 45, Vector2.zero, new Vector2(280, 240));

        SlotsUIManager sUI = sPanel.AddComponent<SlotsUIManager>();
        sUI.statusText = sStatus; sUI.spinButton = sAction;
        sUI.reels[0] = r1; sUI.reels[1] = r2; sUI.reels[2] = r3;
        sUI.betAmountInput = sBet; sUI.halfBetBtn = sHalf; sUI.doubleBetBtn = sDouble;

        GameObject mPanel = new GameObject("MinesPanel", typeof(RectTransform));
        mPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(mPanel, bgDark);
        FillRect(mPanel.GetComponent<RectTransform>());
        CreateGameHeaderBar(mPanel, font, "MINES", lobbyManager.ShowLobby);
        CreateBetSidebarMobile(mPanel, font, out InputField mBet, out Button mHalf, out Button mDouble, out Button mAction);

        Text minesCountLabel = CreateText(mPanel, "MinesLabel", "<color=#888888>MINES COUNT</color>", font, 24, new Vector2(0, 310), new Vector2(1000, 40));
        AnchorToCenter(minesCountLabel.gameObject);

        GameObject mCountBg = new GameObject("MinesInputBg", typeof(RectTransform));
        mCountBg.transform.SetParent(mPanel.transform, false);
        AddImageWithSprite(mCountBg, innerBgColor);
        RectTransform mCountBgRect = mCountBg.GetComponent<RectTransform>();
        AnchorToCenter(mCountBg); mCountBgRect.anchoredPosition = new Vector2(0, 250); mCountBgRect.sizeDelta = new Vector2(200, 60);

        Text mCountText = CreateText(mCountBg, "Text", "3", font, 30, Vector2.zero, new Vector2(180, 60));
        InputField mCountInput = mCountBg.AddComponent<InputField>();
        mCountInput.textComponent = mCountText; mCountInput.text = "3"; mCountInput.targetGraphic = mCountBg.GetComponent<Image>();

        Text mStatus = CreateText(mPanel, "StatusText", "READY TO PLAY", font, 50, new Vector2(0, 400), new Vector2(1000, 100));
        AnchorToCenter(mStatus.gameObject);
        Text multTextMines = CreateText(mPanel, "MultiplierText", "1.00x", font, 70, new Vector2(0, 500), new Vector2(1000, 100));
        multTextMines.color = accentGreen; AnchorToCenter(multTextMines.gameObject);

        GameObject gridContainer = new GameObject("GridContainer", typeof(RectTransform));
        gridContainer.transform.SetParent(mPanel.transform, false);
        RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
        AnchorToCenter(gridContainer); gridRect.anchoredPosition = new Vector2(0, -100); gridRect.sizeDelta = new Vector2(600, 600);

        MinesUIManager mUI = mPanel.AddComponent<MinesUIManager>();
        mUI.statusText = mStatus; mUI.currentMultiplierText = multTextMines;
        mUI.betAmountInput = mBet; mUI.minesCountInput = mCountInput; mUI.halfBetBtn = mHalf; mUI.doubleBetBtn = mDouble; mUI.actionButton = mAction;

        float tileSize = 110f; float spacing = 10f;
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                int index = row * 5 + col;
                float x = -240f + col * (tileSize + spacing);
                float y = 240f - row * (tileSize + spacing);
                GameObject tileBtn = CreateButton(gridContainer, "Tile_" + index, "", font, new Vector2(x, y), new Vector2(tileSize, tileSize), new Color(0.15f, 0.17f, 0.22f, 1f));
                AnchorToCenter(tileBtn);
                Text tTxt = tileBtn.transform.GetChild(0).GetComponent<Text>(); tTxt.fontSize = 60;
                mUI.tileButtons[index] = tileBtn.GetComponent<Button>();
            }
        }

        GameObject pPanel = new GameObject("PlinkoPanel", typeof(RectTransform));
        pPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(pPanel, bgDark);
        FillRect(pPanel.GetComponent<RectTransform>());
        CreateGameHeaderBar(pPanel, font, "PLINKO", lobbyManager.ShowLobby);
        CreateBetSidebarMobile(pPanel, font, out InputField pBet, out Button pHalf, out Button pDouble, out Button pAction);

        Text pStatus = CreateText(pPanel, "StatusText", "READY TO DROP", font, 50, new Vector2(0, 450), new Vector2(1000, 100));
        AnchorToCenter(pStatus.gameObject);
        GameObject boardContainer = new GameObject("BoardContainer", typeof(RectTransform));
        boardContainer.transform.SetParent(pPanel.transform, false);
        RectTransform boardRect = boardContainer.GetComponent<RectTransform>();
        AnchorToCenter(boardContainer); boardRect.anchoredPosition = new Vector2(0, 0); boardRect.sizeDelta = new Vector2(800, 700);

        PlinkoUIManager pUI = pPanel.AddComponent<PlinkoUIManager>();
        pUI.statusText = pStatus; pUI.betAmountInput = pBet; pUI.halfBetBtn = pHalf; pUI.doubleBetBtn = pDouble; pUI.boardContainer = boardContainer.transform; pUI.actionButton = pAction;

        float startY = 400f; float ySpacing = 85f; float xSpacing = 80f; int rows = 8;
        for (int r = 0; r < rows; r++)
        {
            int pegsInRow = r + 1; float rowWidth = (pegsInRow - 1) * xSpacing; float startX = -rowWidth / 2f;
            for (int c = 0; c < pegsInRow; c++)
            {
                GameObject peg = new GameObject($"Peg_{r}_{c}", typeof(RectTransform));
                peg.transform.SetParent(boardContainer.transform, false);
                Image pImg = peg.AddComponent<Image>(); pImg.color = Color.white;
                Sprite pSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"); if (pSprite != null) pImg.sprite = pSprite;
                RectTransform pRt = peg.GetComponent<RectTransform>();
                AnchorToCenter(peg); pRt.sizeDelta = new Vector2(24, 24); pRt.anchoredPosition = new Vector2(startX + c * xSpacing, startY - r * ySpacing);
            }
        }
        int finalSlots = rows + 1; float slotWidth = (finalSlots - 1) * xSpacing; float slotStartX = -slotWidth / 2f;
        float[] mults = new float[] { 29f, 4f, 1.5f, 0.3f, 0.2f, 0.3f, 1.5f, 4f, 29f };
        pUI.slotTexts = new Text[finalSlots];
        for (int c = 0; c < finalSlots; c++)
        {
            GameObject slotObj = new GameObject($"Slot_{c}", typeof(RectTransform));
            slotObj.transform.SetParent(boardContainer.transform, false);
            Image sImg = AddImageWithSprite(slotObj, panelColor);
            RectTransform sRt = slotObj.GetComponent<RectTransform>();
            AnchorToCenter(slotObj); sRt.sizeDelta = new Vector2(60, 45); sRt.anchoredPosition = new Vector2(slotStartX + c * xSpacing, startY - rows * ySpacing - 30f);
            Text sTxt = CreateText(slotObj, "Text", mults[c].ToString() + "x", font, 20, Vector2.zero, new Vector2(60, 45));
            sTxt.color = Color.white; pUI.slotTexts[c] = sTxt;
        }

        GameObject dPanel = new GameObject("DicePanel", typeof(RectTransform));
        dPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(dPanel, bgDark);
        FillRect(dPanel.GetComponent<RectTransform>());
        CreateGameHeaderBar(dPanel, font, "DICE", lobbyManager.ShowLobby);
        CreateBetSidebarMobile(dPanel, font, out InputField dBet, out Button dHalf, out Button dDouble, out Button dAction);

        Text dStatus = CreateText(dPanel, "StatusText", "READY TO ROLL", font, 50, new Vector2(0, 400), new Vector2(1000, 100));
        AnchorToCenter(dStatus.gameObject);
        Text dResult = CreateText(dPanel, "ResultText", "00.00", font, 120, new Vector2(0, 200), new Vector2(1000, 150));
        AnchorToCenter(dResult.gameObject);

        GameObject sliderBg = new GameObject("SliderBg", typeof(RectTransform));
        sliderBg.transform.SetParent(dPanel.transform, false);
        AddImageWithSprite(sliderBg, panelColor);
        RectTransform sBgRt = sliderBg.GetComponent<RectTransform>();
        AnchorToCenter(sliderBg); sBgRt.anchoredPosition = new Vector2(0, 0); sBgRt.sizeDelta = new Vector2(800, 40);

        GameObject sliderFillArea = new GameObject("FillArea", typeof(RectTransform));
        sliderFillArea.transform.SetParent(sliderBg.transform, false);
        RectTransform sfRt = sliderFillArea.GetComponent<RectTransform>(); FillRect(sfRt);

        GameObject sliderFill = new GameObject("Fill", typeof(RectTransform));
        sliderFill.transform.SetParent(sliderFillArea.transform, false);
        AddImageWithSprite(sliderFill, accentGreen);

        Slider slider = sliderBg.AddComponent<Slider>();
        slider.fillRect = sliderFill.GetComponent<RectTransform>();
        FillRect(slider.fillRect);
        sliderFill.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        GameObject handleArea = new GameObject("HandleArea", typeof(RectTransform));
        handleArea.transform.SetParent(sliderBg.transform, false);
        RectTransform hAreaRt = handleArea.GetComponent<RectTransform>(); FillRect(hAreaRt);

        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.transform.SetParent(handleArea.transform, false);
        AddImageWithSprite(handle, Color.white);
        RectTransform handleRt = handle.GetComponent<RectTransform>();
        handleRt.anchorMin = new Vector2(0, 0.5f); handleRt.anchorMax = new Vector2(0, 0.5f); handleRt.pivot = new Vector2(0.5f, 0.5f); handleRt.sizeDelta = new Vector2(40, 80);
        slider.handleRect = handleRt; slider.targetGraphic = handle.GetComponent<Image>();

        GameObject toggleBtn = CreateButton(dPanel, "ToggleBtn", "ROLL OVER", font, new Vector2(0, -120), new Vector2(400, 80), accentBlue, 35);
        AnchorToCenter(toggleBtn);
        Text tTarget = CreateText(dPanel, "TargetText", "Target: 50.00", font, 30, new Vector2(-250, -220), new Vector2(300, 50));
        AnchorToCenter(tTarget.gameObject);
        Text tChance = CreateText(dPanel, "ChanceText", "Chance: 50.00%", font, 30, new Vector2(0, -220), new Vector2(300, 50));
        AnchorToCenter(tChance.gameObject);
        Text tMult = CreateText(dPanel, "MultText", "Mult: 1.98x", font, 30, new Vector2(250, -220), new Vector2(300, 50));
        AnchorToCenter(tMult.gameObject);

        DiceUIManager dUI = dPanel.AddComponent<DiceUIManager>();
        dUI.statusText = dStatus; dUI.resultText = dResult; dUI.targetSlider = slider;
        dUI.toggleOverUnderBtn = toggleBtn.GetComponent<Button>(); dUI.toggleOverUnderText = toggleBtn.transform.GetChild(0).GetComponent<Text>();
        dUI.targetText = tTarget; dUI.winChanceText = tChance; dUI.multiplierText = tMult;
        dUI.betAmountInput = dBet; dUI.halfBetBtn = dHalf; dUI.doubleBetBtn = dDouble; dUI.rollBtn = dAction;

        GameObject bjPanel = new GameObject("BlackjackPanel", typeof(RectTransform));
        bjPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(bjPanel, bgDark);
        FillRect(bjPanel.GetComponent<RectTransform>());
        CreateGameHeaderBar(bjPanel, font, "BLACKJACK", lobbyManager.ShowLobby);
        CreateBetSidebarMobile(bjPanel, font, out InputField bjBet, out Button bjHalf, out Button bjDouble, out Button bjAction);

        Text bjStatus = CreateText(bjPanel, "StatusText", "PLACE YOUR BET", font, 50, new Vector2(0, 400), new Vector2(1000, 100));
        AnchorToCenter(bjStatus.gameObject);

        Text tDealer = CreateText(bjPanel, "DealerTitle", "DEALER", font, 30, new Vector2(0, 250), new Vector2(400, 50));
        AnchorToCenter(tDealer.gameObject);
        Text dScore = CreateText(bjPanel, "DealerScore", "?", font, 40, new Vector2(250, 250), new Vector2(100, 50));
        AnchorToCenter(dScore.gameObject); dScore.color = accentYellow;
        Text dCards = CreateText(bjPanel, "DealerCards", "", font, 60, new Vector2(0, 150), new Vector2(800, 100));
        AnchorToCenter(dCards.gameObject); dCards.supportRichText = true;

        Text tPlayer = CreateText(bjPanel, "PlayerTitle", "PLAYER", font, 30, new Vector2(0, -50), new Vector2(400, 50));
        AnchorToCenter(tPlayer.gameObject);
        Text pScore = CreateText(bjPanel, "PlayerScore", "0", font, 40, new Vector2(250, -50), new Vector2(100, 50));
        AnchorToCenter(pScore.gameObject); pScore.color = accentYellow;
        Text pCards = CreateText(bjPanel, "PlayerCards", "", font, 60, new Vector2(0, -150), new Vector2(800, 100));
        AnchorToCenter(pCards.gameObject); pCards.supportRichText = true;

        GameObject hitBtn = CreateButton(bjPanel, "HitBtn", "HIT", font, new Vector2(-220, -300), new Vector2(400, 100), accentGreen, 40);
        AnchorToCenter(hitBtn);
        GameObject standBtn = CreateButton(bjPanel, "StandBtn", "STAND", font, new Vector2(220, -300), new Vector2(400, 100), accentRed, 40);
        AnchorToCenter(standBtn);

        BlackjackUIManager bjUI = bjPanel.AddComponent<BlackjackUIManager>();
        bjUI.statusText = bjStatus; bjUI.dealerCardsText = dCards; bjUI.playerCardsText = pCards;
        bjUI.dealerScoreText = dScore; bjUI.playerScoreText = pScore;
        bjUI.hitBtn = hitBtn.GetComponent<Button>(); bjUI.standBtn = standBtn.GetComponent<Button>();
        bjUI.betAmountInput = bjBet; bjUI.halfBetBtn = bjHalf; bjUI.doubleBetBtn = bjDouble; bjUI.dealBtn = bjAction;

        lobbyManager.lobbyPanel = lobbyPanel;
        lobbyManager.crashPanel = crashPanel;
        lobbyManager.roulettePanel = rPanel;
        lobbyManager.slotsPanel = sPanel;
        lobbyManager.minesPanel = mPanel;
        lobbyManager.plinkoPanel = pPanel;
        lobbyManager.dicePanel = dPanel;
        lobbyManager.blackjackPanel = bjPanel;
        lobbyManager.lobbyBalanceText = globalBalanceText;

        btnHome.GetComponent<Button>().onClick.AddListener(lobbyManager.ShowLobby);
        btnCrash.GetComponent<Button>().onClick.AddListener(lobbyManager.StartCrash);
        btnRoulette.GetComponent<Button>().onClick.AddListener(lobbyManager.StartRoulette);
        btnSlots.GetComponent<Button>().onClick.AddListener(lobbyManager.StartSlots);
        btnMines.GetComponent<Button>().onClick.AddListener(lobbyManager.StartMines);
        btnPlinko.GetComponent<Button>().onClick.AddListener(lobbyManager.StartPlinko);
        btnDice.GetComponent<Button>().onClick.AddListener(lobbyManager.StartDice);
        btnBlackjack.GetComponent<Button>().onClick.AddListener(lobbyManager.StartBlackjack);

        GameObject lbPanel = new GameObject("LeaderboardPanel", typeof(RectTransform));
        lbPanel.transform.SetParent(canvasGO.transform, false);
        AddImageWithSprite(lbPanel, new Color(0f, 0f, 0f, 0.9f));
        FillRect(lbPanel.GetComponent<RectTransform>());
        lbPanel.SetActive(false);

        Text lbTitle = CreateText(lbPanel, "LBTitle", "TOP PLAYERS", font, 60, new Vector2(0, 400), new Vector2(800, 100));
        AnchorToCenter(lbTitle.gameObject);

        GameObject closeLbBtnGo = CreateButton(lbPanel, "CloseLBBtn", "X", font, new Vector2(0, -450), new Vector2(100, 100), accentRed, 40);
        AnchorToCenter(closeLbBtnGo);

        GameObject tabDaily = CreateButton(lbPanel, "TabDaily", "Daily", font, new Vector2(-220, 300), new Vector2(200, 60), new Color(0.2f, 0.2f, 0.3f), 30);
        AnchorToCenter(tabDaily);
        GameObject tabWeekly = CreateButton(lbPanel, "TabWeekly", "Weekly", font, new Vector2(0, 300), new Vector2(200, 60), new Color(0.2f, 0.2f, 0.3f), 30);
        AnchorToCenter(tabWeekly);
        GameObject tabAllTime = CreateButton(lbPanel, "TabAllTime", "All-Time", font, new Vector2(220, 300), new Vector2(200, 60), new Color(0.2f, 0.6f, 1f), 30);
        AnchorToCenter(tabAllTime);

        LeaderboardUIManager lbUI = lbPanel.AddComponent<LeaderboardUIManager>();
        lbUI.leaderboardPanel = lbPanel;
        lbUI.closeButton = closeLbBtnGo.GetComponent<Button>();
        lbUI.btnDaily = tabDaily.GetComponent<Button>();
        lbUI.btnWeekly = tabWeekly.GetComponent<Button>();
        lbUI.btnAllTime = tabAllTime.GetComponent<Button>();

        lbBtnGo.GetComponent<Button>().onClick.AddListener(() => {
            if (CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
            lbPanel.SetActive(true);
        });

        for (int i = 0; i < 10; i++)
        {
            GameObject row = new GameObject("Row_" + i, typeof(RectTransform));
            row.transform.SetParent(lbPanel.transform, false);
            Image rowBg = AddImageWithSprite(row, new Color(0.1f, 0.1f, 0.15f, 0.8f));
            RectTransform rowRt = row.GetComponent<RectTransform>();
            AnchorToCenter(row);
            rowRt.sizeDelta = new Vector2(700, 60);
            rowRt.anchoredPosition = new Vector2(0, 250 - (i * 70));

            Text rRank = CreateText(row, "Rank", "#1", font, 35, new Vector2(50, 0), new Vector2(100, 60));
            rRank.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f); rRank.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
            rRank.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f); rRank.alignment = TextAnchor.MiddleLeft;

            Text rName = CreateText(row, "Name", "Player", font, 35, new Vector2(150, 0), new Vector2(300, 60));
            rName.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f); rName.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
            rName.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f); rName.alignment = TextAnchor.MiddleLeft;

            Text rBal = CreateText(row, "Balance", "₱0.00", font, 35, new Vector2(-50, 0), new Vector2(250, 60));
            rBal.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f); rBal.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            rBal.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f); rBal.alignment = TextAnchor.MiddleRight;

            lbUI.rowBackgrounds[i] = rowBg;
            lbUI.rankTexts[i] = rRank;
            lbUI.nameTexts[i] = rName;
            lbUI.balanceTexts[i] = rBal;
        }

        SetLayerRecursively(canvasGO, LayerMask.NameToLayer("UI"));
        canvasGO.SetActive(true);
        lobbyPanel.SetActive(true);

        if(System.IO.File.Exists("Assets/Editor/CasinoFeaturesWizard.cs")) {
            System.IO.File.Delete("Assets/Editor/CasinoFeaturesWizard.cs");
            AssetDatabase.Refresh();
        }

        Debug.Log("Casino Project: Mobile WebGL UI Generation Complete!");
    }

    static void AnchorToRight(GameObject go, float xPos) {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0.5f); rt.anchorMax = new Vector2(1, 0.5f);
        rt.pivot = new Vector2(1, 0.5f); rt.anchoredPosition = new Vector2(xPos, 0);
    }
    static void AnchorToCenter(GameObject go) {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    static void CreateGameHeaderBar(GameObject parentPanel, Font font, string gameTitle, UnityEngine.Events.UnityAction onBack)
    {
        // header and back button are handled in the top bar now
    }

    static void CreateBetSidebarMobile(GameObject parentPanel, Font font, out InputField betInput, out Button halfBtn, out Button doubleBtn, out Button actionBtn)
    {
        GameObject betPanel = new GameObject("BetPanel", typeof(RectTransform));
        betPanel.transform.SetParent(parentPanel.transform, false);
        Image bg = AddImageWithSprite(betPanel, new Color(0.071f, 0.098f, 0.153f, 0.95f));
        RectTransform rt = betPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1); rt.sizeDelta = new Vector2(-60, 64); rt.anchoredPosition = new Vector2(0, -10);

        Text label = CreateText(betPanel, "BetLabel", "<b>BET AMOUNT</b>", font, 14, new Vector2(25, 0), new Vector2(130, 44));
        label.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f); label.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
        label.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f); label.alignment = TextAnchor.MiddleLeft; label.color = new Color(0.6f, 0.65f, 0.75f, 1f);

        GameObject inputBgGO = new GameObject("InputBg", typeof(RectTransform));
        inputBgGO.transform.SetParent(betPanel.transform, false);
        Image inputBg = AddImageWithSprite(inputBgGO, new Color(0.12f, 0.16f, 0.24f, 1f));
        RectTransform inRt = inputBgGO.GetComponent<RectTransform>();
        inRt.anchorMin = new Vector2(0, 0.5f); inRt.anchorMax = new Vector2(0, 0.5f); inRt.pivot = new Vector2(0, 0.5f);
        inRt.anchoredPosition = new Vector2(165, 0); inRt.sizeDelta = new Vector2(280, 44);

        GameObject textGO = new GameObject("Text", typeof(RectTransform)); textGO.transform.SetParent(inputBgGO.transform, false);
        Text text = textGO.AddComponent<Text>(); text.font = font; text.fontSize = 18; text.color = Color.white; text.alignment = TextAnchor.MiddleLeft;
        RectTransform textRect = textGO.GetComponent<RectTransform>(); textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(14, 0); textRect.offsetMax = new Vector2(-120, 0);

        betInput = inputBgGO.AddComponent<InputField>();
        betInput.textComponent = text; betInput.text = "10.00"; betInput.contentType = InputField.ContentType.DecimalNumber;

        GameObject halfGO = CreateButton(inputBgGO, "HalfBtn", "1/2", font, new Vector2(-60, 0), new Vector2(48, 34), new Color(0.2f, 0.25f, 0.35f, 1f), 15);
        AnchorToRight(halfGO, -60); halfBtn = halfGO.GetComponent<Button>();
        GameObject doubleGO = CreateButton(inputBgGO, "DoubleBtn", "2x", font, new Vector2(-10, 0), new Vector2(48, 34), new Color(0.2f, 0.25f, 0.35f, 1f), 15);
        AnchorToRight(doubleGO, -10); doubleBtn = doubleGO.GetComponent<Button>();

        GameObject actionGO = CreateButton(betPanel, "ActionBtn", "BET ₱10.00", font, new Vector2(-20, 0), new Vector2(240, 46), new Color(0.063f, 0.725f, 0.506f, 1f), 18);
        actionGO.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f); actionGO.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        actionGO.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        actionBtn = actionGO.GetComponent<Button>();
    }

    static GameObject CreateNavBtn(GameObject parent, string name, string textStr, Font font) {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        Image img = AddImageWithSprite(go, new Color(0.17f, 0.19f, 0.25f, 1f));
        Button btn = go.AddComponent<Button>();
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, 120);
        Text text = CreateText(go, "Text", textStr, font, 30, Vector2.zero, new Vector2(160, 120));
        return go;
    }

    public static Sprite LoadGameSprite(string gameName)
    {
        string lower = gameName.ToLower();
        string[] exts = { ".jpg", ".png" };
        foreach (var ext in exts)
        {
            string path = $"Assets/Resources/Thumbs/{lower}{ext}";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            Sprite direct = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (direct != null) return direct;

            var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            if (subAssets != null)
            {
                foreach (var a in subAssets)
                {
                    if (a is Sprite sp) return sp;
                }
            }

            string fullPath = System.IO.Path.Combine(Application.dataPath, "Resources", "Thumbs", $"{lower}{ext}");
            if (System.IO.File.Exists(fullPath))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
                Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
                if (tex.LoadImage(bytes))
                {
                    tex.wrapMode = TextureWrapMode.Clamp;
                    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
        }

        Sprite res = Resources.Load<Sprite>($"Thumbs/{lower}");
        if (res != null) return res;

        Texture2D resTex = Resources.Load<Texture2D>($"Thumbs/{lower}");
        if (resTex != null)
        {
            return Sprite.Create(resTex, new Rect(0, 0, resTex.width, resTex.height), new Vector2(0.5f, 0.5f));
        }

        return null;
    }

    static void EnsureThumbsImportedAsSprites()
    {
        string[] games = { "crash", "roulette", "slots", "mines", "plinko", "dice", "blackjack" };
        string[] exts = { ".jpg", ".png" };
        foreach (var g in games)
        {
            foreach (var ext in exts)
            {
                string path = $"Assets/Resources/Thumbs/{g}{ext}";
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    bool changed = false;
                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        changed = true;
                    }
                    if (importer.spritePivot != new Vector2(0.5f, 0.5f))
                    {
                        importer.spritePivot = new Vector2(0.5f, 0.5f);
                        changed = true;
                    }
                    if (importer.npotScale != TextureImporterNPOTScale.None)
                    {
                        importer.npotScale = TextureImporterNPOTScale.None;
                        changed = true;
                    }
                    if (importer.wrapMode != TextureWrapMode.Clamp)
                    {
                        importer.wrapMode = TextureWrapMode.Clamp;
                        changed = true;
                    }
                    if (!importer.alphaIsTransparency)
                    {
                        importer.alphaIsTransparency = true;
                        changed = true;
                    }
                    if (changed)
                    {
                        importer.SaveAndReimport();
                    }
                }
            }
        }
        AssetDatabase.Refresh();
    }

    static GameObject CreateGameThumb(GameObject parent, string name, string resourcePath, Font font, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGo = new GameObject(name + "Thumb", typeof(RectTransform));
        btnGo.transform.SetParent(parent.transform, false);
        Image img = btnGo.AddComponent<Image>();

        Sprite sp = LoadGameSprite(name);

        if (sp != null) {
            img.sprite = sp;
            img.type = Image.Type.Simple;
            img.color = Color.white;
            img.preserveAspect = false;
        } else {
            img.color = new Color(0.086f, 0.11f, 0.17f);
        }
        img.raycastTarget = true;

        Button btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        var juice = btnGo.AddComponent<ButtonHoverJuice>();
        juice.hoverScale = 1.04f;
        juice.clickScale = 0.96f;
        juice.speed = 18f;

        GameObject nameBg = new GameObject("NameBg", typeof(RectTransform));
        nameBg.transform.SetParent(btnGo.transform, false);
        Image bgImg = nameBg.AddComponent<Image>();
        bgImg.color = new Color(0.051f, 0.075f, 0.122f, 0.95f);
        bgImg.raycastTarget = false;

        RectTransform nbRt = nameBg.GetComponent<RectTransform>();
        nbRt.anchorMin = new Vector2(0, 0);
        nbRt.anchorMax = new Vector2(1, 0.24f);
        nbRt.pivot = new Vector2(0.5f, 0);
        nbRt.offsetMin = Vector2.zero;
        nbRt.offsetMax = Vector2.zero;

        string subText = name switch {
            "CRASH" => "<color=#10B981>99.0% RTP</color> • Multiplier",
            "ROULETTE" => "<color=#EF4444>CLASSIC</color> • European",
            "SLOTS" => "<color=#A855F7>JACKPOT</color> • 3-Reel",
            "MINES" => "<color=#10B981>CUSTOM RISK</color> • 99% RTP",
            "PLINKO" => "<color=#6366F1>1000X MAX</color> • Arcade",
            "DICE" => "<color=#F59E0B>PROVABLY FAIR</color> • 99% RTP",
            "BLACKJACK" => "<color=#059669>3:2 PAYOUT</color> • Table",
            _ => "Kenchi Originals"
        };

        Text t2 = CreateText(nameBg, "Name", $"<b><size=28>{name}</size></b>\n<size=18>{subText}</size>", font, 24, Vector2.zero, Vector2.zero);
        t2.alignment = TextAnchor.MiddleCenter;
        t2.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        t2.GetComponent<RectTransform>().anchorMax = Vector2.one;
        t2.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        t2.raycastTarget = false;
        return btnGo;
    }

    static GameObject CreatePanel(GameObject parent, string name, Vector2 size, Color color) {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        Image img = AddImageWithSprite(go, color);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = size; rect.anchoredPosition = Vector2.zero;
        return go;
    }

    static InputField CreateInputField(GameObject parent, string name, string placeholder, Font font, Vector2 pos) {
        GameObject inputBgGO = new GameObject(name, typeof(RectTransform));
        inputBgGO.transform.SetParent(parent.transform, false);
        Image inputBg = AddImageWithSprite(inputBgGO, new Color(0.08f, 0.09f, 0.12f, 1f));
        RectTransform inRt = inputBgGO.GetComponent<RectTransform>();
        inRt.anchoredPosition = pos; inRt.sizeDelta = new Vector2(600, 90);

        GameObject textGO = new GameObject("Text", typeof(RectTransform)); textGO.transform.SetParent(inputBgGO.transform, false);
        Text text = textGO.AddComponent<Text>(); text.font = font; text.fontSize = 35; text.color = Color.white; text.alignment = TextAnchor.MiddleLeft;
        RectTransform textRect = textGO.GetComponent<RectTransform>(); textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(20, 0); textRect.offsetMax = new Vector2(-20, 0);

        GameObject pTextGO = new GameObject("Placeholder", typeof(RectTransform)); pTextGO.transform.SetParent(inputBgGO.transform, false);
        Text pText = pTextGO.AddComponent<Text>(); pText.font = font; pText.fontSize = 35; pText.color = new Color(0.5f,0.5f,0.5f); pText.alignment = TextAnchor.MiddleLeft;
        pText.text = placeholder;
        RectTransform pRect = pTextGO.GetComponent<RectTransform>(); pRect.anchorMin = Vector2.zero; pRect.anchorMax = Vector2.one;
        pRect.offsetMin = new Vector2(20, 0); pRect.offsetMax = new Vector2(-20, 0);

        InputField inputField = inputBgGO.AddComponent<InputField>();
        inputField.targetGraphic = inputBg;
        inputField.textComponent = text; inputField.placeholder = pText; return inputField;
    }

    static void FillRect(RectTransform rect) {
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one; rect.anchoredPosition = Vector2.zero;
    }

    static Text CreateText(GameObject parent, string name, string textStr, Font font, int size, Vector2 pos, Vector2 sizeDelta) {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        Text text = go.AddComponent<Text>(); text.text = textStr; text.fontSize = size; text.fontStyle = FontStyle.Bold; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.font = font;
        text.horizontalOverflow = HorizontalWrapMode.Overflow; text.verticalOverflow = VerticalWrapMode.Overflow; text.supportRichText = true;
        Shadow shadow = go.AddComponent<Shadow>(); shadow.effectColor = new Color(0,0,0,0.5f); shadow.effectDistance = new Vector2(2, -2);
        RectTransform rect = go.GetComponent<RectTransform>(); rect.anchoredPosition = pos; rect.sizeDelta = sizeDelta; rect.localScale = Vector3.one;
        return text;
    }

    static GameObject CreateButton(GameObject parent, string name, string textStr, Font font, Vector2 pos, Vector2 size, Color color, int fontSize = 30) {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        Image img = AddImageWithSprite(go, color); Button btn = go.AddComponent<Button>();
        RectTransform rect = go.GetComponent<RectTransform>(); rect.anchoredPosition = pos; rect.sizeDelta = size; rect.localScale = Vector3.one;
        Text text = CreateText(go, "Text", textStr, font, fontSize, Vector2.zero, size);
        return go;
    }

    static Image AddImageWithSprite(GameObject go, Color color) {
        Image img = go.AddComponent<Image>(); img.color = color;
        Sprite bgSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        if (bgSprite != null) { img.sprite = bgSprite; img.type = Image.Type.Sliced; }
        return img;
    }

    static void SetLayerRecursively(GameObject go, int layer) {
        go.layer = layer; foreach (Transform child in go.transform) SetLayerRecursively(child.gameObject, layer);
    }
}
