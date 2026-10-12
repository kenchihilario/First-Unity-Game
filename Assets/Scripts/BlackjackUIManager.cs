using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BlackjackUIManager : MonoBehaviour
{
    public InputField betAmountInput;
    public Button halfBetBtn;
    public Button doubleBetBtn;
    public Button dealBtn;

    public Button hitBtn;
    public Button standBtn;

    public Text statusText;
    public Text dealerCardsText;
    public Text playerCardsText;
    public Text dealerScoreText;
    public Text playerScoreText;

    void Start()
    {
        if (BlackjackGameManager.Instance != null)
        {
            BlackjackGameManager.Instance.OnGameStarted -= HandleGameStart;
            BlackjackGameManager.Instance.OnGameStarted += HandleGameStart;
            BlackjackGameManager.Instance.OnPlayerHit -= HandlePlayerHit;
            BlackjackGameManager.Instance.OnPlayerHit += HandlePlayerHit;
            BlackjackGameManager.Instance.OnGameOver -= HandleGameOver;
            BlackjackGameManager.Instance.OnGameOver += HandleGameOver;
        }

        if (betAmountInput == null) betAmountInput = transform.Find("BetPanel/BetInput")?.GetComponent<InputField>() ?? GetComponentInChildren<InputField>(true);
        if (halfBetBtn == null) halfBetBtn = transform.Find("BetPanel/HalfBtn")?.GetComponent<Button>();
        if (doubleBetBtn == null) doubleBetBtn = transform.Find("BetPanel/DoubleBtn")?.GetComponent<Button>();
        if (dealBtn == null) dealBtn = transform.Find("BetPanel/DealBtn")?.GetComponent<Button>() ?? transform.Find("BetPanel/ActionBtn")?.GetComponent<Button>() ?? transform.Find("DealBtn")?.GetComponent<Button>();
        if (hitBtn == null) hitBtn = transform.Find("HitBtn")?.GetComponent<Button>() ?? transform.Find("ActionPanel/HitBtn")?.GetComponent<Button>();
        if (standBtn == null) standBtn = transform.Find("StandBtn")?.GetComponent<Button>() ?? transform.Find("ActionPanel/StandBtn")?.GetComponent<Button>();

        if (statusText == null) statusText = transform.Find("StatusText")?.GetComponent<Text>();
        if (dealerCardsText == null) dealerCardsText = transform.Find("DealerCards")?.GetComponent<Text>() ?? transform.Find("DealerArea/CardsText")?.GetComponent<Text>();
        if (playerCardsText == null) playerCardsText = transform.Find("PlayerCards")?.GetComponent<Text>() ?? transform.Find("PlayerArea/CardsText")?.GetComponent<Text>();
        if (dealerScoreText == null) dealerScoreText = transform.Find("DealerScore")?.GetComponent<Text>() ?? transform.Find("DealerArea/ScoreText")?.GetComponent<Text>();
        if (playerScoreText == null) playerScoreText = transform.Find("PlayerScore")?.GetComponent<Text>() ?? transform.Find("PlayerArea/ScoreText")?.GetComponent<Text>();

        if (dealBtn != null)
        {
            dealBtn.onClick.RemoveAllListeners();
            dealBtn.onClick.AddListener(OnDealClicked);
            Text dTxt = dealBtn.GetComponentInChildren<Text>();
            if (dTxt != null) dTxt.text = "<b>DEAL HAND</b>";
        }
        if (hitBtn != null)
        {
            hitBtn.onClick.RemoveAllListeners();
            hitBtn.onClick.AddListener(() => { if (BlackjackGameManager.Instance != null) BlackjackGameManager.Instance.Hit(); });
        }
        if (standBtn != null)
        {
            standBtn.onClick.RemoveAllListeners();
            standBtn.onClick.AddListener(() => { if (BlackjackGameManager.Instance != null) BlackjackGameManager.Instance.Stand(); });
        }

        if (halfBetBtn != null)
        {
            halfBetBtn.onClick.RemoveAllListeners();
            halfBetBtn.onClick.AddListener(() => AdjustBet(0.5f));
        }
        if (doubleBetBtn != null)
        {
            doubleBetBtn.onClick.RemoveAllListeners();
            doubleBetBtn.onClick.AddListener(() => AdjustBet(2f));
        }

        if (betAmountInput != null && string.IsNullOrEmpty(betAmountInput.text))
        {
            betAmountInput.text = "100";
        }

        SetPlayState(false);
        if (statusText) statusText.text = "<b><color=#94A3B8>PLACE YOUR BET AND CLICK DEAL</color></b>";
        if (dealerScoreText) dealerScoreText.text = "<b>SCORE: <color=#94A3B8>-</color></b>";
        if (playerScoreText) playerScoreText.text = "<b>SCORE: <color=#94A3B8>-</color></b>";
    }

    void OnDestroy()
    {
        if (BlackjackGameManager.Instance != null)
        {
            BlackjackGameManager.Instance.OnGameStarted -= HandleGameStart;
            BlackjackGameManager.Instance.OnPlayerHit -= HandlePlayerHit;
            BlackjackGameManager.Instance.OnGameOver -= HandleGameOver;
        }
    }

    void AdjustBet(float multiplier)
    {
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float amt))
        {
            betAmountInput.text = Mathf.Max(1f, Mathf.Round(amt * multiplier)).ToString("F2");
        }
    }

    void OnDealClicked()
    {
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float bet) && bet > 0)
        {
            if (PlayerManager.Instance != null && PlayerManager.Instance.balance < bet)
            {
                if (statusText) statusText.text = "<b><color=#EF4444>INSUFFICIENT BALANCE</color></b>";
                return;
            }
            if (statusText) statusText.text = "<b><color=#38BDF8>DEALING CARDS...</color></b>";
            if (BlackjackGameManager.Instance != null) BlackjackGameManager.Instance.StartGame(bet);
        }
        else
        {
            if (statusText) statusText.text = "<b><color=#EF4444>ENTER VALID BET AMOUNT</color></b>";
        }
    }

    void SetPlayState(bool playing)
    {
        if (dealBtn)
        {
            dealBtn.interactable = !playing;
            Text dTxt = dealBtn.GetComponentInChildren<Text>();
            if (dTxt != null) dTxt.text = playing ? "<b>HAND IN PLAY</b>" : "<b>DEAL HAND</b>";
        }
        if (hitBtn) hitBtn.interactable = playing;
        if (standBtn) standBtn.interactable = playing;
        if (betAmountInput) betAmountInput.interactable = !playing;
        if (halfBetBtn) halfBetBtn.interactable = !playing;
        if (doubleBetBtn) doubleBetBtn.interactable = !playing;
    }

    string FormatHand(List<BlackjackGameManager.Card> hand, bool hideSecond = false)
    {
        string s = "";
        for (int i = 0; i < hand.Count; i++)
        {
            if (hideSecond && i == 1) s += "  <b>[ <color=#F59E0B>?</color> ]</b>  ";
            else s += $"  <b>[ {hand[i].ToString()} ]</b>  ";
        }
        return s;
    }

    void HandleGameStart(List<BlackjackGameManager.Card> pHand, List<BlackjackGameManager.Card> dHand, bool dHidden)
    {
        SetPlayState(true);
        if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayCard();
        if (playerCardsText) playerCardsText.text = FormatHand(pHand);
        if (playerScoreText) playerScoreText.text = $"<b>SCORE: <color=#F59E0B>{(BlackjackGameManager.Instance != null ? BlackjackGameManager.Instance.GetScore(pHand) : 0)}</color></b>";

        if (dealerCardsText) dealerCardsText.text = FormatHand(dHand, true);
        if (dealerScoreText) dealerScoreText.text = "<b>SCORE: <color=#F59E0B>?</color></b>";
        if (statusText) statusText.text = "<b><color=#10B981>YOUR TURN - HIT OR STAND</color></b>";
    }

    void HandlePlayerHit(List<BlackjackGameManager.Card> pHand, int score)
    {
        if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayCard();
        if (playerCardsText) playerCardsText.text = FormatHand(pHand);
        if (playerScoreText) playerScoreText.text = $"<b>SCORE: <color=#F59E0B>{score}</color></b>";
    }

    void HandleGameOver(List<BlackjackGameManager.Card> dHand, int dScore, bool playerWon, float payout)
    {
        SetPlayState(false);
        if (dealerCardsText) dealerCardsText.text = FormatHand(dHand);
        if (dealerScoreText) dealerScoreText.text = $"<b>SCORE: <color=#F59E0B>{dScore}</color></b>";

        if (statusText)
        {
            if (payout > 0 && BlackjackGameManager.Instance != null && payout == BlackjackGameManager.Instance.currentBet)
            {
                if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayCoin();
                statusText.text = "<b><color=#F59E0B>PUSH (TIE) - BET RETURNED</color></b>";
            }
            else if (playerWon)
            {
                if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
                statusText.text = $"<b><color=#10B981>PLAYER WINS ₱{payout:N2}!</color></b>";
            }
            else
            {
                if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayLoss();
                statusText.text = "<b><color=#EF4444>DEALER WINS / BUST</color></b>";
            }
        }
    }
}
