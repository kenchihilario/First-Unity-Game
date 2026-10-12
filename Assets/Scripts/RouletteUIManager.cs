using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RouletteUIManager : MonoBehaviour
{
    public Text statusText;
    public Text balanceText;
    public Button betRedBtn;
    public Button betBlackBtn;
    public Button betGreenBtn;
    public RectTransform tickerContainer;
    public InputField betAmountInput;
    public Button halfBetBtn;
    public Button doubleBetBtn;

    private float betAmount = 10f;
    private Color redColor = new Color(0.88f, 0.11f, 0.28f);
    private Color blackColor = new Color(0.09f, 0.12f, 0.18f);
    private Color greenColor = new Color(0.06f, 0.78f, 0.45f);
    private Image[] blocks = new Image[50];
    private float blockWidth = 80f;

    void Start()
    {
        if (RouletteGameManager.Instance != null)
        {
            RouletteGameManager.Instance.OnStateChanged -= UpdateState;
            RouletteGameManager.Instance.OnStateChanged += UpdateState;
            RouletteGameManager.Instance.OnTimerUpdated -= UpdateTimer;
            RouletteGameManager.Instance.OnTimerUpdated += UpdateTimer;
            RouletteGameManager.Instance.OnSpinResult -= SpinTicker;
            RouletteGameManager.Instance.OnSpinResult += SpinTicker;
        }

        if (tickerContainer == null)
        {
            Transform t = transform.Find("TickerBg/TickerContainer") ?? transform.Find("TickerContainer");
            if (t != null) tickerContainer = t.GetComponent<RectTransform>();
        }
        if (statusText == null) statusText = transform.Find("StatusText")?.GetComponent<Text>();
        if (betRedBtn == null) betRedBtn = transform.Find("RButtonGroup/BetRed")?.GetComponent<Button>() ?? transform.Find("BetRedBtn")?.GetComponent<Button>() ?? transform.Find("BetPanel/BetRedBtn")?.GetComponent<Button>();
        if (betBlackBtn == null) betBlackBtn = transform.Find("RButtonGroup/BetBlack")?.GetComponent<Button>() ?? transform.Find("BetBlackBtn")?.GetComponent<Button>() ?? transform.Find("BetPanel/BetBlackBtn")?.GetComponent<Button>();
        if (betGreenBtn == null) betGreenBtn = transform.Find("RButtonGroup/BetGreen")?.GetComponent<Button>() ?? transform.Find("BetGreenBtn")?.GetComponent<Button>() ?? transform.Find("BetPanel/BetGreenBtn")?.GetComponent<Button>();

        Sprite uiSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        if (uiSprite != null)
        {
            if (betRedBtn != null && betRedBtn.image != null) { betRedBtn.image.sprite = uiSprite; betRedBtn.image.type = Image.Type.Sliced; }
            if (betBlackBtn != null && betBlackBtn.image != null) { betBlackBtn.image.sprite = uiSprite; betBlackBtn.image.type = Image.Type.Sliced; }
            if (betGreenBtn != null && betGreenBtn.image != null) { betGreenBtn.image.sprite = uiSprite; betGreenBtn.image.type = Image.Type.Sliced; }
        }
        if (betAmountInput == null) betAmountInput = transform.Find("BetPanel/BetInput")?.GetComponent<InputField>() ?? GetComponentInChildren<InputField>(true);
        if (halfBetBtn == null) halfBetBtn = transform.Find("BetPanel/HalfBtn")?.GetComponent<Button>();
        if (doubleBetBtn == null) doubleBetBtn = transform.Find("BetPanel/DoubleBtn")?.GetComponent<Button>();

        if (betAmountInput != null)
        {
            betAmountInput.onEndEdit.RemoveAllListeners();
            betAmountInput.onEndEdit.AddListener(val => {
                if (float.TryParse(val, out float res)) betAmount = Mathf.Max(1f, res);
                UpdateBetDisplay();
            });
        }
        if (halfBetBtn != null)
        {
            halfBetBtn.onClick.RemoveAllListeners();
            halfBetBtn.onClick.AddListener(() => { betAmount = Mathf.Max(1f, Mathf.Floor(betAmount / 2f)); UpdateBetDisplay(); });
        }
        if (doubleBetBtn != null)
        {
            doubleBetBtn.onClick.RemoveAllListeners();
            doubleBetBtn.onClick.AddListener(() => { betAmount = Mathf.Floor(betAmount * 2f); UpdateBetDisplay(); });
        }

        UpdateBetDisplay();

        if (betRedBtn != null)
        {
            betRedBtn.onClick.RemoveAllListeners();
            betRedBtn.onClick.AddListener(() => {
                if (RouletteGameManager.Instance != null) RouletteGameManager.Instance.PlaceBet(1, betAmount);
                Text t = betRedBtn.GetComponentInChildren<Text>();
                if (t != null) t.text = $"<b><size=20>BET PLACED</size></b>\n<size=13>₱{betAmount:F2}</size>";
            });
        }
        if (betBlackBtn != null)
        {
            betBlackBtn.onClick.RemoveAllListeners();
            betBlackBtn.onClick.AddListener(() => {
                if (RouletteGameManager.Instance != null) RouletteGameManager.Instance.PlaceBet(2, betAmount);
                Text t = betBlackBtn.GetComponentInChildren<Text>();
                if (t != null) t.text = $"<b><size=20>BET PLACED</size></b>\n<size=13>₱{betAmount:F2}</size>";
            });
        }
        if (betGreenBtn != null)
        {
            betGreenBtn.onClick.RemoveAllListeners();
            betGreenBtn.onClick.AddListener(() => {
                if (RouletteGameManager.Instance != null) RouletteGameManager.Instance.PlaceBet(0, betAmount);
                Text t = betGreenBtn.GetComponentInChildren<Text>();
                if (t != null) t.text = $"<b><size=20>BET PLACED</size></b>\n<size=13>₱{betAmount:F2}</size>";
            });
        }

        SetupTickerBlocks();
    }

    void OnDestroy()
    {
        if (RouletteGameManager.Instance != null)
        {
            RouletteGameManager.Instance.OnStateChanged -= UpdateState;
            RouletteGameManager.Instance.OnTimerUpdated -= UpdateTimer;
            RouletteGameManager.Instance.OnSpinResult -= SpinTicker;
        }
    }

    void SetupTickerBlocks()
    {
        if (tickerContainer == null) return;
        foreach (Transform child in tickerContainer) Destroy(child.gameObject);
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        for (int i = 0; i < 50; i++)
        {
            GameObject go = new GameObject("Block_" + i);
            go.transform.SetParent(tickerContainer, false);
            Image img = go.AddComponent<Image>();
            int type = Random.Range(0, 15);
            if (type == 0) img.color = greenColor;
            else if (type <= 7) img.color = redColor;
            else img.color = blackColor;
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(blockWidth - 4f, 0);
            rt.anchoredPosition = new Vector2(i * blockWidth + (blockWidth/2f), 0);

            GameObject numGo = new GameObject("Num", typeof(RectTransform));
            numGo.transform.SetParent(go.transform, false);
            Text numTxt = numGo.AddComponent<Text>();
            numTxt.font = font;
            numTxt.fontSize = 24;
            numTxt.fontStyle = FontStyle.Bold;
            numTxt.alignment = TextAnchor.MiddleCenter;
            numTxt.text = type.ToString();
            numTxt.color = Color.white;
            numTxt.raycastTarget = false;
            RectTransform nRt = numGo.GetComponent<RectTransform>();
            nRt.anchorMin = Vector2.zero;
            nRt.anchorMax = Vector2.one;
            nRt.offsetMin = Vector2.zero;
            nRt.offsetMax = Vector2.zero;

            blocks[i] = img;
        }
    }

    void UpdateBetDisplay()
    {
        if (betAmountInput != null) betAmountInput.text = betAmount.ToString("F2");
        if (RouletteGameManager.Instance != null && RouletteGameManager.Instance.currentState == RouletteGameManager.GameState.Betting)
        {
            if (betRedBtn != null)
            {
                Text t = betRedBtn.GetComponentInChildren<Text>();
                if (t != null) t.text = $"<b><size=22>2X</size></b>\n<size=14>RED (1-7)</size>\n<size=12><color=#FECACA>BET ₱{betAmount:F2}</color></size>";
            }
            if (betBlackBtn != null)
            {
                Text t = betBlackBtn.GetComponentInChildren<Text>();
                if (t != null) t.text = $"<b><size=22>2X</size></b>\n<size=14>BLACK (8-14)</size>\n<size=12><color=#CBD5E1>BET ₱{betAmount:F2}</color></size>";
            }
            if (betGreenBtn != null)
            {
                Text t = betGreenBtn.GetComponentInChildren<Text>();
                if (t != null) t.text = $"<b><size=22>14X</size></b>\n<size=14>GREEN (0)</size>\n<size=12><color=#A7F3D0>BET ₱{betAmount:F2}</color></size>";
            }
        }
    }

    void Update()
    {
        if (PlayerManager.Instance != null && balanceText != null)
        {
            balanceText.text = $"<color=#888888>BALANCE</color>\n<color=#00FFAA><size=50>₱{PlayerManager.Instance.balance:N2}</size></color>";
        }
    }

    void UpdateState(RouletteGameManager.GameState state)
    {
        if (betRedBtn) betRedBtn.interactable = state == RouletteGameManager.GameState.Betting;
        if (betBlackBtn) betBlackBtn.interactable = state == RouletteGameManager.GameState.Betting;
        if (betGreenBtn) betGreenBtn.interactable = state == RouletteGameManager.GameState.Betting;
        if (halfBetBtn) halfBetBtn.interactable = state == RouletteGameManager.GameState.Betting;
        if (doubleBetBtn) doubleBetBtn.interactable = state == RouletteGameManager.GameState.Betting;
        if (betAmountInput) betAmountInput.interactable = state == RouletteGameManager.GameState.Betting;
        if (state == RouletteGameManager.GameState.Betting)
        {
            UpdateBetDisplay();
            if (tickerContainer != null)
            {
                tickerContainer.anchoredPosition = Vector2.zero;
                SetupTickerBlocks();
            }
        }
    }

    void UpdateTimer(float time)
    {
        if (statusText != null)
        {
            statusText.text = $"ROLLING IN {time:F1}s";
            statusText.color = Color.white;
        }
    }

    void SpinTicker(int result)
    {
        if (statusText != null) statusText.text = "<color=#00FFAA>SPINNING!</color>";
        if (!isActiveAndEnabled) return;
        StopAllCoroutines();
        StartCoroutine(AnimateSpin(result));
    }

    IEnumerator AnimateSpin(int result)
    {
        float duration = 4f;
        float elapsed = 0f;
        int targetIndex = 40;
        Color winColor = result == 0 ? greenColor : (result <= 7 ? redColor : blackColor);
        if (blocks != null && targetIndex < blocks.Length && blocks[targetIndex] != null)
        {
            blocks[targetIndex].color = winColor;
        }
        float targetX = -((targetIndex * blockWidth) - 400f);
        targetX += Random.Range(-30f, 30f);
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = new Vector2(targetX, 0);
        float lastTick = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f);
            if (tickerContainer != null) tickerContainer.anchoredPosition = Vector2.Lerp(startPos, endPos, easeT);
            if (gameObject.activeInHierarchy && elapsed - lastTick > 0.08f + (t * 0.18f))
            {
                lastTick = elapsed;
                if (CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
            }
            yield return null;
        }
        if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
        if (statusText != null) statusText.text = result == 0 ? "<color=#00FFAA>GREEN WINS (14x)</color>" : (result <= 7 ? "<color=#FF4444>RED WINS (2x)</color>" : "<color=#AAAAAA>BLACK WINS (2x)</color>");

        float bounceDuration = 0.5f;
        elapsed = 0f;
        while(elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
            if (blocks != null && targetIndex < blocks.Length && blocks[targetIndex] != null)
            {
                blocks[targetIndex].transform.localScale = Vector3.one * scale;
            }
            yield return null;
        }
        if (blocks != null && targetIndex < blocks.Length && blocks[targetIndex] != null)
        {
            blocks[targetIndex].transform.localScale = Vector3.one;
        }
    }
}
