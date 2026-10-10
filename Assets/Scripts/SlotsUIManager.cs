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
        "<color=#FF4444>APPLE</color>", 
        "<color=#FFFF00>LEMON</color>", 
        "<color=#FF0055>CHERRY</color>", 
        "<color=#FF5500><b>7 7 7</b></color>", 
        "<color=#00FFFF><b>DIAMOND</b></color>" 
    };
    void Start()
    {
        if (SlotsGameManager.Instance != null)
        {
            SlotsGameManager.Instance.OnStateChanged += UpdateState;
            SlotsGameManager.Instance.OnSpinResult += SetFinalReels;
            SlotsGameManager.Instance.OnPayout += ShowPayout;
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
        if (spinButton != null)
        {
            spinButton.onClick.AddListener(() => {
                if (SlotsGameManager.Instance.currentState == SlotsGameManager.GameState.Idle)
                {
                    SlotsGameManager.Instance.Spin(betAmount);
                }
            });
        }
    }
    void UpdateBetDisplay()
    {
        if (betAmountInput != null) betAmountInput.text = betAmount.ToString("F2");
        if (SlotsGameManager.Instance.currentState == SlotsGameManager.GameState.Idle)
        {
            spinButton.transform.GetChild(0).GetComponent<Text>().text = $"SPIN ${betAmount:F2}";
        }
    }
    void Update()
    {
        if (PlayerManager.Instance != null && balanceText != null)
        {
            balanceText.text = $"<color=#888888>BALANCE</color>\n<color=#00FFAA><size=50>${PlayerManager.Instance.balance:F2}</size></color>";
        }
    }
    void UpdateState(SlotsGameManager.GameState state)
    {
        spinButton.interactable = state == SlotsGameManager.GameState.Idle;
        halfBetBtn.interactable = state == SlotsGameManager.GameState.Idle;
        doubleBetBtn.interactable = state == SlotsGameManager.GameState.Idle;
        betAmountInput.interactable = state == SlotsGameManager.GameState.Idle;
        if (state == SlotsGameManager.GameState.Idle)
        {
            statusText.text = "READY TO SPIN!";
            statusText.color = Color.white;
            spinButton.transform.GetChild(0).GetComponent<Text>().text = $"SPIN ${betAmount:F2}";
            spinButton.GetComponent<Image>().color = new Color(0.1f, 0.8f, 0.3f);
        }
        else if (state == SlotsGameManager.GameState.Spinning)
        {
            statusText.text = "<color=#FFFF00>SPINNING...</color>";
            spinButton.transform.GetChild(0).GetComponent<Text>().text = "GOOD LUCK!";
            spinButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
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
            if (CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
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
            if (CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
            statusText.text = $"<color=#00FFAA>YOU WON ${payout}!</color>";
        }
        else 
        {
            statusText.text = $"<color=#FF4444>NO WIN. TRY AGAIN.</color>";
        }
    }
}
