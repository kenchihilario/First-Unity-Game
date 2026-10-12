using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool _isStyling = false;
    private bool _buttonsWired = false;

    void Awake()
    {
    }

    void Start()
    {
        if (_hasInitialized) return;
        _hasInitialized = true;

        EnsurePanelsFound();
        ShowLobby();
        AutoWireButtons();
    }

    void Update()
    {
        if (PlayerManager.Instance != null)
        {
            if (lobbyBalanceText == null)
            {
                GameObject topBar = GameObject.Find("TopBar");
                if (topBar != null)
                {
                    Transform balBg = topBar.transform.Find("LoggedInGroup/BalanceBg");
                    if (balBg != null) lobbyBalanceText = balBg.GetComponentInChildren<Text>(true);
                }
            }
            if (lobbyBalanceText != null)
            {
                lobbyBalanceText.text = $"<color=#94A3B8>PHP</color>  <b><color=#00FFAA>₱{PlayerManager.Instance.balance:N2}</color></b>";
            }
        }
    }

    private struct CardMetaItem
    {
        public string key;
        public string title;
        public string tag;
        public string sub;
        public string hexColor;
        public CardMetaItem(string k, string t, string tg, string s, string h)
        {
            key = k; title = t; tag = tg; sub = s; hexColor = h;
        }
    }

    public void StyleCredibleLobbyUI()
    {
        if (_isStyling) return;
        _isStyling = true;
        try
        {
        CanvasScaler scaler = FindAnyObjectByType<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            scaler.dynamicPixelsPerUnit = 2f;
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
                    lRt.anchorMax = new Vector2(0.5f, 0.5f);
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
                    ligRt.anchoredPosition = new Vector2(-30, 0);
                    ligRt.sizeDelta = new Vector2(660, 50);
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
                    balBg.gameObject.SetActive(true);
                    Image balImg = balBg.GetComponent<Image>();
                    if (balImg != null) balImg.color = new Color(0.075f, 0.098f, 0.153f, 1f);
                    RectTransform balRt = balBg.GetComponent<RectTransform>();
                    if (balRt != null)
                    {
                        balRt.sizeDelta = new Vector2(250, 46);
                    }
                    Text balTxt = balBg.GetComponentInChildren<Text>(true);
                    if (balTxt != null)
                    {
                        balTxt.gameObject.SetActive(true);
                        balTxt.fontSize = 19;
                        balTxt.color = Color.white;
                        balTxt.alignment = TextAnchor.MiddleCenter;
                        balTxt.supportRichText = true;
                        float curBal = PlayerManager.Instance != null ? PlayerManager.Instance.balance : 10000f;
                        balTxt.text = $"<color=#94A3B8>PHP</color>  <b><color=#00FFAA>₱{curBal:N2}</color></b>";
                        if (lobbyBalanceText == null) lobbyBalanceText = balTxt;
                    }
                }

                Button depBtn = loggedInGroup.Find("DepositBtn")?.GetComponent<Button>();
                if (depBtn != null)
                {
                    depBtn.gameObject.SetActive(true);
                    Image depImg = depBtn.GetComponent<Image>();
                    if (depImg != null) depImg.color = new Color(0.063f, 0.725f, 0.506f, 1f);
                    Text depTxt = depBtn.GetComponentInChildren<Text>(true);
                    if (depTxt != null)
                    {
                        depTxt.gameObject.SetActive(true);
                        depTxt.text = "<b>+ DEPOSIT</b>";
                        depTxt.fontSize = 16;
                        depTxt.color = Color.white;
                        depTxt.alignment = TextAnchor.MiddleCenter;
                        depTxt.raycastTarget = false;
                        RectTransform dtRt = depTxt.GetComponent<RectTransform>();
                        if (dtRt != null)
                        {
                            dtRt.anchorMin = Vector2.zero;
                            dtRt.anchorMax = Vector2.one;
                            dtRt.offsetMin = Vector2.zero;
                            dtRt.offsetMax = Vector2.zero;
                            dtRt.anchoredPosition = Vector2.zero;
                        }
                    }
                    RectTransform depRt = depBtn.GetComponent<RectTransform>();
                    if (depRt != null) depRt.sizeDelta = new Vector2(130, 46);
                    if (depBtn.GetComponent<ButtonHoverJuice>() == null) depBtn.gameObject.AddComponent<ButtonHoverJuice>();
                    depBtn.onClick.RemoveAllListeners();
                    depBtn.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowDeposit(); });
                }

                Button withBtn = loggedInGroup.Find("WithdrawBtn")?.GetComponent<Button>();
                if (withBtn != null)
                {
                    withBtn.gameObject.SetActive(true);
                    Image withImg = withBtn.GetComponent<Image>();
                    if (withImg != null) withImg.color = new Color(0.137f, 0.180f, 0.259f, 1f);
                    Text withTxt = withBtn.GetComponentInChildren<Text>(true);
                    if (withTxt != null)
                    {
                        withTxt.gameObject.SetActive(true);
                        withTxt.text = "<b>WITHDRAW</b>";
                        withTxt.fontSize = 16;
                        withTxt.color = Color.white;
                        withTxt.alignment = TextAnchor.MiddleCenter;
                        withTxt.raycastTarget = false;
                        RectTransform wtRt = withTxt.GetComponent<RectTransform>();
                        if (wtRt != null)
                        {
                            wtRt.anchorMin = Vector2.zero;
                            wtRt.anchorMax = Vector2.one;
                            wtRt.offsetMin = Vector2.zero;
                            wtRt.offsetMax = Vector2.zero;
                            wtRt.anchoredPosition = Vector2.zero;
                        }
                    }
                    RectTransform withRt = withBtn.GetComponent<RectTransform>();
                    if (withRt != null) withRt.sizeDelta = new Vector2(125, 46);
                    if (withBtn.GetComponent<ButtonHoverJuice>() == null) withBtn.gameObject.AddComponent<ButtonHoverJuice>();
                    withBtn.onClick.RemoveAllListeners();
                    withBtn.onClick.AddListener(() => { if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowWithdraw(); });
                }

                Button logoutBtn = loggedInGroup.Find("LogoutBtn")?.GetComponent<Button>();
                if (logoutBtn != null)
                {
                    logoutBtn.gameObject.SetActive(true);
                    Image logImg = logoutBtn.GetComponent<Image>();
                    if (logImg != null) logImg.color = new Color(0.18f, 0.22f, 0.30f, 1f);
                    Text logTxt = logoutBtn.GetComponentInChildren<Text>(true);
                    if (logTxt != null)
                    {
                        logTxt.gameObject.SetActive(true);
                        logTxt.text = "<b>LOG OUT</b>";
                        logTxt.fontSize = 14;
                        logTxt.color = new Color(0.85f, 0.88f, 0.94f);
                        logTxt.alignment = TextAnchor.MiddleCenter;
                        logTxt.raycastTarget = false;
                        RectTransform ltRt = logTxt.GetComponent<RectTransform>();
                        if (ltRt != null)
                        {
                            ltRt.anchorMin = Vector2.zero;
                            ltRt.anchorMax = Vector2.one;
                            ltRt.offsetMin = Vector2.zero;
                            ltRt.offsetMax = Vector2.zero;
                            ltRt.anchoredPosition = Vector2.zero;
                        }
                    }
                    RectTransform logRt = logoutBtn.GetComponent<RectTransform>();
                    if (logRt != null) logRt.sizeDelta = new Vector2(100, 46);
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

        EnsureCanvasBackground();

        GameObject contentArea = GameObject.Find("ContentArea");
        if (contentArea != null)
        {
            RectTransform caRt = contentArea.GetComponent<RectTransform>();
            if (caRt != null)
            {
                caRt.anchorMin = Vector2.zero;
                caRt.anchorMax = Vector2.one;
                caRt.offsetMin = Vector2.zero;
                caRt.offsetMax = new Vector2(0, -75);
            }
        }

        GameObject heroBanner = GameObject.Find("HeroBanner");
        if (heroBanner != null)
        {
            Image hbImg = heroBanner.GetComponent<Image>();
            if (hbImg != null) hbImg.color = new Color(0.071f, 0.098f, 0.153f, 1f);

            RectTransform hbRt = heroBanner.GetComponent<RectTransform>();
            if (hbRt != null)
            {
                hbRt.anchorMin = new Vector2(0, 1);
                hbRt.anchorMax = new Vector2(1, 1);
                hbRt.pivot = new Vector2(0.5f, 1);
                hbRt.offsetMin = new Vector2(50, -155);
                hbRt.offsetMax = new Vector2(-50, -15);
            }

            Text hTitle = heroBanner.transform.Find("HeroTitle")?.GetComponent<Text>();
            if (hTitle != null)
            {
                hTitle.text = "<b><color=#F8FAFC>MY CASINO GAME I MADE FOR FUN</color></b>";
                hTitle.fontSize = 24;
                hTitle.alignment = TextAnchor.MiddleCenter;
                hTitle.raycastTarget = false;
                RectTransform tRt = hTitle.GetComponent<RectTransform>();
                if (tRt != null) { tRt.anchoredPosition = new Vector2(0, 12); tRt.sizeDelta = new Vector2(1200, 36); }
            }

            Text hSub = heroBanner.transform.Find("HeroSub")?.GetComponent<Text>();
            if (hSub != null)
            {
                hSub.text = "<b><color=#EF4444>⚠️ NOT REAL MONEY</color>  •  <color=#10B981>Philippine Peso (PHP) Simulation</color>  •  <color=#94A3B8>Just for Fun</color></b>";
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
                sRt.offsetMin = new Vector2(50, 15);
                sRt.offsetMax = new Vector2(-50, -170);
            }
        }

        GameObject grid = GameObject.Find("Content");
        if (grid != null)
        {
            GridLayoutGroup layout = grid.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(330, 250);
                layout.spacing = new Vector2(24, 24);
                layout.padding = new RectOffset(15, 15, 10, 30);
                layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layout.constraintCount = 4;
                layout.childAlignment = TextAnchor.UpperCenter;
            }

            RectTransform gRt = grid.GetComponent<RectTransform>();
            if (gRt != null)
            {
                gRt.sizeDelta = new Vector2(0, 620);
            }
        }

        CardMetaItem[] cardItems = new CardMetaItem[]
        {
            new CardMetaItem("CRASH", "CRASH", "99% RTP", "Multiplier", "10B981"),
            new CardMetaItem("ROULETTE", "ROULETTE", "14X MAX", "European", "EF4444"),
            new CardMetaItem("SLOTS", "SLOTS", "JACKPOT", "3-Reel Classic", "A855F7"),
            new CardMetaItem("MINES", "MINES", "99% RTP", "Gem Grid", "10B981"),
            new CardMetaItem("PLINKO", "PLINKO", "1000X", "Peg Drop", "6366F1"),
            new CardMetaItem("DICE", "DICE", "FAIR", "Over / Under", "F59E0B"),
            new CardMetaItem("BLACKJACK", "BLACKJACK", "3:2 PAY", "Table 21", "059669")
        };

        for (int i = 0; i < cardItems.Length; i++)
        {
            var data = cardItems[i];
            string gKey = data.key;
            GameObject cardGo = GameObject.Find(gKey + "Thumb");
            if (cardGo == null) continue;

            if (cardGo.GetComponent<ButtonHoverJuice>() == null)
            {
                var juice = cardGo.AddComponent<ButtonHoverJuice>();
                juice.hoverScale = 1.04f;
                juice.clickScale = 0.96f;
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
                nameBg.gameObject.SetActive(true);
                RectTransform nbRt = nameBg.GetComponent<RectTransform>();
                if (nbRt != null)
                {
                    nbRt.anchorMin = new Vector2(0, 0);
                    nbRt.anchorMax = new Vector2(1, 0);
                    nbRt.pivot = new Vector2(0.5f, 0);
                    nbRt.anchoredPosition = Vector2.zero;
                    nbRt.sizeDelta = new Vector2(0, 42);
                }
                Image nbImg = nameBg.GetComponent<Image>();
                if (nbImg != null) nbImg.color = new Color(0.047f, 0.063f, 0.098f, 0.92f);

                Text nTxt = nameBg.GetComponentInChildren<Text>(true);
                if (nTxt != null)
                {
                    nTxt.gameObject.SetActive(true);
                    nTxt.text = $"<b><color=#{data.hexColor}><size=13>{data.tag}</size></color></b>  <color=#475569>•</color>  <color=#94A3B8><size=12>{data.sub}</size></color>";
                    nTxt.alignment = TextAnchor.MiddleCenter;
                    nTxt.raycastTarget = false;
                }
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
        finally
        {
            _isStyling = false;
        }
    }

    public void AutoWireButtons()
    {
        if (_buttonsWired) return;
        _buttonsWired = true;

        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include);
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
            else if (bName.Equals("Home", System.StringComparison.OrdinalIgnoreCase) ||
                     bName.Equals("TopBarBackBtn", System.StringComparison.OrdinalIgnoreCase) ||
                     bName.Equals("BackBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                b.onClick.RemoveAllListeners();
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

    public void EnsureCanvasBackground()
    {
        GameObject canvasGo = GameObject.Find("CasinoCanvas");
        if (canvasGo == null) return;

        Transform bgTr = canvasGo.transform.Find("CanvasBackground");
        if (bgTr == null)
        {
            GameObject bgGo = new GameObject("CanvasBackground", typeof(RectTransform));
            bgGo.transform.SetParent(canvasGo.transform, false);
            bgGo.transform.SetAsFirstSibling();
            Image img = bgGo.AddComponent<Image>();
            img.color = new Color(0.043f, 0.055f, 0.078f, 1f);
            img.raycastTarget = false;
            RectTransform rt = bgGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }

    private void EnsurePanelsFound()
    {
        Transform contentArea = null;
        GameObject canvas = GameObject.Find("CasinoCanvas");
        if (canvas != null) contentArea = canvas.transform.Find("ContentArea");

        if (!lobbyPanel) lobbyPanel = contentArea != null ? contentArea.Find("LobbyPanel")?.gameObject : GameObject.Find("LobbyPanel");
        if (!crashPanel) crashPanel = contentArea != null ? contentArea.Find("CrashPanel")?.gameObject : GameObject.Find("CrashPanel");
        if (!roulettePanel) roulettePanel = contentArea != null ? contentArea.Find("RoulettePanel")?.gameObject : GameObject.Find("RoulettePanel");
        if (!slotsPanel) slotsPanel = contentArea != null ? contentArea.Find("SlotsPanel")?.gameObject : GameObject.Find("SlotsPanel");
        if (!minesPanel) minesPanel = contentArea != null ? contentArea.Find("MinesPanel")?.gameObject : GameObject.Find("MinesPanel");
        if (!plinkoPanel) plinkoPanel = contentArea != null ? contentArea.Find("PlinkoPanel")?.gameObject : GameObject.Find("PlinkoPanel");
        if (!dicePanel) dicePanel = contentArea != null ? contentArea.Find("DicePanel")?.gameObject : GameObject.Find("DicePanel");
        if (!blackjackPanel) blackjackPanel = contentArea != null ? contentArea.Find("BlackjackPanel")?.gameObject : GameObject.Find("BlackjackPanel");
    }

    public void ShowLobby()
    {
        EnsurePanelsFound();
        if (lobbyPanel) lobbyPanel.SetActive(true);
        if (crashPanel) crashPanel.SetActive(false);
        if (roulettePanel) roulettePanel.SetActive(false);
        if (slotsPanel) slotsPanel.SetActive(false);
        if (minesPanel) minesPanel.SetActive(false);
        if (plinkoPanel) plinkoPanel.SetActive(false);
        if (dicePanel) dicePanel.SetActive(false);
        if (blackjackPanel) blackjackPanel.SetActive(false);

        SetupTopBarNavigation("LOBBY");
        StyleCredibleLobbyUI();
    }

    public void SetupTopBarNavigation(string currentView)
    {
        GameObject topBar = GameObject.Find("TopBar");
        if (topBar == null) return;

        Transform logoTr = topBar.transform.Find("Logo");
        Text logo = logoTr != null ? logoTr.GetComponent<Text>() : null;
        RectTransform lRt = logoTr != null ? logoTr.GetComponent<RectTransform>() : null;

        Transform backTr = topBar.transform.Find("TopBarBackBtn");
        GameObject backBtnGo;
        if (backTr == null)
        {
            backBtnGo = new GameObject("TopBarBackBtn", typeof(RectTransform));
            backBtnGo.transform.SetParent(topBar.transform, false);

            Image bImg = backBtnGo.AddComponent<Image>();
            bImg.color = new Color(0.122f, 0.161f, 0.216f, 1f);
            bImg.raycastTarget = true;

            GameObject bTxtGo = new GameObject("Text", typeof(RectTransform));
            bTxtGo.transform.SetParent(backBtnGo.transform, false);
            Text bTxt = bTxtGo.AddComponent<Text>();
            Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
            bTxt.font = f;
            bTxt.fontSize = 18;
            bTxt.fontStyle = FontStyle.Bold;
            bTxt.alignment = TextAnchor.MiddleCenter;
            bTxt.text = "<b><color=#10B981>←</color> LOBBY</b>";
            bTxt.color = Color.white;
            bTxt.raycastTarget = false;
            RectTransform btRt = bTxtGo.GetComponent<RectTransform>();
            btRt.anchorMin = Vector2.zero;
            btRt.anchorMax = Vector2.one;
            btRt.offsetMin = Vector2.zero;
            btRt.offsetMax = Vector2.zero;
        }
        else
        {
            backBtnGo = backTr.gameObject;
        }

        Button bBtn = backBtnGo.GetComponent<Button>();
        if (bBtn == null) bBtn = backBtnGo.AddComponent<Button>();
        bBtn.onClick.RemoveAllListeners();
        bBtn.onClick.AddListener(ShowLobby);

        Image btnImg = backBtnGo.GetComponent<Image>();
        if (btnImg != null)
        {
            btnImg.raycastTarget = true;
            btnImg.color = new Color(0.122f, 0.161f, 0.216f, 1f);
        }

        RectTransform bRt = backBtnGo.GetComponent<RectTransform>();
        if (bRt != null)
        {
            bRt.anchorMin = new Vector2(0, 0.5f);
            bRt.anchorMax = new Vector2(0, 0.5f);
            bRt.pivot = new Vector2(0, 0.5f);
            bRt.anchoredPosition = new Vector2(35, 0);
            bRt.sizeDelta = new Vector2(150, 48);
        }

        if (backBtnGo.GetComponent<ButtonHoverJuice>() == null)
        {
            backBtnGo.AddComponent<ButtonHoverJuice>();
        }

        backBtnGo.transform.SetAsLastSibling();

        bool isLobby = string.IsNullOrEmpty(currentView) || currentView.Equals("LOBBY", System.StringComparison.OrdinalIgnoreCase);
        backBtnGo.SetActive(!isLobby);

        if (logo != null && lRt != null)
        {
            logo.raycastTarget = false;
            if (isLobby)
            {
                lRt.anchoredPosition = new Vector2(40, 0);
                logo.text = "<b><size=26><color=#F59E0B>✦</color> <color=#FFFFFF>KENCHI'S CASINO</color>  <color=#EF4444><size=14>[NOT REAL MONEY]</size></color></size></b>";
            }
            else
            {
                lRt.anchoredPosition = new Vector2(205, 0);
                logo.text = $"<b><size=18><color=#94A3B8>FOR FUN  •  </color><color=#FFFFFF>{currentView}</color>  <color=#EF4444><size=13>[NOT REAL MONEY]</size></color></size></b>";
            }
        }
    }

    public void SetupGameHeaderAndBack(GameObject panel, string gameTitle)
    {
        if (panel == null) return;

        Transform existingHeader = panel.transform.Find("GameHeader");
        if (existingHeader != null)
        {
            if (Application.isPlaying) Destroy(existingHeader.gameObject);
            else DestroyImmediate(existingHeader.gameObject);
        }

        SetupTopBarNavigation(gameTitle);
        StyleGamePanel(panel, gameTitle);
    }

    public void StyleGamePanel(GameObject panel, string gameTitle)
    {
        if (panel == null) return;

        Image pImg = panel.GetComponent<Image>();
        if (pImg != null) pImg.color = new Color(0.043f, 0.055f, 0.078f, 1f);

        RectTransform pRt = panel.GetComponent<RectTransform>();
        if (pRt != null)
        {
            pRt.anchorMin = Vector2.zero;
            pRt.anchorMax = Vector2.one;
            pRt.offsetMin = Vector2.zero;
            pRt.offsetMax = new Vector2(0, -75);
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Transform betPanel = panel.transform.Find("BetPanel");
        if (betPanel != null)
        {
            betPanel.gameObject.SetActive(true);
            RectTransform bpRt = betPanel.GetComponent<RectTransform>();
            if (bpRt != null)
            {
                bpRt.anchorMin = new Vector2(0, 1);
                bpRt.anchorMax = new Vector2(1, 1);
                bpRt.pivot = new Vector2(0.5f, 1);
                bpRt.anchoredPosition = new Vector2(0, 0);
                bpRt.sizeDelta = new Vector2(0, 68);
            }

            Image bpImg = betPanel.GetComponent<Image>();
            if (bpImg != null) bpImg.color = new Color(0.059f, 0.082f, 0.125f, 0.95f);

            Transform betLabel = betPanel.Find("BetLabel");
            if (betLabel != null)
            {
                betLabel.gameObject.SetActive(true);
                RectTransform blRt = betLabel.GetComponent<RectTransform>();
                if (blRt != null)
                {
                    blRt.anchorMin = new Vector2(0, 0.5f);
                    blRt.anchorMax = new Vector2(0, 0.5f);
                    blRt.pivot = new Vector2(0, 0.5f);
                    blRt.anchoredPosition = new Vector2(30, 0);
                    blRt.sizeDelta = new Vector2(125, 48);
                }
                Text blTxt = betLabel.GetComponent<Text>();
                if (blTxt != null)
                {
                    blTxt.gameObject.SetActive(true);
                    blTxt.text = "<b>BET AMOUNT</b>";
                    blTxt.fontSize = 15;
                    blTxt.color = new Color(0.58f, 0.64f, 0.72f);
                }
            }

            Transform inputBg = betPanel.Find("InputBg");
            if (inputBg != null)
            {
                inputBg.gameObject.SetActive(true);
                RectTransform ibRt = inputBg.GetComponent<RectTransform>();
                if (ibRt != null)
                {
                    ibRt.anchorMin = new Vector2(0, 0.5f);
                    ibRt.anchorMax = new Vector2(0, 0.5f);
                    ibRt.pivot = new Vector2(0, 0.5f);
                    ibRt.anchoredPosition = new Vector2(165, 0);
                    ibRt.sizeDelta = new Vector2(280, 48);
                }
                Image ibImg = inputBg.GetComponent<Image>();
                if (ibImg != null) ibImg.color = new Color(0.102f, 0.141f, 0.208f, 1f);

                InputField inf = inputBg.GetComponent<InputField>();
                if (inf != null && inf.textComponent != null)
                {
                    inf.textComponent.gameObject.SetActive(true);
                    inf.textComponent.fontSize = 18;
                    inf.textComponent.fontStyle = FontStyle.Bold;
                    inf.textComponent.color = Color.white;
                    inf.textComponent.alignment = TextAnchor.MiddleLeft;
                    RectTransform itRt = inf.textComponent.GetComponent<RectTransform>();
                    if (itRt != null)
                    {
                        itRt.anchorMin = Vector2.zero;
                        itRt.anchorMax = Vector2.one;
                        itRt.offsetMin = new Vector2(12, 0);
                        itRt.offsetMax = new Vector2(-115, 0);
                    }
                }

                Transform halfBtn = inputBg.Find("HalfBtn");
                if (halfBtn != null)
                {
                    halfBtn.gameObject.SetActive(true);
                    RectTransform hRt = halfBtn.GetComponent<RectTransform>();
                    if (hRt != null)
                    {
                        hRt.anchorMin = new Vector2(1, 0.5f);
                        hRt.anchorMax = new Vector2(1, 0.5f);
                        hRt.pivot = new Vector2(1, 0.5f);
                        hRt.anchoredPosition = new Vector2(-58, 0);
                        hRt.sizeDelta = new Vector2(48, 36);
                    }
                    Image hImg = halfBtn.GetComponent<Image>();
                    if (hImg != null) hImg.color = new Color(0.16f, 0.22f, 0.32f, 1f);
                    Text ht = halfBtn.GetComponentInChildren<Text>(true);
                    if (ht != null)
                    {
                        ht.gameObject.SetActive(true);
                        ht.text = "<b>1/2</b>";
                        ht.fontSize = 14;
                        ht.color = Color.white;
                        ht.alignment = TextAnchor.MiddleCenter;
                        RectTransform htRt = ht.GetComponent<RectTransform>();
                        if (htRt != null)
                        {
                            htRt.anchorMin = Vector2.zero;
                            htRt.anchorMax = Vector2.one;
                            htRt.offsetMin = Vector2.zero;
                            htRt.offsetMax = Vector2.zero;
                        }
                    }
                }

                Transform doubleBtn = inputBg.Find("DoubleBtn");
                if (doubleBtn != null)
                {
                    doubleBtn.gameObject.SetActive(true);
                    RectTransform dRt = doubleBtn.GetComponent<RectTransform>();
                    if (dRt != null)
                    {
                        dRt.anchorMin = new Vector2(1, 0.5f);
                        dRt.anchorMax = new Vector2(1, 0.5f);
                        dRt.pivot = new Vector2(1, 0.5f);
                        dRt.anchoredPosition = new Vector2(-6, 0);
                        dRt.sizeDelta = new Vector2(48, 36);
                    }
                    Image dImg = doubleBtn.GetComponent<Image>();
                    if (dImg != null) dImg.color = new Color(0.16f, 0.22f, 0.32f, 1f);
                    Text dt = doubleBtn.GetComponentInChildren<Text>(true);
                    if (dt != null)
                    {
                        dt.gameObject.SetActive(true);
                        dt.text = "<b>2x</b>";
                        dt.fontSize = 14;
                        dt.color = Color.white;
                        dt.alignment = TextAnchor.MiddleCenter;
                        RectTransform dtRt = dt.GetComponent<RectTransform>();
                        if (dtRt != null)
                        {
                            dtRt.anchorMin = Vector2.zero;
                            dtRt.anchorMax = Vector2.one;
                            dtRt.offsetMin = Vector2.zero;
                            dtRt.offsetMax = Vector2.zero;
                        }
                    }
                }
            }

            Transform actionBtn = betPanel.Find("ActionBtn");
            if (actionBtn != null)
            {
                actionBtn.gameObject.SetActive(true);
                RectTransform abRt = actionBtn.GetComponent<RectTransform>();
                if (abRt != null)
                {
                    abRt.anchorMin = new Vector2(1, 0.5f);
                    abRt.anchorMax = new Vector2(1, 0.5f);
                    abRt.pivot = new Vector2(1, 0.5f);
                    abRt.anchoredPosition = new Vector2(-40, 0);
                    abRt.sizeDelta = new Vector2(240, 52);
                }
                Image abImg = actionBtn.GetComponent<Image>();
                if (abImg != null) abImg.color = new Color(0.063f, 0.725f, 0.506f, 1f);

                Text abTxt = actionBtn.GetComponentInChildren<Text>(true);
                if (abTxt != null)
                {
                    abTxt.gameObject.SetActive(true);
                    abTxt.color = Color.white;
                    abTxt.fontSize = 18;
                    abTxt.fontStyle = FontStyle.Bold;
                    abTxt.alignment = TextAnchor.MiddleCenter;
                    RectTransform abtRt = abTxt.GetComponent<RectTransform>();
                    if (abtRt != null)
                    {
                        abtRt.anchorMin = Vector2.zero;
                        abtRt.anchorMax = Vector2.one;
                        abtRt.offsetMin = Vector2.zero;
                        abtRt.offsetMax = Vector2.zero;
                    }
                    if (gameTitle == "BLACKJACK") abTxt.text = "<b>DEAL HAND</b>";
                    else if (gameTitle == "CRASH") abTxt.text = "<b>PLACE BET</b>";
                    else if (gameTitle == "ROULETTE") abTxt.text = "<b>SPIN WHEEL</b>";
                    else if (gameTitle == "SLOTS") abTxt.text = "<b>SPIN REELS</b>";
                    else if (gameTitle == "MINES") abTxt.text = "<b>START MINES</b>";
                    else if (gameTitle == "PLINKO") abTxt.text = "<b>DROP BALL</b>";
                    else if (gameTitle == "DICE") abTxt.text = "<b>ROLL DICE</b>";
                    else abTxt.text = "<b>BET</b>";
                }
                if (actionBtn.GetComponent<ButtonHoverJuice>() == null) actionBtn.gameObject.AddComponent<ButtonHoverJuice>();
            }
        }

        if (gameTitle == "CRASH")
        {
            Transform gc = panel.transform.Find("GraphContainer");
            if (gc != null)
            {
                RectTransform gcRt = gc.GetComponent<RectTransform>();
                if (gcRt != null)
                {
                    gcRt.anchorMin = new Vector2(0.5f, 0.5f);
                    gcRt.anchorMax = new Vector2(0.5f, 0.5f);
                    gcRt.pivot = new Vector2(0.5f, 0.5f);
                    gcRt.anchoredPosition = new Vector2(0, -45);
                    gcRt.sizeDelta = new Vector2(1100, 560);
                }
                Image gcImg = gc.GetComponent<Image>();
                if (gcImg != null) gcImg.color = new Color(0.043f, 0.059f, 0.086f, 1f);

                Transform mult = gc.Find("MultiplierText");
                if (mult != null)
                {
                    RectTransform mRt = mult.GetComponent<RectTransform>();
                    if (mRt != null)
                    {
                        mRt.anchorMin = new Vector2(0.5f, 0.5f);
                        mRt.anchorMax = new Vector2(0.5f, 0.5f);
                        mRt.pivot = new Vector2(0.5f, 0.5f);
                        mRt.anchoredPosition = new Vector2(0, 45);
                        mRt.sizeDelta = new Vector2(800, 110);
                    }
                    Text mTxt = mult.GetComponent<Text>();
                    if (mTxt != null) { mTxt.fontSize = 90; mTxt.alignment = TextAnchor.MiddleCenter; }
                }
                Transform st = gc.Find("StatusText");
                if (st != null)
                {
                    RectTransform sRt = st.GetComponent<RectTransform>();
                    if (sRt != null)
                    {
                        sRt.anchorMin = new Vector2(0.5f, 0.5f);
                        sRt.anchorMax = new Vector2(0.5f, 0.5f);
                        sRt.pivot = new Vector2(0.5f, 0.5f);
                        sRt.anchoredPosition = new Vector2(0, -45);
                        sRt.sizeDelta = new Vector2(600, 40);
                    }
                    Text sTxt = st.GetComponent<Text>();
                    if (sTxt != null) { sTxt.fontSize = 22; sTxt.alignment = TextAnchor.MiddleCenter; }
                }
            }
        }
        else if (gameTitle == "ROULETTE")
        {
            Transform st = panel.transform.Find("StatusText");
            if (st != null)
            {
                RectTransform sRt = st.GetComponent<RectTransform>();
                if (sRt != null)
                {
                    sRt.anchorMin = new Vector2(0.5f, 0.5f);
                    sRt.anchorMax = new Vector2(0.5f, 0.5f);
                    sRt.pivot = new Vector2(0.5f, 0.5f);
                    sRt.anchoredPosition = new Vector2(0, 130);
                    sRt.sizeDelta = new Vector2(600, 40);
                }
                Text sTxt = st.GetComponent<Text>();
                if (sTxt != null) { sTxt.fontSize = 24; sTxt.alignment = TextAnchor.MiddleCenter; }
            }
            Transform tb = panel.transform.Find("TickerBg");
            if (tb != null)
            {
                RectTransform tbRt = tb.GetComponent<RectTransform>();
                if (tbRt != null)
                {
                    tbRt.anchorMin = new Vector2(0.5f, 0.5f);
                    tbRt.anchorMax = new Vector2(0.5f, 0.5f);
                    tbRt.pivot = new Vector2(0.5f, 0.5f);
                    tbRt.anchoredPosition = new Vector2(0, 20);
                    tbRt.sizeDelta = new Vector2(900, 96);
                }
                Image tbImg = tb.GetComponent<Image>();
                if (tbImg != null) tbImg.color = new Color(0.035f, 0.051f, 0.078f, 1f);

                Transform cl = tb.Find("CenterLine");
                if (cl != null)
                {
                    RectTransform clRt = cl.GetComponent<RectTransform>();
                    if (clRt != null) { clRt.anchoredPosition = Vector2.zero; clRt.sizeDelta = new Vector2(4, 110); }
                    Image clImg = cl.GetComponent<Image>();
                    if (clImg != null) clImg.color = new Color(0.96f, 0.62f, 0.04f, 1f);
                }
            }
            Transform rb = panel.transform.Find("RButtonGroup");
            if (rb != null)
            {
                RectTransform rbRt = rb.GetComponent<RectTransform>();
                if (rbRt != null)
                {
                    rbRt.anchorMin = new Vector2(0.5f, 0.5f);
                    rbRt.anchorMax = new Vector2(0.5f, 0.5f);
                    rbRt.pivot = new Vector2(0.5f, 0.5f);
                    rbRt.anchoredPosition = new Vector2(0, -115);
                    rbRt.sizeDelta = new Vector2(860, 90);
                }
                Transform r = rb.Find("BetRed");
                if (r != null)
                {
                    r.gameObject.SetActive(true);
                    RectTransform rt = r.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
                        rt.anchoredPosition = new Vector2(-295, 0); rt.sizeDelta = new Vector2(260, 84);
                    }
                    Image rImg = r.GetComponent<Image>();
                    if (rImg != null) rImg.color = new Color(0.86f, 0.15f, 0.22f, 1f);
                    Text rTxt = r.GetComponentInChildren<Text>(true);
                    if (rTxt != null) { rTxt.gameObject.SetActive(true); rTxt.color = Color.white; }
                }
                Transform g = rb.Find("BetGreen");
                if (g != null)
                {
                    g.gameObject.SetActive(true);
                    RectTransform gt = g.GetComponent<RectTransform>();
                    if (gt != null)
                    {
                        gt.anchorMin = new Vector2(0.5f, 0.5f); gt.anchorMax = new Vector2(0.5f, 0.5f); gt.pivot = new Vector2(0.5f, 0.5f);
                        gt.anchoredPosition = new Vector2(0, 0); gt.sizeDelta = new Vector2(240, 84);
                    }
                    Image gImg = g.GetComponent<Image>();
                    if (gImg != null) gImg.color = new Color(0.05f, 0.75f, 0.45f, 1f);
                    Text gTxt = g.GetComponentInChildren<Text>(true);
                    if (gTxt != null) { gTxt.gameObject.SetActive(true); gTxt.color = Color.white; }
                }
                Transform b = rb.Find("BetBlack");
                if (b != null)
                {
                    b.gameObject.SetActive(true);
                    RectTransform bt = b.GetComponent<RectTransform>();
                    if (bt != null)
                    {
                        bt.anchorMin = new Vector2(0.5f, 0.5f); bt.anchorMax = new Vector2(0.5f, 0.5f); bt.pivot = new Vector2(0.5f, 0.5f);
                        bt.anchoredPosition = new Vector2(295, 0); bt.sizeDelta = new Vector2(260, 84);
                    }
                    Image bImg = b.GetComponent<Image>();
                    if (bImg != null) bImg.color = new Color(0.12f, 0.16f, 0.24f, 1f);
                    Text bTxt = b.GetComponentInChildren<Text>(true);
                    if (bTxt != null) { bTxt.gameObject.SetActive(true); bTxt.color = Color.white; }
                }
            }
        }
        else if (gameTitle == "SLOTS")
        {
            Transform st = panel.transform.Find("StatusText");
            if (st != null)
            {
                st.gameObject.SetActive(true);
                RectTransform sRt = st.GetComponent<RectTransform>();
                if (sRt != null)
                {
                    sRt.anchorMin = new Vector2(0.5f, 0.5f);
                    sRt.anchorMax = new Vector2(0.5f, 0.5f);
                    sRt.pivot = new Vector2(0.5f, 0.5f);
                    sRt.anchoredPosition = new Vector2(0, 160);
                    sRt.sizeDelta = new Vector2(700, 46);
                }
                Text sTxt = st.GetComponent<Text>();
                if (sTxt != null)
                {
                    sTxt.gameObject.SetActive(true);
                    sTxt.fontSize = 26;
                    sTxt.fontStyle = FontStyle.Bold;
                    sTxt.color = Color.white;
                    sTxt.alignment = TextAnchor.MiddleCenter;
                }
            }
            Transform reels = panel.transform.Find("ReelsBg");
            if (reels != null)
            {
                reels.gameObject.SetActive(true);
                RectTransform rRt = reels.GetComponent<RectTransform>();
                if (rRt != null)
                {
                    rRt.anchorMin = new Vector2(0.5f, 0.5f);
                    rRt.anchorMax = new Vector2(0.5f, 0.5f);
                    rRt.pivot = new Vector2(0.5f, 0.5f);
                    rRt.anchoredPosition = new Vector2(0, -30);
                    rRt.sizeDelta = new Vector2(800, 260);
                }
                Image rImg = reels.GetComponent<Image>();
                if (rImg != null) rImg.color = new Color(0.059f, 0.082f, 0.125f, 1f);

                string[] rNames = { "Reel1Bg", "Reel2Bg", "Reel3Bg" };
                float[] xPos = { -255f, 0f, 255f };
                string[] defaultIcons = {
                    "<size=52>🍒</size>\n<size=16><color=#F43F5E><b>CHERRY</b></color></size>",
                    "<size=52>🔥</size>\n<size=16><color=#F97316><b>7 7 7</b></color></size>",
                    "<size=52>💎</size>\n<size=16><color=#38BDF8><b>DIAMOND</b></color></size>"
                };
                for (int i = 0; i < 3; i++)
                {
                    Transform reelBox = reels.Find(rNames[i]);
                    if (reelBox != null)
                    {
                        reelBox.gameObject.SetActive(true);
                        RectTransform rboxRt = reelBox.GetComponent<RectTransform>();
                        if (rboxRt != null)
                        {
                            rboxRt.anchorMin = new Vector2(0.5f, 0.5f);
                            rboxRt.anchorMax = new Vector2(0.5f, 0.5f);
                            rboxRt.pivot = new Vector2(0.5f, 0.5f);
                            rboxRt.anchoredPosition = new Vector2(xPos[i], 0);
                            rboxRt.sizeDelta = new Vector2(230, 230);
                        }
                        Image rboxImg = reelBox.GetComponent<Image>();
                        if (rboxImg != null) rboxImg.color = new Color(0.09f, 0.12f, 0.18f, 1f);

                        Text reelTxt = reelBox.GetComponentInChildren<Text>(true);
                        if (reelTxt != null)
                        {
                            reelTxt.gameObject.SetActive(true);
                            reelTxt.fontSize = 26;
                            reelTxt.color = Color.white;
                            reelTxt.alignment = TextAnchor.MiddleCenter;
                            reelTxt.supportRichText = true;
                            if (string.IsNullOrEmpty(reelTxt.text)) reelTxt.text = defaultIcons[i];
                            RectTransform rt = reelTxt.GetComponent<RectTransform>();
                            if (rt != null)
                            {
                                rt.anchorMin = Vector2.zero;
                                rt.anchorMax = Vector2.one;
                                rt.offsetMin = Vector2.zero;
                                rt.offsetMax = Vector2.zero;
                            }
                        }
                    }
                }
            }
        }
        else if (gameTitle == "MINES")
        {
            Transform grid = panel.transform.Find("GridContainer");
            if (grid != null)
            {
                grid.gameObject.SetActive(true);
                RectTransform gRt = grid.GetComponent<RectTransform>();
                if (gRt != null)
                {
                    gRt.anchorMin = new Vector2(0.5f, 0.5f);
                    gRt.anchorMax = new Vector2(0.5f, 0.5f);
                    gRt.pivot = new Vector2(0.5f, 0.5f);
                    gRt.anchoredPosition = new Vector2(0, -15);
                    gRt.sizeDelta = new Vector2(590, 590);
                    gRt.localScale = new Vector3(0.68f, 0.68f, 1f);
                }
                foreach (Transform tile in grid)
                {
                    tile.gameObject.SetActive(true);
                    Text tt = tile.GetComponentInChildren<Text>(true);
                    if (tt != null) tt.gameObject.SetActive(true);
                }
            }

            Transform mult = panel.transform.Find("MultiplierText");
            if (mult != null)
            {
                mult.gameObject.SetActive(true);
                RectTransform mRt = mult.GetComponent<RectTransform>();
                if (mRt != null)
                {
                    mRt.anchorMin = new Vector2(0.5f, 0.5f);
                    mRt.anchorMax = new Vector2(0.5f, 0.5f);
                    mRt.pivot = new Vector2(0.5f, 0.5f);
                    mRt.anchoredPosition = new Vector2(0, 270);
                    mRt.sizeDelta = new Vector2(600, 48);
                }
                Text mTxt = mult.GetComponent<Text>();
                if (mTxt != null) { mTxt.gameObject.SetActive(true); mTxt.fontSize = 42; mTxt.fontStyle = FontStyle.Bold; mTxt.alignment = TextAnchor.MiddleCenter; mTxt.color = new Color(0f, 1f, 0.67f); }
                mult.SetAsLastSibling();
            }

            Transform st = panel.transform.Find("StatusText");
            if (st != null)
            {
                st.gameObject.SetActive(true);
                RectTransform sRt = st.GetComponent<RectTransform>();
                if (sRt != null)
                {
                    sRt.anchorMin = new Vector2(0.5f, 0.5f);
                    sRt.anchorMax = new Vector2(0.5f, 0.5f);
                    sRt.pivot = new Vector2(0.5f, 0.5f);
                    sRt.anchoredPosition = new Vector2(0, 222);
                    sRt.sizeDelta = new Vector2(600, 36);
                }
                Text sTxt = st.GetComponent<Text>();
                if (sTxt != null) { sTxt.gameObject.SetActive(true); sTxt.fontSize = 20; sTxt.fontStyle = FontStyle.Bold; sTxt.alignment = TextAnchor.MiddleCenter; }
                st.SetAsLastSibling();
            }

            Transform ml = panel.transform.Find("MinesLabel");
            if (ml != null)
            {
                ml.gameObject.SetActive(true);
                RectTransform mlRt = ml.GetComponent<RectTransform>();
                if (mlRt != null)
                {
                    mlRt.anchorMin = new Vector2(0.5f, 0.5f);
                    mlRt.anchorMax = new Vector2(0.5f, 0.5f);
                    mlRt.pivot = new Vector2(0.5f, 0.5f);
                    mlRt.anchoredPosition = new Vector2(-75, -270);
                    mlRt.sizeDelta = new Vector2(140, 36);
                }
                Text mlTxt = ml.GetComponent<Text>();
                if (mlTxt != null) { mlTxt.gameObject.SetActive(true); mlTxt.fontSize = 15; mlTxt.fontStyle = FontStyle.Bold; mlTxt.color = new Color(0.6f, 0.65f, 0.75f); mlTxt.text = "<b>MINES COUNT:</b>"; }
                ml.SetAsLastSibling();
            }

            Transform mib = panel.transform.Find("MinesInputBg");
            if (mib != null)
            {
                mib.gameObject.SetActive(true);
                RectTransform mibRt = mib.GetComponent<RectTransform>();
                if (mibRt != null)
                {
                    mibRt.anchorMin = new Vector2(0.5f, 0.5f);
                    mibRt.anchorMax = new Vector2(0.5f, 0.5f);
                    mibRt.pivot = new Vector2(0.5f, 0.5f);
                    mibRt.anchoredPosition = new Vector2(50, -270);
                    mibRt.sizeDelta = new Vector2(90, 42);
                }
                Text mibTxt = mib.GetComponentInChildren<Text>(true);
                if (mibTxt != null) mibTxt.gameObject.SetActive(true);
                mib.SetAsLastSibling();
            }
        }
        else if (gameTitle == "DICE")
        {
            Transform res = panel.transform.Find("ResultText");
            if (res != null)
            {
                res.gameObject.SetActive(true);
                RectTransform rRt = res.GetComponent<RectTransform>();
                if (rRt != null)
                {
                    rRt.anchorMin = new Vector2(0.5f, 0.5f); rRt.anchorMax = new Vector2(0.5f, 0.5f); rRt.pivot = new Vector2(0.5f, 0.5f);
                    rRt.anchoredPosition = new Vector2(0, 140); rRt.sizeDelta = new Vector2(400, 70);
                }
                Text rTxt = res.GetComponent<Text>();
                if (rTxt != null) { rTxt.gameObject.SetActive(true); rTxt.fontSize = 72; rTxt.fontStyle = FontStyle.Bold; rTxt.alignment = TextAnchor.MiddleCenter; }
            }
            Transform sb = panel.transform.Find("SliderBg");
            if (sb != null)
            {
                sb.gameObject.SetActive(true);
                RectTransform sRt = sb.GetComponent<RectTransform>();
                if (sRt != null)
                {
                    sRt.anchorMin = new Vector2(0.5f, 0.5f); sRt.anchorMax = new Vector2(0.5f, 0.5f); sRt.pivot = new Vector2(0.5f, 0.5f);
                    sRt.anchoredPosition = new Vector2(0, 40); sRt.sizeDelta = new Vector2(780, 32);
                }
            }
            Transform target = panel.transform.Find("TargetText");
            if (target != null)
            {
                target.gameObject.SetActive(true);
                RectTransform tRt = target.GetComponent<RectTransform>();
                if (tRt != null)
                {
                    tRt.anchorMin = new Vector2(0.5f, 0.5f); tRt.anchorMax = new Vector2(0.5f, 0.5f); tRt.pivot = new Vector2(0.5f, 0.5f);
                    tRt.anchoredPosition = new Vector2(-250, -25);
                }
                Text tTxt = target.GetComponent<Text>();
                if (tTxt != null) tTxt.gameObject.SetActive(true);
            }
            Transform chance = panel.transform.Find("ChanceText");
            if (chance != null)
            {
                chance.gameObject.SetActive(true);
                RectTransform cRt = chance.GetComponent<RectTransform>();
                if (cRt != null)
                {
                    cRt.anchorMin = new Vector2(0.5f, 0.5f); cRt.anchorMax = new Vector2(0.5f, 0.5f); cRt.pivot = new Vector2(0.5f, 0.5f);
                    cRt.anchoredPosition = new Vector2(0, -25);
                }
                Text cTxt = chance.GetComponent<Text>();
                if (cTxt != null) cTxt.gameObject.SetActive(true);
            }
            Transform mult = panel.transform.Find("MultText");
            if (mult != null)
            {
                mult.gameObject.SetActive(true);
                RectTransform mRt = mult.GetComponent<RectTransform>();
                if (mRt != null)
                {
                    mRt.anchorMin = new Vector2(0.5f, 0.5f); mRt.anchorMax = new Vector2(0.5f, 0.5f); mRt.pivot = new Vector2(0.5f, 0.5f);
                    mRt.anchoredPosition = new Vector2(250, -25);
                }
                Text mTxt = mult.GetComponent<Text>();
                if (mTxt != null) mTxt.gameObject.SetActive(true);
            }
            Transform tog = panel.transform.Find("ToggleOverUnderBtn");
            if (tog != null)
            {
                tog.gameObject.SetActive(true);
                RectTransform togRt = tog.GetComponent<RectTransform>();
                if (togRt != null)
                {
                    togRt.anchorMin = new Vector2(0.5f, 0.5f); togRt.anchorMax = new Vector2(0.5f, 0.5f); togRt.pivot = new Vector2(0.5f, 0.5f);
                    togRt.anchoredPosition = new Vector2(0, -95); togRt.sizeDelta = new Vector2(220, 48);
                }
                Text tgTxt = tog.GetComponentInChildren<Text>(true);
                if (tgTxt != null) tgTxt.gameObject.SetActive(true);
            }
        }
        else if (gameTitle == "PLINKO")
        {
            Transform ticker = panel.transform.Find("TickerBg");
            if (ticker != null) ticker.gameObject.SetActive(false);

            Transform bc = panel.transform.Find("BoardContainer");
            if (bc != null)
            {
                bc.gameObject.SetActive(true);
                RectTransform bRt = bc.GetComponent<RectTransform>();
                if (bRt != null)
                {
                    bRt.anchorMin = new Vector2(0.5f, 0.5f);
                    bRt.anchorMax = new Vector2(0.5f, 0.5f);
                    bRt.pivot = new Vector2(0.5f, 0.5f);
                    bRt.anchoredPosition = new Vector2(0, -65);
                    bRt.sizeDelta = new Vector2(800, 700);
                    bRt.localScale = new Vector3(0.72f, 0.72f, 1f);
                }

                Sprite circleSprite = GetCircleSprite();
                Image[] pegImgs = bc.GetComponentsInChildren<Image>(true);
                foreach (var pi in pegImgs)
                {
                    pi.gameObject.SetActive(true);
                    if (pi.name.StartsWith("Peg_"))
                    {
                        if (circleSprite != null) pi.sprite = circleSprite;
                        pi.color = Color.white;
                        RectTransform pegRt = pi.GetComponent<RectTransform>();
                        if (pegRt != null) pegRt.sizeDelta = new Vector2(24, 24);
                    }
                }
                Text[] slotTexts = bc.GetComponentsInChildren<Text>(true);
                foreach (var stx in slotTexts) stx.gameObject.SetActive(true);
            }

            Transform st = panel.transform.Find("StatusText");
            if (st != null)
            {
                st.gameObject.SetActive(true);
                RectTransform sRt = st.GetComponent<RectTransform>();
                if (sRt != null)
                {
                    sRt.anchorMin = new Vector2(0.5f, 0.5f);
                    sRt.anchorMax = new Vector2(0.5f, 0.5f);
                    sRt.pivot = new Vector2(0.5f, 0.5f);
                    sRt.anchoredPosition = new Vector2(0, 275);
                    sRt.sizeDelta = new Vector2(700, 42);
                }
                Text sTxt = st.GetComponent<Text>();
                if (sTxt != null) { sTxt.gameObject.SetActive(true); sTxt.fontSize = 24; sTxt.fontStyle = FontStyle.Bold; sTxt.alignment = TextAnchor.MiddleCenter; }
                st.SetAsLastSibling();
            }
        }
        else if (gameTitle == "BLACKJACK")
        {
            Transform st = panel.transform.Find("StatusText");
            if (st != null)
            {
                st.gameObject.SetActive(true);
                RectTransform sRt = st.GetComponent<RectTransform>();
                if (sRt != null)
                {
                    sRt.anchorMin = new Vector2(0.5f, 0.5f); sRt.anchorMax = new Vector2(0.5f, 0.5f); sRt.pivot = new Vector2(0.5f, 0.5f);
                    sRt.anchoredPosition = new Vector2(0, 260); sRt.sizeDelta = new Vector2(700, 46);
                }
                Text sTxt = st.GetComponent<Text>();
                if (sTxt != null) { sTxt.fontSize = 24; sTxt.fontStyle = FontStyle.Bold; sTxt.alignment = TextAnchor.MiddleCenter; sTxt.supportRichText = true; }
            }

            Transform dt = panel.transform.Find("DealerTitle");
            if (dt != null)
            {
                dt.gameObject.SetActive(true);
                RectTransform dtRt = dt.GetComponent<RectTransform>();
                if (dtRt != null)
                {
                    dtRt.anchorMin = new Vector2(0.5f, 0.5f); dtRt.anchorMax = new Vector2(0.5f, 0.5f);
                    dtRt.pivot = new Vector2(1f, 0.5f);
                    dtRt.anchoredPosition = new Vector2(-15, 195);
                    dtRt.sizeDelta = new Vector2(200, 38);
                }
                Text dtTxt = dt.GetComponent<Text>();
                if (dtTxt != null) { dtTxt.fontSize = 20; dtTxt.fontStyle = FontStyle.Bold; dtTxt.color = new Color(0.58f, 0.64f, 0.72f); dtTxt.alignment = TextAnchor.MiddleRight; dtTxt.text = "<b>DEALER</b>"; }
            }

            Transform ds = panel.transform.Find("DealerScore");
            if (ds != null)
            {
                ds.gameObject.SetActive(true);
                RectTransform dsRt = ds.GetComponent<RectTransform>();
                if (dsRt != null)
                {
                    dsRt.anchorMin = new Vector2(0.5f, 0.5f); dsRt.anchorMax = new Vector2(0.5f, 0.5f);
                    dsRt.pivot = new Vector2(0f, 0.5f);
                    dsRt.anchoredPosition = new Vector2(15, 195);
                    dsRt.sizeDelta = new Vector2(200, 38);
                }
                Text dsTxt = ds.GetComponent<Text>();
                if (dsTxt != null) { dsTxt.fontSize = 20; dsTxt.fontStyle = FontStyle.Bold; dsTxt.alignment = TextAnchor.MiddleLeft; dsTxt.supportRichText = true; }
            }

            Transform dc = panel.transform.Find("DealerCards");
            if (dc != null)
            {
                dc.gameObject.SetActive(true);
                RectTransform dRt = dc.GetComponent<RectTransform>();
                if (dRt != null)
                {
                    dRt.anchorMin = new Vector2(0.5f, 0.5f); dRt.anchorMax = new Vector2(0.5f, 0.5f); dRt.pivot = new Vector2(0.5f, 0.5f);
                    dRt.anchoredPosition = new Vector2(0, 120); dRt.sizeDelta = new Vector2(800, 75);
                }
                Text dcTxt = dc.GetComponent<Text>();
                if (dcTxt != null) { dcTxt.fontSize = 44; dcTxt.fontStyle = FontStyle.Bold; dcTxt.alignment = TextAnchor.MiddleCenter; dcTxt.supportRichText = true; }
            }

            Transform pt = panel.transform.Find("PlayerTitle");
            if (pt != null)
            {
                pt.gameObject.SetActive(true);
                RectTransform ptRt = pt.GetComponent<RectTransform>();
                if (ptRt != null)
                {
                    ptRt.anchorMin = new Vector2(0.5f, 0.5f); ptRt.anchorMax = new Vector2(0.5f, 0.5f);
                    ptRt.pivot = new Vector2(1f, 0.5f);
                    ptRt.anchoredPosition = new Vector2(-15, 15);
                    ptRt.sizeDelta = new Vector2(200, 38);
                }
                Text ptTxt = pt.GetComponent<Text>();
                if (ptTxt != null) { ptTxt.fontSize = 20; ptTxt.fontStyle = FontStyle.Bold; ptTxt.color = new Color(0.58f, 0.64f, 0.72f); ptTxt.alignment = TextAnchor.MiddleRight; ptTxt.text = "<b>PLAYER</b>"; }
            }

            Transform ps = panel.transform.Find("PlayerScore");
            if (ps != null)
            {
                ps.gameObject.SetActive(true);
                RectTransform psRt = ps.GetComponent<RectTransform>();
                if (psRt != null)
                {
                    psRt.anchorMin = new Vector2(0.5f, 0.5f); psRt.anchorMax = new Vector2(0.5f, 0.5f);
                    psRt.pivot = new Vector2(0f, 0.5f);
                    psRt.anchoredPosition = new Vector2(15, 15);
                    psRt.sizeDelta = new Vector2(200, 38);
                }
                Text psTxt = ps.GetComponent<Text>();
                if (psTxt != null) { psTxt.fontSize = 20; psTxt.fontStyle = FontStyle.Bold; psTxt.alignment = TextAnchor.MiddleLeft; psTxt.supportRichText = true; }
            }

            Transform pc = panel.transform.Find("PlayerCards");
            if (pc != null)
            {
                pc.gameObject.SetActive(true);
                RectTransform pCardsRt = pc.GetComponent<RectTransform>();
                if (pCardsRt != null)
                {
                    pCardsRt.anchorMin = new Vector2(0.5f, 0.5f); pCardsRt.anchorMax = new Vector2(0.5f, 0.5f); pCardsRt.pivot = new Vector2(0.5f, 0.5f);
                    pCardsRt.anchoredPosition = new Vector2(0, -55); pCardsRt.sizeDelta = new Vector2(800, 75);
                }
                Text pcTxt = pc.GetComponent<Text>();
                if (pcTxt != null) { pcTxt.fontSize = 44; pcTxt.fontStyle = FontStyle.Bold; pcTxt.alignment = TextAnchor.MiddleCenter; pcTxt.supportRichText = true; }
            }

            Transform hit = panel.transform.Find("HitBtn");
            if (hit != null)
            {
                hit.gameObject.SetActive(true);
                RectTransform hRt = hit.GetComponent<RectTransform>();
                if (hRt != null)
                {
                    hRt.anchorMin = new Vector2(0.5f, 0.5f); hRt.anchorMax = new Vector2(0.5f, 0.5f); hRt.pivot = new Vector2(0.5f, 0.5f);
                    hRt.anchoredPosition = new Vector2(-125, -155); hRt.sizeDelta = new Vector2(220, 56);
                }
                Image hImg = hit.GetComponent<Image>();
                if (hImg != null) hImg.color = new Color(0.145f, 0.388f, 0.922f, 1f);
                Text hTxt = hit.GetComponentInChildren<Text>();
                if (hTxt != null) { hTxt.fontSize = 22; hTxt.fontStyle = FontStyle.Bold; hTxt.text = "<b>HIT</b>"; }
                if (hit.GetComponent<ButtonHoverJuice>() == null) hit.gameObject.AddComponent<ButtonHoverJuice>();
            }

            Transform stand = panel.transform.Find("StandBtn");
            if (stand != null)
            {
                stand.gameObject.SetActive(true);
                RectTransform sRt = stand.GetComponent<RectTransform>();
                if (sRt != null)
                {
                    sRt.anchorMin = new Vector2(0.5f, 0.5f); sRt.anchorMax = new Vector2(0.5f, 0.5f); sRt.pivot = new Vector2(0.5f, 0.5f);
                    sRt.anchoredPosition = new Vector2(125, -155); sRt.sizeDelta = new Vector2(220, 56);
                }
                Image sImg = stand.GetComponent<Image>();
                if (sImg != null) sImg.color = new Color(0.85f, 0.47f, 0.02f, 1f);
                Text stxt = stand.GetComponentInChildren<Text>();
                if (stxt != null) { stxt.fontSize = 22; stxt.fontStyle = FontStyle.Bold; stxt.text = "<b>STAND</b>"; }
                if (stand.GetComponent<ButtonHoverJuice>() == null) stand.gameObject.AddComponent<ButtonHoverJuice>();
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

    private static Sprite circleSpriteCache;
    public static Sprite GetCircleSprite()
    {
        if (circleSpriteCache != null) return circleSpriteCache;

        circleSpriteCache = Resources.Load<Sprite>("CirclePeg");
        if (circleSpriteCache != null) return circleSpriteCache;

        circleSpriteCache = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
        if (circleSpriteCache != null) return circleSpriteCache;

        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        float rSq = r * r;
        Color[] cols = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - r + 0.5f;
                float dy = y - r + 0.5f;
                float distSq = dx * dx + dy * dy;
                if (distSq <= (r - 1.5f) * (r - 1.5f))
                {
                    cols[y * size + x] = Color.white;
                }
                else if (distSq <= rSq)
                {
                    float alpha = Mathf.Clamp01((r - Mathf.Sqrt(distSq)) / 1.5f);
                    cols[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    cols[y * size + x] = Color.clear;
                }
            }
        }

        tex.SetPixels(cols);
        tex.Apply();
        circleSpriteCache = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        return circleSpriteCache;
    }
}
