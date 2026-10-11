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
            BlackjackGameManager.Instance.OnGameStarted += HandleGameStart;
            BlackjackGameManager.Instance.OnPlayerHit += HandlePlayerHit;
            BlackjackGameManager.Instance.OnGameOver += HandleGameOver;
        }

        if (dealBtn != null) dealBtn.onClick.AddListener(OnDealClicked);
        if (hitBtn != null) hitBtn.onClick.AddListener(() => BlackjackGameManager.Instance.Hit());
        if (standBtn != null) standBtn.onClick.AddListener(() => BlackjackGameManager.Instance.Stand());

        if (halfBetBtn != null) halfBetBtn.onClick.AddListener(() => AdjustBet(0.5f));
        if (doubleBetBtn != null) doubleBetBtn.onClick.AddListener(() => AdjustBet(2f));

        SetPlayState(false);
        if (statusText) statusText.text = "PLACE YOUR BET";
    }

    void AdjustBet(float multiplier)
    {
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float amt))
        {
            betAmountInput.text = (amt * multiplier).ToString("F2");
        }
    }

    void OnDealClicked()
    {
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float bet) && bet > 0)
        {
            if (statusText) statusText.text = "";
            BlackjackGameManager.Instance.StartGame(bet);
        }
    }

    void SetPlayState(bool playing)
    {
        if (dealBtn) dealBtn.interactable = !playing;
        if (hitBtn) hitBtn.interactable = playing;
        if (standBtn) standBtn.interactable = playing;
        if (betAmountInput) betAmountInput.interactable = !playing;
    }

    string FormatHand(List<BlackjackGameManager.Card> hand, bool hideSecond = false)
    {
        string s = "";
        for (int i = 0; i < hand.Count; i++)
        {
            if (hideSecond && i == 1) s += "[?] ";
            else s += "[" + hand[i].ToString() + "] ";
        }
        return s;
    }

    void HandleGameStart(List<BlackjackGameManager.Card> pHand, List<BlackjackGameManager.Card> dHand, bool dHidden)
    {
        SetPlayState(true);
        if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayCard();
        if (playerCardsText) playerCardsText.text = FormatHand(pHand);
        if (playerScoreText) playerScoreText.text = BlackjackGameManager.Instance.GetScore(pHand).ToString();

        if (dealerCardsText) dealerCardsText.text = FormatHand(dHand, true);
        if (dealerScoreText) dealerScoreText.text = "?";
    }

    void HandlePlayerHit(List<BlackjackGameManager.Card> pHand, int score)
    {
        if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayCard();
        if (playerCardsText) playerCardsText.text = FormatHand(pHand);
        if (playerScoreText) playerScoreText.text = score.ToString();
    }

    void HandleGameOver(List<BlackjackGameManager.Card> dHand, int dScore, bool playerWon, float payout)
    {
        SetPlayState(false);
        if (dealerCardsText) dealerCardsText.text = FormatHand(dHand);
        if (dealerScoreText) dealerScoreText.text = dScore.ToString();

        if (statusText)
        {
            if (payout > 0 && payout == BlackjackGameManager.Instance.currentBet)
            {
                if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayCoin();
                statusText.text = $"<color=yellow>PUSH</color>";
            }
            else if (playerWon)
            {
                if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
                statusText.text = $"<color=#00FFAA>WON ₱{payout:F2}!</color>";
            }
            else
            {
                if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayLoss();
                statusText.text = $"<color=#FF3333>BUST / LOST</color>";
            }
        }
    }
}
