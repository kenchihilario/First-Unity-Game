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
            lobbyBalanceText.text = $"<color=#6B7280>USD</color>  <b><color=#00FFAA>${PlayerManager.Instance.balance:F2}</color></b>";
        }
    }

    public void StyleCredibleLobbyUI()
    {

        if (lobbyPanel != null)
        {
            Image lpImg = lobbyPanel.GetComponent<Image>();
            if (lpImg != null) lpImg.color = new Color(0.043f, 0.055f, 0.078f, 1f);
        }

        GameObject topBar = GameObject.Find("TopBar");
        if (topBar != null)
        {
            Image tbImg = topBar.GetComponent<Image>();
            if (tbImg != null) tbImg.color = new Color(0.059f, 0.082f, 0.133f, 1f);

            Text logo = topBar.transform.Find("Logo")?.GetComponent<Text>();
            if (logo != null)
            {
                logo.text = "<b><color=#F59E0B>✦</color> <color=#FFFFFF>KENCHI</color> <color=#00FFAA>CASINO</color></b>";
                logo.raycastTarget = false;
            }

            Button depBtn = topBar.transform.Find("LoggedInGroup/DepositBtn")?.GetComponent<Button>();
            if (depBtn != null)
            {
                Image depImg = depBtn.GetComponent<Image>();
                if (depImg != null) depImg.color = new Color(0.063f, 0.725f, 0.506f, 1f);
                Text depTxt = depBtn.GetComponentInChildren<Text>();
                if (depTxt != null) { depTxt.text = "<b>+ DEPOSIT</b>"; depTxt.raycastTarget = false; }
                if (depBtn.GetComponent<ButtonHoverJuice>() == null) depBtn.gameObject.AddComponent<ButtonHoverJuice>();
            }

            Button withBtn = topBar.transform.Find("LoggedInGroup/WithdrawBtn")?.GetComponent<Button>();
            if (withBtn != null)
            {
                Image withImg = withBtn.GetComponent<Image>();
                if (withImg != null) withImg.color = new Color(0.122f, 0.161f, 0.216f, 1f);
                Text withTxt = withBtn.GetComponentInChildren<Text>();
                if (withTxt != null) { withTxt.text = "<b>WITHDRAW</b>"; withTxt.raycastTarget = false; }
                RectTransform withRt = withBtn.GetComponent<RectTransform>();
                if (withRt != null) withRt.sizeDelta = new Vector2(160, 80);
                if (withBtn.GetComponent<ButtonHoverJuice>() == null) withBtn.gameObject.AddComponent<ButtonHoverJuice>();
            }

            Transform balBg = topBar.transform.Find("LoggedInGroup/BalanceBg");
            if (balBg != null)
            {
                Image balImg = balBg.GetComponent<Image>();
                if (balImg != null) balImg.color = new Color(0.067f, 0.094f, 0.153f, 1f);
                RectTransform balRt = balBg.GetComponent<RectTransform>();
                if (balRt != null) balRt.sizeDelta = new Vector2(280, 80);
            }
        }

        GameObject heroBanner = GameObject.Find("HeroBanner");
        if (heroBanner != null)
        {
            Image hbImg = heroBanner.GetComponent<Image>();
            if (hbImg != null) hbImg.color = new Color(0.067f, 0.094f, 0.153f, 1f);

            RectTransform hbRt = heroBanner.GetComponent<RectTransform>();
            if (hbRt != null)
            {
                hbRt.anchorMin = new Vector2(0, 1);
                hbRt.anchorMax = new Vector2(1, 1);
                hbRt.pivot = new Vector2(0.5f, 1);
                hbRt.offsetMin = new Vector2(30, -250);
                hbRt.offsetMax = new Vector2(-30, -15);
            }

            Text hTitle = heroBanner.transform.Find("HeroTitle")?.GetComponent<Text>();
            if (hTitle != null)
            {
                hTitle.text = "<b>KENCHI CASINO</b>";
                hTitle.fontSize = 50;
                hTitle.alignment = TextAnchor.MiddleCenter;
                hTitle.raycastTarget = false;
            }

            Text hSub = heroBanner.transform.Find("HeroSub")?.GetComponent<Text>();
            if (hSub != null)
            {
                hSub.text = "<b><color=#FF4444>HOUSE ALWAYS WINS</color> • <color=#F59E0B>PERSONAL GITHUB PROJECT</color> • <color=#94A3B8>JUST MADE FOR FUN</color></b>";
                hSub.fontSize = 24;
                hSub.alignment = TextAnchor.MiddleCenter;
                hSub.raycastTarget = false;
                RectTransform subRt = hSub.GetComponent<RectTransform>();
                if (subRt != null) subRt.sizeDelta = new Vector2(1000, 60);
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
                sRt.offsetMin = new Vector2(0, 160);
                sRt.offsetMax = new Vector2(0, -270);
            }
        }

        GameObject grid = GameObject.Find("Content");
        if (grid != null)
        {
            GridLayoutGroup layout = grid.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(340, 420);
                layout.spacing = new Vector2(28, 28);
                layout.padding = new RectOffset(30, 30, 20, 50);
                layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layout.constraintCount = 4;
                layout.childAlignment = TextAnchor.UpperCenter;
            }

            RectTransform gRt = grid.GetComponent<RectTransform>();
            if (gRt != null)
            {
                gRt.sizeDelta = new Vector2(0, 1200);
            }
        }

        var cardMeta = new Dictionary<string, (string title, string sub)>()
        {
            { "CRASH", ("CRASH", "Multiplier") },
            { "ROULETTE", ("ROULETTE", "European Wheel") },
            { "SLOTS", ("SLOTS", "3-Reel Classic") },
            { "MINES", ("MINES", "Grid Game") },
            { "PLINKO", ("PLINKO", "Peg Drop") },
            { "DICE", ("DICE", "Roll 1-100") },
            { "BLACKJACK", ("BLACKJACK", "Table 21") }
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
                juice.hoverScale = 1.04f;
                juice.clickScale = 0.96f;
                juice.speed = 18f;
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
                Image bgImg = nameBg.GetComponent<Image>();
                if (bgImg != null)
                {
                    bgImg.color = new Color(0.051f, 0.075f, 0.122f, 0.95f);
                    bgImg.raycastTarget = false;
                }

                RectTransform nbRt = nameBg.GetComponent<RectTransform>();
                if (nbRt != null)
                {
                    nbRt.anchorMin = new Vector2(0, 0);
                    nbRt.anchorMax = new Vector2(1, 0.24f);
                    nbRt.pivot = new Vector2(0.5f, 0);
                    nbRt.offsetMin = Vector2.zero;
                    nbRt.offsetMax = Vector2.zero;
                }

                Text nameText = nameBg.GetComponentInChildren<Text>();
                if (nameText != null)
                {
                    nameText.text = $"<b><size=28>{data.title}</size></b>\n<size=18>{data.sub}</size>";
                    nameText.alignment = TextAnchor.MiddleCenter;
                    nameText.color = Color.white;
                    nameText.raycastTarget = false;
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
            Image bbImg = bottomBar.GetComponent<Image>();
            if (bbImg != null) bbImg.color = new Color(0.043f, 0.055f, 0.078f, 0.98f);

            RectTransform bbRt = bottomBar.GetComponent<RectTransform>();
            if (bbRt != null) bbRt.sizeDelta = new Vector2(0, 150);

            var navBtns = bottomBar.GetComponentsInChildren<Button>(true);
            foreach (var nb in navBtns)
            {
                Image nbImg = nb.GetComponent<Image>();
                if (nbImg != null) nbImg.color = new Color(0.086f, 0.125f, 0.173f, 1f);

                Text txt = nb.GetComponentInChildren<Text>();
                if (txt != null)
                {
                    txt.color = Color.white;
                    txt.raycastTarget = false;
                }
                if (nb.GetComponent<ButtonHoverJuice>() == null) nb.gameObject.AddComponent<ButtonHoverJuice>();
            }
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

        Sprite res = Resources.Load<Sprite>($"Thumbs/{lowerName}");
        if (res != null) return res;

        Texture2D resTex = Resources.Load<Texture2D>($"Thumbs/{lowerName}");
        if (resTex != null)
        {
            return Sprite.Create(resTex, new Rect(0, 0, resTex.width, resTex.height), new Vector2(0.5f, 0.5f));
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

    private bool CheckLogin()
    {
        if (AuthManager.Instance != null && !AuthManager.Instance.IsLoggedIn)
        {
            AuthManager.Instance.AutoLogin();
        }
        return true;
    }

    public void StartCrash()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(true);
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
        if(roulettePanel) roulettePanel.SetActive(true);
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
        if(slotsPanel) slotsPanel.SetActive(true);
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
        if(minesPanel) minesPanel.SetActive(true);
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
        if(plinkoPanel) plinkoPanel.SetActive(true);
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
        if(dicePanel) dicePanel.SetActive(true);
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
        if(blackjackPanel) blackjackPanel.SetActive(true);
    }
}
