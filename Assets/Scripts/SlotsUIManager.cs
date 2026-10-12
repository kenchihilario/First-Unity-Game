using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class SlotsUIManager : MonoBehaviour
{
    public Text[] reels = new Text[3];
    public Text statusText;
    public Text balanceText;
    public Button spinButton;
    public InputField betAmountInput;
    public Button halfBetBtn;
    public Button doubleBetBtn;
    private float betAmount = 10f;
    private string[] symbolEmojis = new string[] {
        "<size=54>🍎</size>\n<size=18><color=#EF4444><b>APPLE</b></color></size>",
        "<size=54>🍋</size>\n<size=18><color=#FACC15><b>LEMON</b></color></size>",
        "<size=54>🍒</size>\n<size=18><color=#F43F5E><b>CHERRY</b></color></size>",
        "<size=54>🔥</size>\n<size=18><color=#F97316><b>7 7 7</b></color></size>",
        "<size=54>💎</size>\n<size=18><color=#38BDF8><b>DIAMOND</b></color></size>"
    };
    void Start()
    {
        EnsureReferences();
        if (SlotsGameManager.Instance != null)
        {
            SlotsGameManager.Instance.OnStateChanged -= UpdateState;
            SlotsGameManager.Instance.OnStateChanged += UpdateState;
            SlotsGameManager.Instance.OnSpinResult -= SetFinalReels;
            SlotsGameManager.Instance.OnSpinResult += SetFinalReels;
            SlotsGameManager.Instance.OnPayout -= ShowPayout;
            SlotsGameManager.Instance.OnPayout += ShowPayout;
        }
        if (betAmountInput != null)
        {
            betAmountInput.gameObject.SetActive(true);
            betAmountInput.onEndEdit.RemoveAllListeners();
            betAmountInput.onEndEdit.AddListener(val => {
                if (float.TryParse(val, out float res)) betAmount = Mathf.Max(1f, res);
                UpdateBetDisplay();
            });
        }
        if (halfBetBtn != null)
        {
            halfBetBtn.gameObject.SetActive(true);
            halfBetBtn.onClick.RemoveAllListeners();
            halfBetBtn.onClick.AddListener(() => { betAmount = Mathf.Max(1f, Mathf.Floor(betAmount / 2f)); UpdateBetDisplay(); });
        }
        if (doubleBetBtn != null)
        {
            doubleBetBtn.gameObject.SetActive(true);
            doubleBetBtn.onClick.RemoveAllListeners();
            doubleBetBtn.onClick.AddListener(() => { betAmount = Mathf.Floor(betAmount * 2f); UpdateBetDisplay(); });
        }
        UpdateBetDisplay();
        if (spinButton != null)
        {
            spinButton.gameObject.SetActive(true);
            spinButton.onClick.RemoveAllListeners();
            spinButton.onClick.AddListener(() => {
                if (SlotsGameManager.Instance != null && SlotsGameManager.Instance.currentState == SlotsGameManager.GameState.Idle)
                {
                    SlotsGameManager.Instance.Spin(betAmount);
                }
            });
        }
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = "READY TO SPIN!";
            statusText.color = Color.white;
        }
        for (int i = 0; i < 3; i++)
        {
            if (reels[i] != null)
            {
                reels[i].gameObject.SetActive(true);
                if (reels[i].transform.parent != null)
                {
                    reels[i].transform.parent.gameObject.SetActive(true);
                }
                reels[i].supportRichText = true;
                reels[i].fontSize = 28;
                reels[i].alignment = TextAnchor.MiddleCenter;
                reels[i].text = symbolEmojis[(i + 2) % 5];
            }
        }
    }
    void EnsureReferences()
    {
        if (statusText == null)
        {
            Transform st = transform.Find("StatusText");
            if (st != null) statusText = st.GetComponent<Text>();
        }
        if (betAmountInput == null)
        {
            Transform inf = transform.Find("BetPanel/InputBg");
            if (inf != null) betAmountInput = inf.GetComponent<InputField>();
        }
        if (halfBetBtn == null)
        {
            Transform hb = transform.Find("BetPanel/InputBg/HalfBtn");
            if (hb != null) halfBetBtn = hb.GetComponent<Button>();
        }
        if (doubleBetBtn == null)
        {
            Transform db = transform.Find("BetPanel/InputBg/DoubleBtn");
            if (db != null) doubleBetBtn = db.GetComponent<Button>();
        }
        if (spinButton == null)
        {
            Transform sb = transform.Find("BetPanel/ActionBtn");
            if (sb != null) spinButton = sb.GetComponent<Button>();
        }
        if (reels == null || reels.Length < 3 || reels[0] == null)
        {
            Transform rBg = transform.Find("ReelsBg");
            if (rBg != null)
            {
                reels = new Text[3];
                string[] rNames = { "Reel1Bg/Reel1", "Reel2Bg/Reel2", "Reel3Bg/Reel3" };
                for (int i = 0; i < 3; i++)
                {
                    Transform t = rBg.Find(rNames[i]);
                    if (t != null) reels[i] = t.GetComponent<Text>();
                }
            }
        }
    }
    void OnDestroy()
    {
        if (SlotsGameManager.Instance != null)
        {
            SlotsGameManager.Instance.OnStateChanged -= UpdateState;
            SlotsGameManager.Instance.OnSpinResult -= SetFinalReels;
            SlotsGameManager.Instance.OnPayout -= ShowPayout;
        }
    }
    void UpdateBetDisplay()
    {
        if (betAmountInput != null) betAmountInput.text = betAmount.ToString("F2");
        if (spinButton != null && spinButton.transform.childCount > 0)
        {
            if (SlotsGameManager.Instance == null || SlotsGameManager.Instance.currentState == SlotsGameManager.GameState.Idle)
            {
                Text t = spinButton.transform.GetChild(0).GetComponent<Text>();
                if (t != null) t.text = $"SPIN ₱{betAmount:F2}";
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
    void UpdateState(SlotsGameManager.GameState state)
    {
        if (spinButton != null) spinButton.interactable = state == SlotsGameManager.GameState.Idle;
        if (halfBetBtn != null) halfBetBtn.interactable = state == SlotsGameManager.GameState.Idle;
        if (doubleBetBtn != null) doubleBetBtn.interactable = state == SlotsGameManager.GameState.Idle;
        if (betAmountInput != null) betAmountInput.interactable = state == SlotsGameManager.GameState.Idle;
        if (state == SlotsGameManager.GameState.Idle)
        {
            if (statusText != null)
            {
                statusText.text = "READY TO SPIN!";
                statusText.color = Color.white;
            }
            if (spinButton != null && spinButton.transform.childCount > 0)
            {
                Text t = spinButton.transform.GetChild(0).GetComponent<Text>();
                if (t != null) t.text = $"SPIN ₱{betAmount:F2}";
                Image img = spinButton.GetComponent<Image>();
                if (img != null) img.color = new Color(0.1f, 0.8f, 0.3f);
            }
        }
        else if (state == SlotsGameManager.GameState.Spinning)
        {
            if (statusText != null) statusText.text = "<color=#FFFF00>SPINNING...</color>";
            if (spinButton != null && spinButton.transform.childCount > 0)
            {
                Text t = spinButton.transform.GetChild(0).GetComponent<Text>();
                if (t != null) t.text = "GOOD LUCK!";
                Image img = spinButton.GetComponent<Image>();
                if (img != null) img.color = new Color(0.5f, 0.5f, 0.5f);
            }
            if (!isSpinning) StartCoroutine(SpinAnimation());
        }
    }
    private bool isSpinning = false;
    IEnumerator SpinAnimation()
    {
        isSpinning = true;
        while (isSpinning)
        {
            for (int i = 0; i < 3; i++)
            {
                if (reels[i] != null)
                {
                    reels[i].text = symbolEmojis[Random.Range(0, 5)];
                    reels[i].transform.localPosition = new Vector3(0, Random.Range(-20f, 20f), 0);
                    reels[i].transform.localScale = new Vector3(1f, 1.3f, 1f);
                }
            }
            if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
            yield return new WaitForSeconds(0.04f);
        }
    }
    void SetFinalReels(int[] result)
    {
        isSpinning = false;
        StartCoroutine(SnapReel(0, result[0], 0f));
        StartCoroutine(SnapReel(1, result[1], 0.2f));
        StartCoroutine(SnapReel(2, result[2], 0.4f));
    }
    IEnumerator SnapReel(int index, int resultSymbol, float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        if (reels[index] == null) yield break;
        reels[index].text = symbolEmojis[resultSymbol];
        reels[index].transform.localScale = Vector3.one;

        Vector3 startPos = new Vector3(0, 80f, 0);
        Vector3 endPos = Vector3.zero;

        float duration = 0.4f;
        float elapsed = 0f;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float ease = Mathf.Sin(-13f * (t + 1) * Mathf.PI * 0.5f) * Mathf.Pow(2f, -10f * t) + 1f;
            reels[index].transform.localPosition = Vector3.LerpUnclamped(startPos, endPos, ease);
            yield return null;
        }
        reels[index].transform.localPosition = endPos;
    }
    void ShowPayout(float payout)
    {
        if (payout > 0)
        {
            if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
            statusText.text = $"<color=#00FFAA>YOU WON ₱{payout:N2}!</color>";
        }
        else
        {
            if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayLoss();
            statusText.text = $"<color=#FF4444>NO WIN. TRY AGAIN.</color>";
        }
    }
}
