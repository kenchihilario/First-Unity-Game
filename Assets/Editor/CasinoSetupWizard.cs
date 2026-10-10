using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CasinoSetupWizard
{
    [MenuItem("Casino/Setup Mobile WebGL Casino")]
    public static void SetupCasino()
    {
        // I added this to clean up the old managers from the scene so we don't have duplicates when running this script again.
        var oldManagers = Object.FindObjectsByType<CrashGameManager>(FindObjectsSortMode.None);
        foreach (var m in oldManagers) Object.DestroyImmediate(m.gameObject);
        var oldR = Object.FindObjectsByType<RouletteGameManager>(FindObjectsSortMode.None);
        foreach (var r in oldR) Object.DestroyImmediate(r.gameObject);
        var oldS = Object.FindObjectsByType<SlotsGameManager>(FindObjectsSortMode.None);
        foreach (var s in oldS) Object.DestroyImmediate(s.gameObject);
        var oldLobbies = Object.FindObjectsByType<CasinoLobbyManager>(FindObjectsSortMode.None);
        foreach (var l in oldLobbies) Object.DestroyImmediate(l.gameObject);
        var oldUIs = Object.FindObjectsByType<CrashUIManager>(FindObjectsSortMode.None);
        foreach (var u in oldUIs) Object.DestroyImmediate(u.gameObject);
        var oldRUI = Object.FindObjectsByType<RouletteUIManager>(FindObjectsSortMode.None);
        foreach (var r in oldRUI) Object.DestroyImmediate(r.gameObject);
        var oldSUI = Object.FindObjectsByType<SlotsUIManager>(FindObjectsSortMode.None);
        foreach (var s in oldSUI) Object.DestroyImmediate(s.gameObject);
        var oldMUI = Object.FindObjectsByType<MinesUIManager>(FindObjectsSortMode.None);
        foreach (var m in oldMUI) Object.DestroyImmediate(m.gameObject);
        var oldEvents = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (var e in oldEvents) Object.DestroyImmediate(e.gameObject);
        var oldCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in oldCanvases) Object.DestroyImmediate(c.gameObject);
        var oldAuth = Object.FindObjectsByType<AuthManager>(FindObjectsSortMode.None);
        foreach (var a in oldAuth) Object.DestroyImmediate(a.gameObject);
        var oldAuthUI = Object.FindObjectsByType<AuthUIManager>(FindObjectsSortMode.None);
        foreach (var a in oldAuthUI) Object.DestroyImmediate(a.gameObject);

        // We need to create the main casino managers and attach them to a GameObject so our games work.
        GameObject managers = new GameObject("CasinoManagers");
        managers.AddComponent<PlayerManager>();
        managers.AddComponent<AuthManager>();
        managers.AddComponent<CrashGameManager>();
        managers.AddComponent<RouletteGameManager>();
        managers.AddComponent<SlotsGameManager>();
        managers.AddComponent<MinesGameManager>();
        managers.AddComponent<PlinkoGameManager>();
        CasinoLobbyManager lobbyManager = managers.AddComponent<CasinoLobbyManager>();

        // This creates the EventSystem which is absolutely required for Unity UI buttons to be clickable.
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // This creates the main Canvas for the UI. I set it to scale with the screen size so it looks good on mobile phones.
        GameObject canvasGO = new GameObject("CasinoCanvas", typeof(RectTransform));
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 5f;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920); // I set this to mobile portrait mode as the default layout.
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f; // Setting this to 1 matches the height perfectly, which guarantees it won't stretch weirdly on different phones or desktop!
        
        if (canvasGO.GetComponent<GraphicRaycaster>() == null) canvasGO.AddComponent<GraphicRaycaster>();
        
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Color bgDark = new Color(0.05f, 0.06f, 0.08f, 1f);       
        Color panelColor = new Color(0.11f, 0.13f, 0.17f, 1f);   
        Color accentRed = new Color(1f, 0.13f, 0.33f, 1f);       
        Color accentGreen = new Color(0f, 0.9f, 0.0f, 1f);       
        Color accentGray = new Color(0.17f, 0.19f, 0.25f, 1f);   
        Color accentBlue = new Color(0.13f, 0.67f, 1f, 1f);

        // This section creates the Top Bar UI which holds the logo, user balance, and account buttons.
        GameObject topBar = new GameObject("TopBar", typeof(RectTransform));
        topBar.transform.SetParent(canvasGO.transform, false);
        AddImageWithSprite(topBar, panelColor); 
        RectTransform topBarRt = topBar.GetComponent<RectTransform>();
        topBarRt.anchorMin = new Vector2(0, 1); topBarRt.anchorMax = new Vector2(1, 1);
        topBarRt.pivot = new Vector2(0.5f, 1); topBarRt.sizeDelta = new Vector2(0, 140);
        topBarRt.anchoredPosition = Vector2.zero;

        Text logoText = CreateText(topBar, "Logo", "<b><color=#FF7700>●</color> KENCHI</b>", font, 50, new Vector2(20, 0), new Vector2(400, 140));
        logoText.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f); logoText.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
        logoText.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        logoText.alignment = TextAnchor.MiddleLeft;

        // This group holds the Login and Register buttons for when the user is not logged in yet.
        GameObject loggedOutGroup = new GameObject("LoggedOutGroup", typeof(RectTransform));
        loggedOutGroup.transform.SetParent(topBar.transform, false);
        AnchorToRight(loggedOutGroup, -20);
        
        GameObject loginBtnGo = CreateButton(loggedOutGroup, "LoginBtn", "Log In", font, Vector2.zero, new Vector2(200, 80), accentGray, 30);
        AnchorToRight(loginBtnGo, -220);
        GameObject registerBtnGo = CreateButton(loggedOutGroup, "RegisterBtn", "Register", font, Vector2.zero, new Vector2(200, 80), accentBlue, 30);
        AnchorToRight(registerBtnGo, 0);

        // This group holds the Deposit, Withdraw, Profile, and Logout buttons for when the user has logged into their account.
        GameObject loggedInGroup = new GameObject("LoggedInGroup", typeof(RectTransform));
        loggedInGroup.transform.SetParent(topBar.transform, false);
        AnchorToRight(loggedInGroup, -20);
        
        GameObject logoutBtnGo = CreateButton(loggedInGroup, "LogoutBtn", "X", font, Vector2.zero, new Vector2(80, 80), accentRed, 30);
        AnchorToRight(logoutBtnGo, 0);
        
        GameObject withdrawBtnGo = CreateButton(loggedInGroup, "WithdrawBtn", "W", font, Vector2.zero, new Vector2(80, 80), accentGray, 30);
        AnchorToRight(withdrawBtnGo, -100);
        
        GameObject depositBtnGo = CreateButton(loggedInGroup, "DepositBtn", "Deposit", font, Vector2.zero, new Vector2(200, 80), accentGreen, 30);
        AnchorToRight(depositBtnGo, -200);

        // I made a profile button here so users can click it to open their account settings popup.
        GameObject profileBtnGo = CreateButton(loggedInGroup, "ProfileBtn", "👤", font, Vector2.zero, new Vector2(80, 80), accentBlue, 40);
        AnchorToRight(profileBtnGo, -420);

        GameObject balBg = new GameObject("BalanceBg", typeof(RectTransform));
        balBg.transform.SetParent(loggedInGroup.transform, false);
        AddImageWithSprite(balBg, bgDark);
        AnchorToRight(balBg, -520);
        balBg.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 80);
        Text globalBalanceText = CreateText(balBg, "BalanceText", "$0.00", font, 35, Vector2.zero, new Vector2(230, 80));
        lobbyManager.lobbyBalanceText = globalBalanceText;

        // Here I create the bottom navigation bar with icons for all the games so you can easily switch between them without going back to the lobby.
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
        navLayout.childControlWidth = false; navLayout.childControlHeight = true;
        navLayout.childForceExpandWidth = false; navLayout.childForceExpandHeight = true;

        GameObject btnHome = CreateNavBtn(bottomBar, "Home", "🏠\n<size=20>Home</size>", font);
        GameObject btnCrash = CreateNavBtn(bottomBar, "Crash", "🚀\n<size=20>Crash</size>", font);
        GameObject btnRoulette = CreateNavBtn(bottomBar, "Roulette", "🎡\n<size=20>Roulette</size>", font);
        GameObject btnSlots = CreateNavBtn(bottomBar, "Slots", "🎰\n<size=20>Slots</size>", font);
        GameObject btnMines = CreateNavBtn(bottomBar, "Mines", "💣\n<size=20>Mines</size>", font);
        GameObject btnPlinko = CreateNavBtn(bottomBar, "Plinko", "👾\n<size=20>Plinko</size>", font);

        // The content area is where the games and lobby will actually be displayed. It sits between the top and bottom bars.
        GameObject contentArea = new GameObject("ContentArea", typeof(RectTransform));
        contentArea.transform.SetParent(canvasGO.transform, false);
        RectTransform caRt = contentArea.GetComponent<RectTransform>();
        caRt.anchorMin = Vector2.zero; caRt.anchorMax = Vector2.one;
        caRt.offsetMin = new Vector2(0, 160); 
        caRt.offsetMax = new Vector2(0, -140); 
        caRt.localScale = Vector3.one;

        // These are the popup windows (overlays) for logging in, registering, depositing money, and withdrawing money.
        GameObject overlays = new GameObject("Overlays", typeof(RectTransform));
        overlays.transform.SetParent(canvasGO.transform, false);
        FillRect(overlays.GetComponent<RectTransform>());
        
        GameObject authOverlay = new GameObject("AuthOverlay", typeof(RectTransform));
        authOverlay.transform.SetParent(overlays.transform, false);
        FillRect(authOverlay.GetComponent<RectTransform>());
        AddImageWithSprite(authOverlay, new Color(0,0,0,0.8f));
        
        GameObject loginPanel = CreatePanel(authOverlay, "LoginPanel", new Vector2(700, 700), panelColor);
        CreateText(loginPanel, "Title", "LOG IN", font, 50, new Vector2(0, 250), new Vector2(600, 80));
        InputField loginPhone = CreateInputField(loginPanel, "PhoneInput", "Phone Number", font, new Vector2(0, 100));
        InputField loginPass = CreateInputField(loginPanel, "PassInput", "Password", font, new Vector2(0, -20));
        loginPass.inputType = InputField.InputType.Password;
        Text loginErr = CreateText(loginPanel, "Error", "", font, 24, new Vector2(0, -120), new Vector2(600, 60));
        loginErr.color = accentRed;
        GameObject loginSubmit = CreateButton(loginPanel, "SubmitBtn", "Log In", font, new Vector2(0, -220), new Vector2(600, 100), accentGreen, 40);
        GameObject closeLogin = CreateButton(loginPanel, "CloseBtn", "X", font, new Vector2(300, 300), new Vector2(60, 60), accentGray, 30);

        GameObject registerPanel = CreatePanel(authOverlay, "RegisterPanel", new Vector2(700, 700), panelColor);
        CreateText(registerPanel, "Title", "REGISTER", font, 50, new Vector2(0, 250), new Vector2(600, 80));
        InputField regPhone = CreateInputField(registerPanel, "PhoneInput", "Phone Number", font, new Vector2(0, 100));
        InputField regPass = CreateInputField(registerPanel, "PassInput", "Password", font, new Vector2(0, -20));
        regPass.inputType = InputField.InputType.Password;
        Text regErr = CreateText(registerPanel, "Error", "", font, 24, new Vector2(0, -120), new Vector2(600, 60));
        regErr.color = accentRed;
        GameObject regSubmit = CreateButton(registerPanel, "SubmitBtn", "Register", font, new Vector2(0, -220), new Vector2(600, 100), accentBlue, 40);
        GameObject closeReg = CreateButton(registerPanel, "CloseBtn", "X", font, new Vector2(300, 300), new Vector2(60, 60), accentGray, 30);

        GameObject bankOverlay = new GameObject("BankOverlay", typeof(RectTransform));
        bankOverlay.transform.SetParent(overlays.transform, false);
        FillRect(bankOverlay.GetComponent<RectTransform>());
        AddImageWithSprite(bankOverlay, new Color(0,0,0,0.8f));

        GameObject depPanel = CreatePanel(bankOverlay, "DepositPanel", new Vector2(700, 500), panelColor);
        CreateText(depPanel, "Title", "DEPOSIT", font, 50, new Vector2(0, 180), new Vector2(600, 80));
        InputField depAmount = CreateInputField(depPanel, "AmountInput", "Amount ($)", font, new Vector2(0, 50));
        depAmount.contentType = InputField.ContentType.DecimalNumber;
        Text bankErrD = CreateText(depPanel, "Error", "", font, 24, new Vector2(0, -50), new Vector2(600, 60));
        bankErrD.color = accentRed;
        GameObject depSubmit = CreateButton(depPanel, "SubmitBtn", "Deposit", font, new Vector2(0, -150), new Vector2(600, 100), accentGreen, 40);
        GameObject closeDep = CreateButton(depPanel, "CloseBtn", "X", font, new Vector2(300, 200), new Vector2(60, 60), accentGray, 30);

        GameObject withPanel = CreatePanel(bankOverlay, "WithdrawPanel", new Vector2(700, 500), panelColor);
        CreateText(withPanel, "Title", "WITHDRAW", font, 50, new Vector2(0, 180), new Vector2(600, 80));
        InputField withAmount = CreateInputField(withPanel, "AmountInput", "Amount ($)", font, new Vector2(0, 50));
        withAmount.contentType = InputField.ContentType.DecimalNumber;
        Text bankErrW = CreateText(withPanel, "Error", "", font, 24, new Vector2(0, -50), new Vector2(600, 60));
        bankErrW.color = accentRed;
        GameObject withSubmit = CreateButton(withPanel, "SubmitBtn", "Withdraw", font, new Vector2(0, -150), new Vector2(600, 100), accentGray, 40);
        GameObject closeWith = CreateButton(withPanel, "CloseBtn", "X", font, new Vector2(300, 200), new Vector2(60, 60), accentGray, 30);

        // This is the overlay that shows up when you click the profile button. It shows your username and lets you securely log out.
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
        regSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnRegisterSubmit);
        depSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnDepositSubmit);
        withSubmit.GetComponent<Button>().onClick.AddListener(() => { aUI.bankErrorText = bankErrW; aUI.OnWithdrawSubmit(); });
        closeLogin.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeReg.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeDep.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeWith.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        accCloseBtn.onClick.AddListener(aUI.CloseAll);
        accLogoutBtn.onClick.AddListener(() => { AuthManager.Instance.Logout(); aUI.CloseAll(); });


        // The main lobby screen that shows the hero banner and the grid of games.
        GameObject lobbyPanel = new GameObject("LobbyPanel", typeof(RectTransform));
        lobbyPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(lobbyPanel, bgDark);
        FillRect(lobbyPanel.GetComponent<RectTransform>());
        
        GameObject heroBanner = new GameObject("HeroBanner", typeof(RectTransform));
        heroBanner.transform.SetParent(lobbyPanel.transform, false);
        AddImageWithSprite(heroBanner, new Color(0.05f, 0.06f, 0.08f, 1f));
        RectTransform hRt = heroBanner.GetComponent<RectTransform>();
        hRt.anchorMin = new Vector2(0, 1); hRt.anchorMax = new Vector2(1, 1); hRt.pivot = new Vector2(0.5f, 1);
        hRt.offsetMin = new Vector2(20, -220); hRt.offsetMax = new Vector2(-20, -20);
        
        Text heroTitle = CreateText(heroBanner, "HeroTitle", "<b>KENCHI</b> CASINO", font, 60, new Vector2(0, 30), new Vector2(1000, 100));
        heroTitle.color = accentBlue;
        CreateText(heroBanner, "HeroSub", "Provably Fair. Instant Crypto. Top Games.", font, 28, new Vector2(0, -40), new Vector2(1000, 50));
        
        // I added a ScrollRect here so that if we add more games later, the player can scroll down to see them all.
        GameObject scrollGO = new GameObject("GamesScrollView", typeof(RectTransform));
        scrollGO.transform.SetParent(lobbyPanel.transform, false);
        RectTransform scrollRt = scrollGO.GetComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0, 0); scrollRt.anchorMax = new Vector2(1, 1);
        scrollRt.pivot = new Vector2(0.5f, 0.5f);
        scrollRt.offsetMin = new Vector2(0, 0); scrollRt.offsetMax = new Vector2(0, -240); // space for smaller hero banner
        
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
        gRt.sizeDelta = new Vector2(0, 1500); // Allow scrolling down!

        scrollRect.viewport = vRt;
        scrollRect.content = gRt;

        GridLayoutGroup layout = grid.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(400, 400); // Big square icons
        layout.spacing = new Vector2(40, 40);
        layout.padding = new RectOffset(40, 40, 40, 40);
        layout.childAlignment = TextAnchor.UpperCenter;

        CreateGameThumb(grid, "CRASH", "Thumbs/crash", font, lobbyManager.StartCrash);
        CreateGameThumb(grid, "ROULETTE", "Thumbs/roulette", font, lobbyManager.StartRoulette);
        CreateGameThumb(grid, "SLOTS", "Thumbs/slots", font, lobbyManager.StartSlots);
        CreateGameThumb(grid, "MINES", "Thumbs/mines", font, lobbyManager.StartMines);
        CreateGameThumb(grid, "PLINKO", "Thumbs/plinko", font, lobbyManager.StartPlinko);

        // Now we create the individual game panels (Crash, Roulette, Slots, Mines, Plinko). They are hidden by default.
        // Building the Crash game UI with the graph container and multiplier text.
        GameObject crashPanel = new GameObject("CrashPanel", typeof(RectTransform));
        crashPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(crashPanel, bgDark);
        FillRect(crashPanel.GetComponent<RectTransform>());
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

        // Building the Roulette game UI. I added a ticker background where the colors will spin.
        GameObject rPanel = new GameObject("RoulettePanel", typeof(RectTransform));
        rPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(rPanel, bgDark);
        FillRect(rPanel.GetComponent<RectTransform>());
        CreateBetSidebarMobile(rPanel, font, out InputField rBet, out Button rHalf, out Button rDouble, out Button rAction);
        Object.DestroyImmediate(rAction.gameObject); // I am deleting the default action button because Roulette needs 3 separate betting buttons (Red, Green, Black).
        
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

        // Building the Slots game UI. There are 3 reels that will show emojis when you spin.
        GameObject sPanel = new GameObject("SlotsPanel", typeof(RectTransform));
        sPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(sPanel, bgDark);
        FillRect(sPanel.GetComponent<RectTransform>());
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

        // Building the Mines game UI. I generated a 5x5 grid of buttons using a loop for the tiles.
        GameObject mPanel = new GameObject("MinesPanel", typeof(RectTransform));
        mPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(mPanel, bgDark);
        FillRect(mPanel.GetComponent<RectTransform>());
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

        // Building the Plinko game board. I used a nested loop to create all the pegs in a triangle shape so the ball bounces down randomly.
        GameObject pPanel = new GameObject("PlinkoPanel", typeof(RectTransform));
        pPanel.transform.SetParent(contentArea.transform, false);
        AddImageWithSprite(pPanel, bgDark);
        FillRect(pPanel.GetComponent<RectTransform>());
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

        lobbyManager.lobbyPanel = lobbyPanel;
        lobbyManager.crashPanel = crashPanel;
        lobbyManager.roulettePanel = rPanel;
        lobbyManager.slotsPanel = sPanel;
        lobbyManager.minesPanel = mPanel;
        lobbyManager.plinkoPanel = pPanel;
        lobbyManager.lobbyBalanceText = globalBalanceText;
        
        btnHome.GetComponent<Button>().onClick.AddListener(lobbyManager.ShowLobby);
        btnCrash.GetComponent<Button>().onClick.AddListener(lobbyManager.StartCrash);
        btnRoulette.GetComponent<Button>().onClick.AddListener(lobbyManager.StartRoulette);
        btnSlots.GetComponent<Button>().onClick.AddListener(lobbyManager.StartSlots);
        btnMines.GetComponent<Button>().onClick.AddListener(lobbyManager.StartMines);
        btnPlinko.GetComponent<Button>().onClick.AddListener(lobbyManager.StartPlinko);



        SetLayerRecursively(canvasGO, LayerMask.NameToLayer("UI"));
        canvasGO.SetActive(true);
        lobbyPanel.SetActive(true);
        
        // Since I merged the features into this wizard, I delete the old feature wizard script so the user's project stays clean.
        if(System.IO.File.Exists("Assets/Editor/CasinoFeaturesWizard.cs")) {
            System.IO.File.Delete("Assets/Editor/CasinoFeaturesWizard.cs");
            AssetDatabase.Refresh();
        }

        Debug.Log("Kenchi Casino: Mobile WebGL UI Generation Complete!");
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

    static void CreateBetSidebarMobile(GameObject parentPanel, Font font, out InputField betInput, out Button halfBtn, out Button doubleBtn, out Button actionBtn)
    {
        GameObject betPanel = new GameObject("BetPanel", typeof(RectTransform));
        betPanel.transform.SetParent(parentPanel.transform, false);
        Image bg = AddImageWithSprite(betPanel, new Color(0.11f, 0.13f, 0.17f, 1f));
        RectTransform rt = betPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1); rt.sizeDelta = new Vector2(0, 200); rt.anchoredPosition = Vector2.zero;
        
        Text label = CreateText(betPanel, "BetLabel", "Bet Amount", font, 30, new Vector2(20, -40), new Vector2(400, 40));
        label.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1); label.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        label.GetComponent<RectTransform>().pivot = new Vector2(0, 1); label.alignment = TextAnchor.MiddleLeft; label.color = new Color(0.6f, 0.6f, 0.7f, 1f);
        
        GameObject inputBgGO = new GameObject("InputBg", typeof(RectTransform));
        inputBgGO.transform.SetParent(betPanel.transform, false);
        Image inputBg = AddImageWithSprite(inputBgGO, new Color(0.08f, 0.09f, 0.12f, 1f));
        RectTransform inRt = inputBgGO.GetComponent<RectTransform>();
        inRt.anchorMin = new Vector2(0, 1); inRt.anchorMax = new Vector2(0, 1); inRt.pivot = new Vector2(0, 1);
        inRt.anchoredPosition = new Vector2(20, -100); inRt.sizeDelta = new Vector2(500, 80);
        
        GameObject textGO = new GameObject("Text", typeof(RectTransform)); textGO.transform.SetParent(inputBgGO.transform, false);
        Text text = textGO.AddComponent<Text>(); text.font = font; text.fontSize = 35; text.color = Color.white; text.alignment = TextAnchor.MiddleLeft;
        RectTransform textRect = textGO.GetComponent<RectTransform>(); textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one; 
        textRect.offsetMin = new Vector2(20, 0); textRect.offsetMax = new Vector2(-180, 0); 
        
        betInput = inputBgGO.AddComponent<InputField>();
        betInput.textComponent = text; betInput.text = "10.00"; betInput.contentType = InputField.ContentType.DecimalNumber;
        
        GameObject halfGO = CreateButton(inputBgGO, "HalfBtn", "1/2", font, new Vector2(-95, 0), new Vector2(80, 60), new Color(0.17f, 0.19f, 0.25f, 1f), 24);
        AnchorToRight(halfGO, -95); halfBtn = halfGO.GetComponent<Button>();
        GameObject doubleGO = CreateButton(inputBgGO, "DoubleBtn", "2x", font, new Vector2(-10, 0), new Vector2(80, 60), new Color(0.17f, 0.19f, 0.25f, 1f), 24);
        AnchorToRight(doubleGO, -10); doubleBtn = doubleGO.GetComponent<Button>();

        GameObject actionGO = CreateButton(betPanel, "ActionBtn", "ACTION", font, new Vector2(-20, -100), new Vector2(400, 80), new Color(0f, 0.9f, 0.0f, 1f), 35);
        actionGO.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1); actionGO.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        actionGO.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
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

    static GameObject CreateGameThumb(GameObject parent, string name, string resourcePath, Font font, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGo = new GameObject(name + "Thumb", typeof(RectTransform));
        btnGo.transform.SetParent(parent.transform, false);
        Image img = btnGo.AddComponent<Image>();
        
        Texture2D tex = Resources.Load<Texture2D>(resourcePath);
        if(tex != null) {
            img.sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f,0.5f));
        } else {
            img.color = new Color(0.1f, 0.12f, 0.16f); // Fallback dark gray
        }
        
        Button btn = btnGo.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        // Name Tag Background
        GameObject nameBg = new GameObject("NameBg", typeof(RectTransform));
        nameBg.transform.SetParent(btnGo.transform, false);
        Image bgImg = nameBg.AddComponent<Image>(); bgImg.color = new Color(0,0,0,0.8f);
        RectTransform nbRt = nameBg.GetComponent<RectTransform>();
        nbRt.anchorMin = new Vector2(0,0); nbRt.anchorMax = new Vector2(1,0);
        nbRt.pivot = new Vector2(0.5f,0); nbRt.sizeDelta = new Vector2(0, 80);
        nbRt.anchoredPosition = Vector2.zero;

        Text t2 = CreateText(nameBg, "Name", name, font, 35, Vector2.zero, new Vector2(0, 80));
        t2.GetComponent<RectTransform>().anchorMin = Vector2.zero; t2.GetComponent<RectTransform>().anchorMax = Vector2.one;
        t2.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
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
