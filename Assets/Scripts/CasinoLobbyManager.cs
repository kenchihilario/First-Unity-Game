using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CasinoLobbyManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject crashPanel;
    public GameObject roulettePanel;
    public GameObject slotsPanel;
    public GameObject minesPanel;
    public GameObject plinkoPanel;
    public GameObject dicePanel;
    public GameObject blackjackPanel;

    [Header("Lobby UI")]
    public Text lobbyBalanceText;
    public Button playCrashButton;
    public Button playRouletteButton;
    public Button playSlotsButton;
    public Button playMinesButton;
    public Button playPlinkoButton;
    public Button playDiceButton;
    public Button playBlackjackButton;

    [Header("Top Bar Buttons (Auto-Wired)")]
    public Button loginBtn;
    public Button registerBtn;
    public Button depositBtn;
    public Button withdrawBtn;
    public Button logoutBtn;

    private bool _hasInitialized = false;

    void Awake()
    {
        AutoWireButtons();
        StyleCredibleLobbyUI();
    }

    void Start()
    {
        ShowLobby();
        AutoWireButtons();
        StyleCredibleLobbyUI();
    }

    void Update()
    {
        if (!_hasInitialized)
        {
            _hasInitialized = true;
            AutoWireButtons();
            StyleCredibleLobbyUI();
        }

        if (PlayerManager.Instance != null && lobbyBalanceText != null)
        {
            lobbyBalanceText.text = $"<color=#94A3B8>PHP</color>  <b><color=#00FFAA>₱{PlayerManager.Instance.balance:N2}</color></b>";
        }
    }

    public void StyleCredibleLobbyUI()
    {
        CanvasScaler scaler = FindAnyObjectByType<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (lobbyPanel != null)
        {
            Image lpImg = lobbyPanel.GetComponent<Image>();
            if (lpImg != null) lpImg.color = new Color(0.043f, 0.055f, 0.078f, 1f);
        }

        GameObject topBar = GameObject.Find("TopBar");
        if (topBar != null)
        {
            Image tbImg = topBar.GetComponent<Image>();
            if (tbImg != null) tbImg.color = new Color(0.047f, 0.063f, 0.098f, 1f);

            RectTransform tbRt = topBar.GetComponent<RectTransform>();
            if (tbRt != null)
            {
                tbRt.anchorMin = new Vector2(0, 1);
                tbRt.anchorMax = new Vector2(1, 1);
                tbRt.pivot = new Vector2(0.5f, 1);
                tbRt.anchoredPosition = Vector2.zero;
                tbRt.sizeDelta = new Vector2(0, 75);
            }

            Text logo = topBar.transform.Find("Logo")?.GetComponent<Text>();
            if (logo != null)
            {
                logo.text = "<b><size=28><color=#F59E0B>✦</color> <color=#FFFFFF>KENCHI</color> <color=#10B981>CASINO</color></size></b>";
                logo.alignment = TextAnchor.MiddleLeft;
                logo.raycastTarget = false;
                RectTransform lRt = logo.GetComponent<RectTransform>();
                if (lRt != null)
                {
                    lRt.anchorMin = new Vector2(0, 0.5f);
                    lRt.anchorMax = new Vector2(0, 0.5f);
                    lRt.pivot = new Vector2(0, 0.5f);
                    lRt.anchoredPosition = new Vector2(40, 0);
                    lRt.sizeDelta = new Vector2(350, 60);
                }
            }

            Transform loggedInGroup = topBar.transform.Find("LoggedInGroup");
            if (loggedInGroup != null)
            {
                RectTransform ligRt = loggedInGroup.GetComponent<RectTransform>();
                if (ligRt != null)
                {
                    ligRt.anchorMin = new Vector2(1, 0.5f);
                    ligRt.anchorMax = new Vector2(1, 0.5f);
                    ligRt.pivot = new Vector2(1, 0.5f);
                    ligRt.anchoredPosition = new Vector2(-40, 0);
                    ligRt.sizeDelta = new Vector2(680, 50);
                }

                HorizontalLayoutGroup hlg = loggedInGroup.GetComponent<HorizontalLayoutGroup>();
                if (hlg == null) hlg = loggedInGroup.gameObject.AddComponent<HorizontalLayoutGroup>();
                hlg.childAlignment = TextAnchor.MiddleRight;
                hlg.spacing = 14;
                hlg.childControlWidth = false;
                hlg.childControlHeight = false;
                hlg.childForceExpandWidth = false;
                hlg.childForceExpandHeight = false;

                Transform balBg = loggedInGroup.Find("BalanceBg");
                if (balBg != null)
                {
                    Image balImg = balBg.GetComponent<Image>();
                    if (balImg != null) balImg.color = new Color(0.075f, 0.098f, 0.153f, 1f);
                    RectTransform balRt = balBg.GetComponent<RectTransform>();
                    if (balRt != null)
                    {
                        balRt.sizeDelta = new Vector2(250, 46);
                    }
                    Text balTxt = balBg.GetComponentInChildren<Text>();
                    if (balTxt != null)
                    {
                        balTxt.fontSize = 20;
                        balTxt.alignment = TextAnchor.MiddleCenter;
                        if (PlayerManager.Instance != null)
                        {
                            balTxt.text = $"<color=#94A3B8>PHP</color>  <b><color=#00FFAA>₱{PlayerManager.Instance.balance:N2}</color></b>";
                        }
                    }
                }

                Button depBtn = loggedInGroup.Find("DepositBtn")?.GetComponent<Button>();
                if (depBtn != null)
                {
                    Image depImg = depBtn.GetComponent<Image>();
                    if (depImg != null) depImg.color = new Color(0.063f, 0.725f, 0.506f, 1f);
                    Text depTxt = depBtn.GetComponentInChildren<Text>();
                    if (depTxt != null)
                    {
                        depTxt.text = "<b>+ DEPOSIT</b>";
                        depTxt.fontSize = 17;
                        depTxt.raycastTarget = false;
                    }
                    RectTransform depRt = depBtn.GetComponent<RectTransform>();
                    if (depRt != null) depRt.sizeDelta = new Vector2(135, 46);
                    if (depBtn.GetComponent<ButtonHoverJuice>() == null) depBtn.gameObject.AddComponent<ButtonHoverJuice>();
                    depBtn.onClick.RemoveAllListeners();
                    depBtn.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowDeposit(); });
                }

                Button withBtn = loggedInGroup.Find("WithdrawBtn")?.GetComponent<Button>();
                if (withBtn != null)
                {
                    Image withImg = withBtn.GetComponent<Image>();
                    if (withImg != null) withImg.color = new Color(0.137f, 0.180f, 0.259f, 1f);
                    Text withTxt = withBtn.GetComponentInChildren<Text>();
                    if (withTxt != null)
                    {
                        withTxt.text = "<b>WITHDRAW</b>";
                        withTxt.fontSize = 16;
                        withTxt.raycastTarget = false;
                    }
                    RectTransform withRt = withBtn.GetComponent<RectTransform>();
                    if (withRt != null) withRt.sizeDelta = new Vector2(130, 46);
                    if (withBtn.GetComponent<ButtonHoverJuice>() == null) withBtn.gameObject.AddComponent<ButtonHoverJuice>();
                    withBtn.onClick.RemoveAllListeners();
                    withBtn.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowWithdraw(); });
                }

                Button logoutBtn = loggedInGroup.Find("LogoutBtn")?.GetComponent<Button>();
                if (logoutBtn != null)
                {
                    Image logImg = logoutBtn.GetComponent<Image>();
                    if (logImg != null) logImg.color = new Color(0.137f, 0.180f, 0.259f, 1f);
                    Text logTxt = logoutBtn.GetComponentInChildren<Text>();
                    if (logTxt != null) { logTxt.text = "<b>✕</b>"; logTxt.fontSize = 20; logTxt.raycastTarget = false; }
                    RectTransform logRt = logoutBtn.GetComponent<RectTransform>();
                    if (logRt != null) logRt.sizeDelta = new Vector2(46, 46);
                    if (logoutBtn.GetComponent<ButtonHoverJuice>() == null) logoutBtn.gameObject.AddComponent<ButtonHoverJuice>();
                    logoutBtn.onClick.RemoveAllListeners();
                    logoutBtn.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.OnLogoutClick(); });
                }

                Button lbBtn = loggedInGroup.Find("LeaderboardBtn")?.GetComponent<Button>();
                if (lbBtn != null) lbBtn.gameObject.SetActive(false);

                Transform profBtn = loggedInGroup.Find("ProfileBtn");
                if (profBtn != null) profBtn.gameObject.SetActive(false);
            }

            Transform loggedOutGroup = topBar.transform.Find("LoggedOutGroup");
            if (loggedOutGroup != null)
            {
                RectTransform logRt = loggedOutGroup.GetComponent<RectTransform>();
                if (logRt != null)
                {
                    logRt.anchorMin = new Vector2(1, 0.5f);
                    logRt.anchorMax = new Vector2(1, 0.5f);
                    logRt.pivot = new Vector2(1, 0.5f);
                    logRt.anchoredPosition = new Vector2(-40, 0);
                    logRt.sizeDelta = new Vector2(320, 50);
                }

                HorizontalLayoutGroup hlg = loggedOutGroup.GetComponent<HorizontalLayoutGroup>();
                if (hlg == null) hlg = loggedOutGroup.gameObject.AddComponent<HorizontalLayoutGroup>();
                hlg.childAlignment = TextAnchor.MiddleRight;
                hlg.spacing = 14;
                hlg.childControlWidth = false;
                hlg.childControlHeight = false;
                hlg.childForceExpandWidth = false;
                hlg.childForceExpandHeight = false;

                Button loginB = loggedOutGroup.Find("LoginBtn")?.GetComponent<Button>();
                if (loginB != null)
                {
                    Image img = loginB.GetComponent<Image>();
                    if (img != null) img.color = new Color(0.137f, 0.180f, 0.259f, 1f);
                    Text txt = loginB.GetComponentInChildren<Text>();
                    if (txt != null) { txt.text = "<b>LOG IN</b>"; txt.fontSize = 17; txt.raycastTarget = false; }
                    RectTransform rt = loginB.GetComponent<RectTransform>();
                    if (rt != null) rt.sizeDelta = new Vector2(130, 46);
                    if (loginB.GetComponent<ButtonHoverJuice>() == null) loginB.gameObject.AddComponent<ButtonHoverJuice>();
                    loginB.onClick.RemoveAllListeners();
                    loginB.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowLogin(); });
                }
                Button regB = loggedOutGroup.Find("RegisterBtn")?.GetComponent<Button>();
                if (regB != null)
                {
                    Image img = regB.GetComponent<Image>();
                    if (img != null) img.color = new Color(0.145f, 0.388f, 0.922f, 1f);
                    Text txt = regB.GetComponentInChildren<Text>();
                    if (txt != null) { txt.text = "<b>REGISTER</b>"; txt.fontSize = 17; txt.raycastTarget = false; }
                    RectTransform rt = regB.GetComponent<RectTransform>();
                    if (rt != null) rt.sizeDelta = new Vector2(135, 46);
                    if (regB.GetComponent<ButtonHoverJuice>() == null) regB.gameObject.AddComponent<ButtonHoverJuice>();
                    regB.onClick.RemoveAllListeners();
                    regB.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowRegister(); });
                }
            }
        }

        GameObject heroBanner = GameObject.Find("HeroBanner");
        if (heroBanner != null)
        {
            Image hbImg = heroBanner.GetComponent<Image>();
            if (hbImg != null) hbImg.color = new Color(0.067f, 0.090f, 0.141f, 1f);

            RectTransform hbRt = heroBanner.GetComponent<RectTransform>();
            if (hbRt != null)
            {
                hbRt.anchorMin = new Vector2(0, 1);
                hbRt.anchorMax = new Vector2(1, 1);
                hbRt.pivot = new Vector2(0.5f, 1);
                hbRt.offsetMin = new Vector2(60, -170);
                hbRt.offsetMax = new Vector2(-60, -90);
            }

            Text hTitle = heroBanner.transform.Find("HeroTitle")?.GetComponent<Text>();
            if (hTitle != null)
            {
                hTitle.text = "<b>ORIGINALS & POPULAR GAMES</b>";
                hTitle.fontSize = 25;
                hTitle.alignment = TextAnchor.MiddleCenter;
                hTitle.raycastTarget = false;
                RectTransform tRt = hTitle.GetComponent<RectTransform>();
                if (tRt != null) { tRt.anchoredPosition = new Vector2(0, 12); tRt.sizeDelta = new Vector2(1200, 36); }
            }

            Text hSub = heroBanner.transform.Find("HeroSub")?.GetComponent<Text>();
            if (hSub != null)
            {
                hSub.text = "<b><color=#10B981>Philippine Peso (PHP)</color>  •  <color=#94A3B8>7 Provably Fair Games  •  Fast Gameplay</color></b>";
                hSub.fontSize = 15;
                hSub.alignment = TextAnchor.MiddleCenter;
                hSub.raycastTarget = false;
                RectTransform subRt = hSub.GetComponent<RectTransform>();
                if (subRt != null) { subRt.anchoredPosition = new Vector2(0, -14); subRt.sizeDelta = new Vector2(1200, 28); }
            }
        }

        GameObject scrollView = GameObject.Find("GamesScrollView");
        if (scrollView != null)
        {
            RectTransform sRt = scrollView.GetComponent<RectTransform>();
            if (sRt != null)
            {
                sRt.anchorMin = Vector2.zero;
                sRt.anchorMax = Vector2.one;
                sRt.offsetMin = new Vector2(60, 20);
                sRt.offsetMax = new Vector2(-60, -190);
            }
        }

        GameObject grid = GameObject.Find("Content");
        if (grid != null)
        {
            GridLayoutGroup layout = grid.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(360, 360);
                layout.spacing = new Vector2(24, 24);
                layout.padding = new RectOffset(20, 20, 10, 30);
                layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layout.constraintCount = 4;
                layout.childAlignment = TextAnchor.UpperCenter;
            }

            RectTransform gRt = grid.GetComponent<RectTransform>();
            if (gRt != null)
            {
                gRt.sizeDelta = new Vector2(0, 800);
            }
        }

        var cardMeta = new Dictionary<string, (string title, string tag, string sub, string hexColor)>()
        {
            { "CRASH", ("CRASH", "99% RTP", "Multiplier", "10B981") },
            { "ROULETTE", ("ROULETTE", "CLASSIC", "European", "EF4444") },
            { "SLOTS", ("SLOTS", "JACKPOT", "3-Reel Classic", "A855F7") },
            { "MINES", ("MINES", "99% RTP", "Gem Grid", "10B981") },
            { "PLINKO", ("PLINKO", "1000X", "Peg Drop", "6366F1") },
            { "DICE", ("DICE", "FAIR", "Over / Under", "F59E0B") },
            { "BLACKJACK", ("BLACKJACK", "3:2 PAY", "Table 21", "059669") }
        };

        foreach (var kvp in cardMeta)
        {
            string gKey = kvp.Key;
            var data = kvp.Value;
            GameObject cardGo = GameObject.Find(gKey + "Thumb");
            if (cardGo == null) continue;

            if (cardGo.GetComponent<ButtonHoverJuice>() == null)
            {
                var juice = cardGo.AddComponent<ButtonHoverJuice>();
                juice.hoverScale = 1.03f;
                juice.clickScale = 0.97f;
                juice.speed = 20f;
            }

            Image cardImg = cardGo.GetComponent<Image>();
            if (cardImg != null)
            {
                Sprite sp = LoadThumbnailSprite(gKey.ToLower());
                if (sp != null)
                {
                    cardImg.sprite = sp;
                    cardImg.type = Image.Type.Simple;
                    cardImg.preserveAspect = false;
                    cardImg.color = Color.white;
                }
                cardImg.raycastTarget = true;
            }

            Transform nameBg = cardGo.transform.Find("NameBg");
            if (nameBg != null)
            {
                // Rainbet style tiles already include the crisp typography cleanly embedded!
                nameBg.gameObject.SetActive(false);
            }

            Graphic[] graphics = cardGo.GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics)
            {
                if (g.gameObject != cardGo) g.raycastTarget = false;
            }
        }

        GameObject bottomBar = GameObject.Find("BottomNavBar");
        if (bottomBar != null)
        {
            bottomBar.SetActive(false);
        }
    }

    public void AutoWireButtons()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var b in allButtons)
        {
            string bName = b.gameObject.name;
            if (bName.Equals("CRASHThumb", System.StringComparison.OrdinalIgnoreCase) || bName.Equals("Crash", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveListener(StartCrash);
                b.onClick.AddListener(StartCrash);
            }
            else if (bName.Equals("ROULETTEThumb", System.StringComparison.OrdinalIgnoreCase) || bName.Equals("Roulette", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveListener(StartRoulette);
                b.onClick.AddListener(StartRoulette);
            }
            else if (bName.Equals("SLOTSThumb", System.StringComparison.OrdinalIgnoreCase) || bName.Equals("Slots", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveListener(StartSlots);
                b.onClick.AddListener(StartSlots);
            }
            else if (bName.Equals("MINESThumb", System.StringComparison.OrdinalIgnoreCase) || bName.Equals("Mines", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveListener(StartMines);
                b.onClick.AddListener(StartMines);
            }
            else if (bName.Equals("PLINKOThumb", System.StringComparison.OrdinalIgnoreCase) || bName.Equals("Plinko", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveListener(StartPlinko);
                b.onClick.AddListener(StartPlinko);
            }
            else if (bName.Equals("DICEThumb", System.StringComparison.OrdinalIgnoreCase) || bName.Equals("Dice", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveListener(StartDice);
                b.onClick.AddListener(StartDice);
            }
            else if (bName.Equals("BLACKJACKThumb", System.StringComparison.OrdinalIgnoreCase) || bName.Equals("Blackjack", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveListener(StartBlackjack);
                b.onClick.AddListener(StartBlackjack);
            }
            else if (bName.Equals("Home", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveListener(ShowLobby);
                b.onClick.AddListener(ShowLobby);
            }
            else if (bName.Equals("LoginBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowLogin(); });
            }
            else if (bName.Equals("RegisterBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowRegister(); });
            }
            else if (bName.Equals("DepositBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowDeposit(); });
            }
            else if (bName.Equals("WithdrawBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowWithdraw(); });
            }
            else if (bName.Equals("LogoutBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.OnLogoutClick(); });
            }

            Graphic[] graphics = b.GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics)
            {
                if (g.gameObject != b.gameObject) g.raycastTarget = false;
            }
        }

        if (AuthUIManager.Instance != null)
        {
            AuthUIManager.Instance.WireAllButtons();
        }

        if (playCrashButton != null) { playCrashButton.onClick.RemoveListener(StartCrash); playCrashButton.onClick.AddListener(StartCrash); }
        if (playRouletteButton != null) { playRouletteButton.onClick.RemoveListener(StartRoulette); playRouletteButton.onClick.AddListener(StartRoulette); }
        if (playSlotsButton != null) { playSlotsButton.onClick.RemoveListener(StartSlots); playSlotsButton.onClick.AddListener(StartSlots); }
        if (playMinesButton != null) { playMinesButton.onClick.RemoveListener(StartMines); playMinesButton.onClick.AddListener(StartMines); }
        if (playPlinkoButton != null) { playPlinkoButton.onClick.RemoveListener(StartPlinko); playPlinkoButton.onClick.AddListener(StartPlinko); }
        if (playDiceButton != null) { playDiceButton.onClick.RemoveListener(StartDice); playDiceButton.onClick.AddListener(StartDice); }
        if (playBlackjackButton != null) { playBlackjackButton.onClick.RemoveListener(StartBlackjack); playBlackjackButton.onClick.AddListener(StartBlackjack); }
    }

    public void RefreshThumbnails()
    {
        StyleCredibleLobbyUI();
    }

    private Sprite LoadThumbnailSprite(string lowerName)
    {
        Sprite res = Resources.Load<Sprite>($"Thumbs/{lowerName}");
        if (res != null) return res;

        Texture2D resTex = Resources.Load<Texture2D>($"Thumbs/{lowerName}");
        if (resTex != null)
        {
            return Sprite.Create(resTex, new Rect(0, 0, resTex.width, resTex.height), new Vector2(0.5f, 0.5f));
        }

        string[] exts = { ".png", ".jpg" };
        foreach (var ext in exts)
        {
            string filePath = System.IO.Path.Combine(Application.dataPath, "Resources", "Thumbs", lowerName + ext);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                    Texture2D tex = new Texture2D(512, 512, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(bytes))
                    {
                        tex.wrapMode = TextureWrapMode.Clamp;
                        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("Direct file load failed: " + ex.Message);
                }
            }
        }

        return null;
    }

    public void ShowLobby()
    {
        if(lobbyPanel) lobbyPanel.SetActive(true);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
        if(dicePanel) dicePanel.SetActive(false);
        if(blackjackPanel) blackjackPanel.SetActive(false);
        StyleCredibleLobbyUI();
    }

    public void SetupGameHeaderAndBack(GameObject panel, string gameTitle)
    {
        if (panel == null) return;

        Transform existingHeader = panel.transform.Find("GameHeader");
        if (existingHeader == null)
        {
            GameObject headerGo = new GameObject("GameHeader", typeof(RectTransform));
            headerGo.transform.SetParent(panel.transform, false);
            headerGo.transform.SetAsFirstSibling();

            Image bg = headerGo.AddComponent<Image>();
            bg.color = new Color(0.059f, 0.082f, 0.133f, 1f);

            RectTransform hRt = headerGo.GetComponent<RectTransform>();
            hRt.anchorMin = new Vector2(0, 1);
            hRt.anchorMax = new Vector2(1, 1);
            hRt.pivot = new Vector2(0.5f, 1);
            hRt.anchoredPosition = Vector2.zero;
            hRt.sizeDelta = new Vector2(0, 100);

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            GameObject backGo = new GameObject("BackBtn", typeof(RectTransform));
            backGo.transform.SetParent(headerGo.transform, false);
            Image backImg = backGo.AddComponent<Image>();
            backImg.color = new Color(0.122f, 0.161f, 0.216f, 1f);
            Button backBtn = backGo.AddComponent<Button>();
            backBtn.onClick.AddListener(ShowLobby);
            backGo.AddComponent<ButtonHoverJuice>();

            RectTransform bRt = backGo.GetComponent<RectTransform>();
            bRt.anchorMin = new Vector2(0, 0.5f);
            bRt.anchorMax = new Vector2(0, 0.5f);
            bRt.pivot = new Vector2(0, 0.5f);
            bRt.anchoredPosition = new Vector2(25, 0);
            bRt.sizeDelta = new Vector2(220, 70);

            GameObject backTxtGo = new GameObject("Text", typeof(RectTransform));
            backTxtGo.transform.SetParent(backGo.transform, false);
            Text backTxt = backTxtGo.AddComponent<Text>();
            backTxt.font = font;
            backTxt.fontSize = 28;
            backTxt.fontStyle = FontStyle.Bold;
            backTxt.alignment = TextAnchor.MiddleCenter;
            backTxt.text = "<b><color=#00FFAA>←</color> LOBBY</b>";
            backTxt.color = Color.white;
            RectTransform btRt = backTxtGo.GetComponent<RectTransform>();
            btRt.anchorMin = Vector2.zero;
            btRt.anchorMax = Vector2.one;
            btRt.offsetMin = Vector2.zero;
            btRt.offsetMax = Vector2.zero;

            GameObject titleGo = new GameObject("Title", typeof(RectTransform));
            titleGo.transform.SetParent(headerGo.transform, false);
            Text tText = titleGo.AddComponent<Text>();
            tText.font = font;
            tText.fontSize = 34;
            tText.fontStyle = FontStyle.Bold;
            tText.alignment = TextAnchor.MiddleCenter;
            tText.text = $"<b>{gameTitle}</b>";
            tText.color = Color.white;
            RectTransform tRt = titleGo.GetComponent<RectTransform>();
            tRt.anchorMin = new Vector2(0.5f, 0.5f);
            tRt.anchorMax = new Vector2(0.5f, 0.5f);
            tRt.pivot = new Vector2(0.5f, 0.5f);
            tRt.anchoredPosition = Vector2.zero;
            tRt.sizeDelta = new Vector2(350, 70);

            GameObject balGo = new GameObject("Balance", typeof(RectTransform));
            balGo.transform.SetParent(headerGo.transform, false);
            Text balTxt = balGo.AddComponent<Text>();
            balTxt.font = font;
            balTxt.fontSize = 28;
            balTxt.fontStyle = FontStyle.Bold;
            balTxt.alignment = TextAnchor.MiddleRight;
            balTxt.text = PlayerManager.Instance != null ? $"<b><color=#00FFAA>₱{PlayerManager.Instance.balance:N2}</color></b>" : "₱0.00";
            balTxt.color = Color.white;
            RectTransform balRt = balGo.GetComponent<RectTransform>();
            balRt.anchorMin = new Vector2(1, 0.5f);
            balRt.anchorMax = new Vector2(1, 0.5f);
            balRt.pivot = new Vector2(1, 0.5f);
            balRt.anchoredPosition = new Vector2(-25, 0);
            balRt.sizeDelta = new Vector2(260, 70);
        }
        else
        {
            Text balTxt = existingHeader.Find("Balance")?.GetComponent<Text>();
            if (balTxt != null && PlayerManager.Instance != null)
            {
                balTxt.text = $"<b><color=#00FFAA>₱{PlayerManager.Instance.balance:N2}</color></b>";
            }
        }

        Transform betPanel = panel.transform.Find("BetPanel");
        if (betPanel != null)
        {
            RectTransform bpRt = betPanel.GetComponent<RectTransform>();
            if (bpRt != null)
            {
                bpRt.anchorMin = new Vector2(0, 1);
                bpRt.anchorMax = new Vector2(1, 1);
                bpRt.pivot = new Vector2(0.5f, 1);
                bpRt.anchoredPosition = new Vector2(0, -100);
            }
        }
    }

    private bool CheckLogin()
    {
        if (AuthManager.Instance != null && !AuthManager.Instance.IsLoggedIn)
        {
            if (AuthUIManager.Instance != null)
            {
                AuthUIManager.Instance.ShowLogin();
            }
            return false;
        }
        return true;
    }

    public void StartCrash()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) { crashPanel.SetActive(true); SetupGameHeaderAndBack(crashPanel, "CRASH"); }
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
        if(dicePanel) dicePanel.SetActive(false);
        if(blackjackPanel) blackjackPanel.SetActive(false);
    }

    public void StartRoulette()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) { roulettePanel.SetActive(true); SetupGameHeaderAndBack(roulettePanel, "ROULETTE"); }
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
        if(dicePanel) dicePanel.SetActive(false);
        if(blackjackPanel) blackjackPanel.SetActive(false);
    }

    public void StartSlots()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) { slotsPanel.SetActive(true); SetupGameHeaderAndBack(slotsPanel, "SLOTS"); }
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
        if(dicePanel) dicePanel.SetActive(false);
        if(blackjackPanel) blackjackPanel.SetActive(false);
    }

    public void StartMines()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) { minesPanel.SetActive(true); SetupGameHeaderAndBack(minesPanel, "MINES"); }
        if(plinkoPanel) plinkoPanel.SetActive(false);
        if(dicePanel) dicePanel.SetActive(false);
        if(blackjackPanel) blackjackPanel.SetActive(false);
    }

    public void StartPlinko()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) { plinkoPanel.SetActive(true); SetupGameHeaderAndBack(plinkoPanel, "PLINKO"); }
        if(dicePanel) dicePanel.SetActive(false);
        if(blackjackPanel) blackjackPanel.SetActive(false);
    }

    public void StartDice()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
        if(dicePanel) { dicePanel.SetActive(true); SetupGameHeaderAndBack(dicePanel, "DICE"); }
        if(blackjackPanel) blackjackPanel.SetActive(false);
    }

    public void StartBlackjack()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
        if(dicePanel) dicePanel.SetActive(false);
        if(blackjackPanel) { blackjackPanel.SetActive(true); SetupGameHeaderAndBack(blackjackPanel, "BLACKJACK"); }
    }
}
