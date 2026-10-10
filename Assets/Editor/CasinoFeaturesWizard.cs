using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CasinoFeaturesWizard
{
    [MenuItem("Casino/Apply Rainbet Features")]
    public static void ApplyFeatures()
    {
        GameObject canvasGO = GameObject.Find("CasinoCanvas");
        if (canvasGO == null)
        {
            Debug.LogError("Run 'Casino/Setup Kenchi Casino' first!");
            return;
        }
        
        GameObject managers = GameObject.Find("CasinoManagers");
        if (managers.GetComponent<AuthManager>() == null)
        {
            managers.AddComponent<AuthManager>();
        }
        
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Color bgDark = new Color(0.08f, 0.09f, 0.12f, 1f);       
        Color panelColor = new Color(0.11f, 0.13f, 0.17f, 1f);   
        Color accentRed = new Color(1f, 0.13f, 0.33f, 1f);       
        Color accentGreen = new Color(0f, 0.9f, 0.0f, 1f);       
        Color accentGray = new Color(0.17f, 0.19f, 0.25f, 1f);   
        Color accentBlue = new Color(0.13f, 0.67f, 1f, 1f);

        // 1. TOP BAR AUTH GROUPS
        Transform topBar = canvasGO.transform.Find("TopBar");
        
        // Remove old top bar stuff if they exist
        if (topBar.Find("Profile")) Object.DestroyImmediate(topBar.Find("Profile").gameObject);
        if (topBar.Find("WithdrawBtn")) Object.DestroyImmediate(topBar.Find("WithdrawBtn").gameObject);
        if (topBar.Find("DepositBtn")) Object.DestroyImmediate(topBar.Find("DepositBtn").gameObject);
        if (topBar.Find("BalanceBg")) Object.DestroyImmediate(topBar.Find("BalanceBg").gameObject);
        
        // Logged Out Group
        GameObject loggedOutGroup = new GameObject("LoggedOutGroup", typeof(RectTransform));
        loggedOutGroup.transform.SetParent(topBar, false);
        RectTransform logOutRt = loggedOutGroup.GetComponent<RectTransform>();
        logOutRt.anchorMin = new Vector2(1, 0.5f);
        logOutRt.anchorMax = new Vector2(1, 0.5f);
        logOutRt.pivot = new Vector2(1, 0.5f);
        logOutRt.anchoredPosition = new Vector2(-20, 0);
        
        GameObject loginBtnGo = CreateButton(loggedOutGroup, "LoginBtn", "Log In", font, new Vector2(-150, 0), new Vector2(140, 45), accentGray, 18);
        loginBtnGo.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        loginBtnGo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        loginBtnGo.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);

        GameObject registerBtnGo = CreateButton(loggedOutGroup, "RegisterBtn", "Register", font, new Vector2(0, 0), new Vector2(140, 45), accentBlue, 18);
        registerBtnGo.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        registerBtnGo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        registerBtnGo.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);

        // Logged In Group
        GameObject loggedInGroup = new GameObject("LoggedInGroup", typeof(RectTransform));
        loggedInGroup.transform.SetParent(topBar, false);
        RectTransform logInRt = loggedInGroup.GetComponent<RectTransform>();
        logInRt.anchorMin = new Vector2(1, 0.5f);
        logInRt.anchorMax = new Vector2(1, 0.5f);
        logInRt.pivot = new Vector2(1, 0.5f);
        logInRt.anchoredPosition = new Vector2(-20, 0);
        
        GameObject logoutBtnGo = CreateButton(loggedInGroup, "LogoutBtn", "Logout", font, new Vector2(0, 0), new Vector2(100, 45), accentRed, 18);
        logoutBtnGo.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        logoutBtnGo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        logoutBtnGo.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        
        GameObject profileTextGo = new GameObject("ProfileText", typeof(RectTransform));
        profileTextGo.transform.SetParent(loggedInGroup, false);
        Text pText = profileTextGo.AddComponent<Text>();
        pText.font = font; pText.fontSize = 20; pText.color = Color.white; pText.alignment = TextAnchor.MiddleRight;
        pText.text = "1234567890";
        RectTransform pRt = profileTextGo.GetComponent<RectTransform>();
        pRt.anchorMin = new Vector2(1, 0.5f); pRt.anchorMax = new Vector2(1, 0.5f); pRt.pivot = new Vector2(1, 0.5f);
        pRt.anchoredPosition = new Vector2(-120, 0); pRt.sizeDelta = new Vector2(200, 45);

        GameObject withdrawBtnGo = CreateButton(loggedInGroup, "WithdrawBtn", "Withdraw", font, new Vector2(-340, 0), new Vector2(120, 45), accentGray, 18);
        withdrawBtnGo.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        withdrawBtnGo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        withdrawBtnGo.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);

        GameObject depositBtnGo = CreateButton(loggedInGroup, "DepositBtn", "Deposit", font, new Vector2(-470, 0), new Vector2(120, 45), accentGreen, 18);
        depositBtnGo.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        depositBtnGo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        depositBtnGo.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);

        GameObject balBg = new GameObject("BalanceBg", typeof(RectTransform));
        balBg.transform.SetParent(loggedInGroup.transform, false);
        AddImageWithSprite(balBg, bgDark);
        RectTransform balBgRt = balBg.GetComponent<RectTransform>();
        balBgRt.anchorMin = new Vector2(1, 0.5f);
        balBgRt.anchorMax = new Vector2(1, 0.5f);
        balBgRt.pivot = new Vector2(1, 0.5f);
        balBgRt.anchoredPosition = new Vector2(-600, 0);
        balBgRt.sizeDelta = new Vector2(250, 45);
        Text globalBalanceText = CreateText(balBg, "BalanceText", "$0.00", font, 22, Vector2.zero, new Vector2(230, 45));
        
        CasinoLobbyManager clm = managers.GetComponent<CasinoLobbyManager>();
        clm.lobbyBalanceText = globalBalanceText;
        
        // 2. AUTH & BANK OVERLAYS
        GameObject overlays = new GameObject("Overlays", typeof(RectTransform));
        overlays.transform.SetParent(canvasGO.transform, false);
        FillRect(overlays.GetComponent<RectTransform>());
        
        // Auth Overlay
        GameObject authOverlay = new GameObject("AuthOverlay", typeof(RectTransform));
        authOverlay.transform.SetParent(overlays.transform, false);
        FillRect(authOverlay.GetComponent<RectTransform>());
        Image authBg = AddImageWithSprite(authOverlay, new Color(0,0,0,0.8f));
        
        GameObject loginPanel = CreatePanel(authOverlay, "LoginPanel", new Vector2(400, 450), panelColor);
        CreateText(loginPanel, "Title", "LOG IN", font, 30, new Vector2(0, 180), new Vector2(300, 50));
        InputField loginPhone = CreateInputField(loginPanel, "PhoneInput", "Phone Number", font, new Vector2(0, 80));
        InputField loginPass = CreateInputField(loginPanel, "PassInput", "Password", font, new Vector2(0, 0));
        loginPass.inputType = InputField.InputType.Password;
        Text loginErr = CreateText(loginPanel, "Error", "", font, 18, new Vector2(0, -60), new Vector2(350, 40));
        loginErr.color = accentRed;
        GameObject loginSubmit = CreateButton(loginPanel, "SubmitBtn", "Log In", font, new Vector2(0, -120), new Vector2(350, 60), accentGreen, 22);
        GameObject closeLogin = CreateButton(loginPanel, "CloseBtn", "X", font, new Vector2(160, 190), new Vector2(40, 40), accentGray, 20);

        GameObject registerPanel = CreatePanel(authOverlay, "RegisterPanel", new Vector2(400, 450), panelColor);
        CreateText(registerPanel, "Title", "REGISTER", font, 30, new Vector2(0, 180), new Vector2(300, 50));
        InputField regPhone = CreateInputField(registerPanel, "PhoneInput", "Phone Number", font, new Vector2(0, 80));
        InputField regPass = CreateInputField(registerPanel, "PassInput", "Password", font, new Vector2(0, 0));
        regPass.inputType = InputField.InputType.Password;
        Text regErr = CreateText(registerPanel, "Error", "", font, 18, new Vector2(0, -60), new Vector2(350, 40));
        regErr.color = accentRed;
        GameObject regSubmit = CreateButton(registerPanel, "SubmitBtn", "Register", font, new Vector2(0, -120), new Vector2(350, 60), accentBlue, 22);
        GameObject closeReg = CreateButton(registerPanel, "CloseBtn", "X", font, new Vector2(160, 190), new Vector2(40, 40), accentGray, 20);

        // Bank Overlay
        GameObject bankOverlay = new GameObject("BankOverlay", typeof(RectTransform));
        bankOverlay.transform.SetParent(overlays.transform, false);
        FillRect(bankOverlay.GetComponent<RectTransform>());
        Image bankBg = AddImageWithSprite(bankOverlay, new Color(0,0,0,0.8f));

        GameObject depPanel = CreatePanel(bankOverlay, "DepositPanel", new Vector2(400, 350), panelColor);
        CreateText(depPanel, "Title", "DEPOSIT", font, 30, new Vector2(0, 120), new Vector2(300, 50));
        InputField depAmount = CreateInputField(depPanel, "AmountInput", "Amount ($)", font, new Vector2(0, 40));
        depAmount.contentType = InputField.ContentType.DecimalNumber;
        Text bankErrD = CreateText(depPanel, "Error", "", font, 18, new Vector2(0, -20), new Vector2(350, 40));
        bankErrD.color = accentRed;
        GameObject depSubmit = CreateButton(depPanel, "SubmitBtn", "Deposit", font, new Vector2(0, -80), new Vector2(350, 60), accentGreen, 22);
        GameObject closeDep = CreateButton(depPanel, "CloseBtn", "X", font, new Vector2(160, 130), new Vector2(40, 40), accentGray, 20);

        GameObject withPanel = CreatePanel(bankOverlay, "WithdrawPanel", new Vector2(400, 350), panelColor);
        CreateText(withPanel, "Title", "WITHDRAW", font, 30, new Vector2(0, 120), new Vector2(300, 50));
        InputField withAmount = CreateInputField(withPanel, "AmountInput", "Amount ($)", font, new Vector2(0, 40));
        withAmount.contentType = InputField.ContentType.DecimalNumber;
        Text bankErrW = CreateText(withPanel, "Error", "", font, 18, new Vector2(0, -20), new Vector2(350, 40));
        bankErrW.color = accentRed;
        GameObject withSubmit = CreateButton(withPanel, "SubmitBtn", "Withdraw", font, new Vector2(0, -80), new Vector2(350, 60), accentGray, 22);
        GameObject closeWith = CreateButton(withPanel, "CloseBtn", "X", font, new Vector2(160, 130), new Vector2(40, 40), accentGray, 20);
        
        AuthUIManager aUI = overlays.AddComponent<AuthUIManager>();
        aUI.authOverlay = authOverlay;
        aUI.loginPanel = loginPanel;
        aUI.registerPanel = registerPanel;
        aUI.bankOverlay = bankOverlay;
        aUI.depositPanel = depPanel;
        aUI.withdrawPanel = withPanel;
        
        aUI.loginPhoneInput = loginPhone; aUI.loginPasswordInput = loginPass; aUI.loginErrorText = loginErr;
        aUI.registerPhoneInput = regPhone; aUI.registerPasswordInput = regPass; aUI.registerErrorText = regErr;
        aUI.depositAmountInput = depAmount; aUI.withdrawAmountInput = withAmount;
        aUI.bankErrorText = bankErrD; 
        
        aUI.loggedOutGroup = loggedOutGroup;
        aUI.loggedInGroup = loggedInGroup;
        aUI.profileText = pText;

        // Hook up buttons
        loginBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowLogin);
        registerBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowRegister);
        depositBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowDeposit);
        withdrawBtnGo.GetComponent<Button>().onClick.AddListener(aUI.ShowWithdraw);
        logoutBtnGo.GetComponent<Button>().onClick.AddListener(aUI.OnLogoutClick);
        
        loginSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnLoginSubmit);
        regSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnRegisterSubmit);
        depSubmit.GetComponent<Button>().onClick.AddListener(aUI.OnDepositSubmit);
        withSubmit.GetComponent<Button>().onClick.AddListener(() => { aUI.bankErrorText = bankErrW; aUI.OnWithdrawSubmit(); });
        
        closeLogin.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeReg.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeDep.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);
        closeWith.GetComponent<Button>().onClick.AddListener(aUI.CloseAll);


        // 3. ENHANCE MAIN MENU (LOBBY)
        Transform contentArea = canvasGO.transform.Find("ContentArea");
        Transform lobbyP = contentArea.Find("LobbyPanel");
        
        // Remove old simple text
        if (lobbyP.Find("TitleText")) Object.DestroyImmediate(lobbyP.Find("TitleText").gameObject);
        if (lobbyP.Find("SubText")) Object.DestroyImmediate(lobbyP.Find("SubText").gameObject);
        
        // Add Hero Banner
        GameObject heroBanner = new GameObject("HeroBanner", typeof(RectTransform));
        heroBanner.transform.SetParent(lobbyP, false);
        AddImageWithSprite(heroBanner, new Color(0.05f, 0.06f, 0.08f, 1f));
        RectTransform hRt = heroBanner.GetComponent<RectTransform>();
        hRt.anchorMin = new Vector2(0, 1); hRt.anchorMax = new Vector2(1, 1); hRt.pivot = new Vector2(0.5f, 1);
        hRt.offsetMin = new Vector2(20, -250); hRt.offsetMax = new Vector2(-20, -20);
        
        Text heroTitle = CreateText(heroBanner, "HeroTitle", "<b>KENCHI</b> CASINO", font, 60, new Vector2(0, 30), new Vector2(800, 100));
        heroTitle.color = accentBlue;
        CreateText(heroBanner, "HeroSub", "Provably Fair. Instant Crypto. Top Games.", font, 24, new Vector2(0, -30), new Vector2(800, 50));
        
        GameObject grid = new GameObject("GamesGrid", typeof(RectTransform));
        grid.transform.SetParent(lobbyP, false);
        RectTransform gRt = grid.GetComponent<RectTransform>();
        gRt.anchorMin = new Vector2(0, 1); gRt.anchorMax = new Vector2(1, 1); gRt.pivot = new Vector2(0.5f, 1);
        gRt.offsetMin = new Vector2(20, -700); gRt.offsetMax = new Vector2(-20, -270);
        
        GridLayoutGroup layout = grid.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(300, 200);
        layout.spacing = new Vector2(20, 20);
        layout.childAlignment = TextAnchor.UpperCenter;

        // Create Game Thumbnails
        CreateGameThumb(grid.gameObject, "CRASH", "🚀", accentBlue, font, clm.StartCrash);
        CreateGameThumb(grid.gameObject, "ROULETTE", "🎡", accentRed, font, clm.StartRoulette);
        CreateGameThumb(grid.gameObject, "SLOTS", "🎰", new Color(1f, 0.8f, 0f), font, clm.StartSlots);
        CreateGameThumb(grid.gameObject, "MINES", "💣", accentGray, font, clm.StartMines);
        CreateGameThumb(grid.gameObject, "PLINKO", "👾", accentGreen, font, clm.StartPlinko);

        SetLayerRecursively(canvasGO, LayerMask.NameToLayer("UI"));
        Debug.Log("Rainbet Features Applied!");
    }

    static GameObject CreateGameThumb(GameObject parent, string name, string icon, Color color, Font font, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGo = CreateButton(parent, name + "Thumb", "", font, Vector2.zero, new Vector2(300, 200), color, 20);
        
        Text t1 = CreateText(btnGo, "Icon", icon, font, 80, new Vector2(0, 20), new Vector2(300, 100));
        Text t2 = CreateText(btnGo, "Name", name, font, 28, new Vector2(0, -60), new Vector2(300, 50));
        
        btnGo.GetComponent<Button>().onClick.AddListener(onClick);
        return btnGo;
    }

    static GameObject CreatePanel(GameObject parent, string name, Vector2 size, Color color) {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        Image img = AddImageWithSprite(go, color);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        return go;
    }

    static InputField CreateInputField(GameObject parent, string name, string placeholder, Font font, Vector2 pos) {
        GameObject inputBgGO = new GameObject(name, typeof(RectTransform));
        inputBgGO.transform.SetParent(parent.transform, false);
        Image inputBg = AddImageWithSprite(inputBgGO, new Color(0.08f, 0.09f, 0.12f, 1f));
        RectTransform inRt = inputBgGO.GetComponent<RectTransform>();
        inRt.anchoredPosition = pos;
        inRt.sizeDelta = new Vector2(350, 60);
        
        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(inputBgGO.transform, false);
        Text text = textGO.AddComponent<Text>();
        text.font = font; text.fontSize = 22; text.color = Color.white; text.alignment = TextAnchor.MiddleLeft;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one; 
        textRect.offsetMin = new Vector2(15, 0); textRect.offsetMax = new Vector2(-15, 0);
        
        GameObject pTextGO = new GameObject("Placeholder", typeof(RectTransform));
        pTextGO.transform.SetParent(inputBgGO.transform, false);
        Text pText = pTextGO.AddComponent<Text>();
        pText.font = font; pText.fontSize = 22; pText.color = new Color(0.5f,0.5f,0.5f); pText.alignment = TextAnchor.MiddleLeft;
        pText.text = placeholder;
        RectTransform pRect = pTextGO.GetComponent<RectTransform>();
        pRect.anchorMin = Vector2.zero; pRect.anchorMax = Vector2.one; 
        pRect.offsetMin = new Vector2(15, 0); pRect.offsetMax = new Vector2(-15, 0);

        InputField inputField = inputBgGO.AddComponent<InputField>();
        inputField.textComponent = text;
        inputField.placeholder = pText;
        return inputField;
    }

    static void FillRect(RectTransform rect) {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.anchoredPosition = Vector2.zero;
    }

    static Text CreateText(GameObject parent, string name, string textStr, Font font, int size, Vector2 pos, Vector2 sizeDelta) {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        Text text = go.AddComponent<Text>();
        text.text = textStr; text.fontSize = size; text.fontStyle = FontStyle.Bold; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.font = font;
        text.horizontalOverflow = HorizontalWrapMode.Overflow; text.verticalOverflow = VerticalWrapMode.Overflow; text.supportRichText = true;
        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(0,0,0,0.5f); shadow.effectDistance = new Vector2(2, -2);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos; rect.sizeDelta = sizeDelta; rect.localScale = Vector3.one;
        return text;
    }

    static GameObject CreateButton(GameObject parent, string name, string textStr, Font font, Vector2 pos, Vector2 size, Color color, int fontSize = 30) {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        Image img = AddImageWithSprite(go, color);
        Button btn = go.AddComponent<Button>();
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos; rect.sizeDelta = size; rect.localScale = Vector3.one;
        Text text = CreateText(go, "Text", textStr, font, fontSize, Vector2.zero, size);
        return go;
    }

    static Image AddImageWithSprite(GameObject go, Color color) {
        Image img = go.AddComponent<Image>();
        img.color = color;
        Sprite bgSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        if (bgSprite != null) { img.sprite = bgSprite; img.type = Image.Type.Sliced; }
        return img;
    }
    
    static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform) SetLayerRecursively(child.gameObject, layer);
    }
}
