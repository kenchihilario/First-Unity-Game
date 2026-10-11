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
    private Color redColor = new Color(1f, 0.13f, 0.33f);
    private Color blackColor = new Color(0.17f, 0.19f, 0.25f);
    private Color greenColor = new Color(0f, 0.9f, 0.0f);
    private Image[] blocks = new Image[50];
    private float blockWidth = 80f;
    void Start()
    {
        if (RouletteGameManager.Instance != null)
        {
            RouletteGameManager.Instance.OnStateChanged += UpdateState;
            RouletteGameManager.Instance.OnTimerUpdated += UpdateTimer;
            RouletteGameManager.Instance.OnSpinResult += SpinTicker;
        }
        if (betAmountInput != null)
        {
            betAmountInput.onEndEdit.AddListener(val => {
                if (float.TryParse(val, out float res)) betAmount = Mathf.Max(1f, res);
                UpdateBetDisplay();
            });
        }
        if (halfBetBtn != null) halfBetBtn.onClick.AddListener(() => { betAmount = Mathf.Max(1f, Mathf.Floor(betAmount / 2f)); UpdateBetDisplay(); });
        if (doubleBetBtn != null) doubleBetBtn.onClick.AddListener(() => { betAmount = Mathf.Floor(betAmount * 2f); UpdateBetDisplay(); });
        UpdateBetDisplay();
        betRedBtn.onClick.AddListener(() => { RouletteGameManager.Instance.PlaceBet(1, betAmount); betRedBtn.transform.GetChild(0).GetComponent<Text>().text = "BET PLACED"; });
        betBlackBtn.onClick.AddListener(() => { RouletteGameManager.Instance.PlaceBet(2, betAmount); betBlackBtn.transform.GetChild(0).GetComponent<Text>().text = "BET PLACED"; });
        betGreenBtn.onClick.AddListener(() => { RouletteGameManager.Instance.PlaceBet(0, betAmount); betGreenBtn.transform.GetChild(0).GetComponent<Text>().text = "BET PLACED"; });
        SetupTickerBlocks();
    }
    void SetupTickerBlocks()
    {
        foreach (Transform child in tickerContainer) Destroy(child.gameObject);
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
            blocks[i] = img;
        }
    }
    void UpdateBetDisplay()
    {
        if (betAmountInput != null) betAmountInput.text = betAmount.ToString("F2");
        if (RouletteGameManager.Instance.currentState == RouletteGameManager.GameState.Betting)
        {
            betRedBtn.transform.GetChild(0).GetComponent<Text>().text = $"<b>2X</b>\n<size=18>BET ${betAmount:F2}</size>";
            betBlackBtn.transform.GetChild(0).GetComponent<Text>().text = $"<b>2X</b>\n<size=18>BET ${betAmount:F2}</size>";
            betGreenBtn.transform.GetChild(0).GetComponent<Text>().text = $"<b>14X</b>\n<size=18>BET ${betAmount:F2}</size>";
        }
    }
    void Update()
    {
        if (PlayerManager.Instance != null && balanceText != null)
        {
            balanceText.text = $"<color=#888888>BALANCE</color>\n<color=#00FFAA><size=50>${PlayerManager.Instance.balance:F2}</size></color>";
        }
    }
    void UpdateState(RouletteGameManager.GameState state)
    {
        betRedBtn.interactable = state == RouletteGameManager.GameState.Betting;
        betBlackBtn.interactable = state == RouletteGameManager.GameState.Betting;
        betGreenBtn.interactable = state == RouletteGameManager.GameState.Betting;
        halfBetBtn.interactable = state == RouletteGameManager.GameState.Betting;
        doubleBetBtn.interactable = state == RouletteGameManager.GameState.Betting;
        betAmountInput.interactable = state == RouletteGameManager.GameState.Betting;
        if (state == RouletteGameManager.GameState.Betting)
        {
            UpdateBetDisplay();
            tickerContainer.anchoredPosition = Vector2.zero;
            SetupTickerBlocks();
        }
    }
    void UpdateTimer(float time)
    {
        statusText.text = $"ROLLING IN {time:F1}s";
        statusText.color = Color.white;
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
        blocks[targetIndex].color = winColor;
        float targetX = -((targetIndex * blockWidth) - 400f);
        targetX += Random.Range(-30f, 30f);
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = new Vector2(targetX, 0);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f);
            tickerContainer.anchoredPosition = Vector2.Lerp(startPos, endPos, easeT);
            yield return null;
        }
        if (CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
        statusText.text = result == 0 ? "<color=#00FFAA>GREEN WINS (14x)</color>" : (result <= 7 ? "<color=#FF4444>RED WINS (2x)</color>" : "<color=#AAAAAA>BLACK WINS (2x)</color>");

        float bounceDuration = 0.5f;
        elapsed = 0f;
        while(elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
            blocks[targetIndex].transform.localScale = Vector3.one * scale;
            yield return null;
        }
        blocks[targetIndex].transform.localScale = Vector3.one;
    }
}
