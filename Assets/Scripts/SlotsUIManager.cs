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
            if (reels[0] != null) reels[0].text = symbolEmojis[Random.Range(0, 5)];
            if (reels[1] != null) reels[1].text = symbolEmojis[Random.Range(0, 5)];
            if (reels[2] != null) reels[2].text = symbolEmojis[Random.Range(0, 5)];
            float scale = 1f + Mathf.PingPong(Time.time * 5f, 0.2f);
            if (reels[0] != null) reels[0].transform.localScale = Vector3.one * scale;
            if (reels[1] != null) reels[1].transform.localScale = Vector3.one * scale;
            if (reels[2] != null) reels[2].transform.localScale = Vector3.one * scale;
            yield return new WaitForSeconds(0.05f); 
        }
    }
    void SetFinalReels(int[] result)
    {
        isSpinning = false; 
        reels[0].text = symbolEmojis[result[0]];
        reels[1].text = symbolEmojis[result[1]];
        reels[2].text = symbolEmojis[result[2]];
        reels[0].transform.localScale = Vector3.one;
        reels[1].transform.localScale = Vector3.one;
        reels[2].transform.localScale = Vector3.one;
    }
    void ShowPayout(float payout)
    {
        if (payout > 0) statusText.text = $"<color=#00FFAA>YOU WON ${payout}!</color>";
        else statusText.text = $"<color=#FF4444>NO WIN. TRY AGAIN.</color>";
    }
}
