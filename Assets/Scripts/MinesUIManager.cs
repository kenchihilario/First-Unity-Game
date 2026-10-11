using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class MinesUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text statusText;
    public Text currentMultiplierText;
    public Button actionButton;
    public InputField betAmountInput;
    public InputField minesCountInput;
    public Button halfBetBtn;
    public Button doubleBetBtn;
    public Button[] tileButtons = new Button[25];
    private Text[] tileTexts = new Text[25];
    private MinesGameManager gameManager;
    private Color darkTileColor = new Color(0.15f, 0.17f, 0.22f, 1f);
    private Color gemColor = new Color(0f, 0.8f, 0.4f, 1f);
    private Color mineColor = new Color(1f, 0.2f, 0.2f, 1f);
    void Start()
    {
        gameManager = FindAnyObjectByType<MinesGameManager>();
        if (gameManager != null)
        {
            gameManager.OnStateChanged += UpdateState;
            gameManager.OnTileRevealed += HandleTileRevealed;
            gameManager.OnCashOut += HandleCashOut;
        }
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(OnActionClicked);
        }
        if (halfBetBtn != null) halfBetBtn.onClick.AddListener(() => ModifyBet(0.5f));
        if (doubleBetBtn != null) doubleBetBtn.onClick.AddListener(() => ModifyBet(2f));
        for (int i = 0; i < 25; i++)
        {
            int index = i;
            if (tileButtons[i] != null)
            {
                tileButtons[i].onClick.AddListener(() => gameManager.ClickTile(index));
                tileTexts[i] = tileButtons[i].transform.GetChild(0).GetComponent<Text>();
                ResetTile(i);
            }
        }
        UpdateState(MinesGameManager.GameState.Idle);
    }
    void OnActionClicked()
    {
        if (gameManager.currentState == MinesGameManager.GameState.Idle)
        {
            float bet = 10f;
            if (betAmountInput != null && float.TryParse(betAmountInput.text, out float b)) bet = b;
            int mines = 3;
            if (minesCountInput != null && int.TryParse(minesCountInput.text, out int m)) mines = m;
            gameManager.StartGame(bet, mines);
        }
        else if (gameManager.currentState == MinesGameManager.GameState.Playing)
        {
            gameManager.CashOut();
        }
        else if (gameManager.currentState == MinesGameManager.GameState.Result)
        {
            gameManager.ResetGame();
        }
    }
    void ModifyBet(float multiplier)
    {
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float currentBet))
        {
            betAmountInput.text = (currentBet * multiplier).ToString("0.00");
        }
    }
    void UpdateState(MinesGameManager.GameState state)
    {
        Text actionText = actionButton.transform.GetChild(0).GetComponent<Text>();
        betAmountInput.interactable = state == MinesGameManager.GameState.Idle;
        if(minesCountInput != null) minesCountInput.interactable = state == MinesGameManager.GameState.Idle;
        halfBetBtn.interactable = state == MinesGameManager.GameState.Idle;
        doubleBetBtn.interactable = state == MinesGameManager.GameState.Idle;
        if (state == MinesGameManager.GameState.Idle)
        {
            actionText.text = "BET";
            actionButton.GetComponent<Image>().color = new Color(0f, 0.9f, 0.0f, 1f);
            statusText.text = "<color=#AAAAAA>READY TO PLAY</color>";
            currentMultiplierText.text = "1.00x";
            for (int i = 0; i < 25; i++) ResetTile(i);
        }
        else if (state == MinesGameManager.GameState.Playing)
        {
            actionText.text = "CASHOUT";
            actionButton.GetComponent<Image>().color = new Color(1f, 0.6f, 0.0f, 1f);
            statusText.text = "<color=#FFFF00>PICK A TILE...</color>";
        }
        else if (state == MinesGameManager.GameState.Result)
        {
            actionText.text = "PLAY AGAIN";
            actionButton.GetComponent<Image>().color = new Color(0.13f, 0.67f, 1f, 1f);
            for (int i = 0; i < 25; i++)
            {
                if (!gameManager.revealed[i])
                {
                    if (gameManager.grid[i])
                    {
                        tileTexts[i].text = "💣";
                        tileButtons[i].GetComponent<Image>().color = new Color(mineColor.r, mineColor.g, mineColor.b, 0.3f);
                    }
                    else
                    {
                        tileTexts[i].text = "💎";
                        tileButtons[i].GetComponent<Image>().color = new Color(gemColor.r, gemColor.g, gemColor.b, 0.3f);
                    }
                }
            }
        }
    }
    void ResetTile(int index)
    {
        if (tileButtons[index] == null) return;
        tileTexts[index].text = "";
        tileButtons[index].GetComponent<Image>().color = darkTileColor;
        tileButtons[index].transform.localScale = Vector3.one;
        tileButtons[index].transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    void HandleTileRevealed(int index, bool isMine, float newMultiplier)
    {
        if (tileButtons[index] == null) return;
        StartCoroutine(FlipTileRoutine(index, isMine, newMultiplier));
    }

    IEnumerator FlipTileRoutine(int index, bool isMine, float newMultiplier)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        RectTransform rt = tileButtons[index].GetComponent<RectTransform>();

        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2f);
            rt.localRotation = Quaternion.Euler(0, Mathf.Lerp(0, 90f, t), 0);
            yield return null;
        }

        if (isMine)
        {
            if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayExplosion();
            tileTexts[index].text = "💣";
            tileButtons[index].GetComponent<Image>().color = mineColor;
        }
        else
        {
            if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayCoin();
            tileTexts[index].text = "💎";
            tileButtons[index].GetComponent<Image>().color = gemColor;
            currentMultiplierText.text = $"{newMultiplier:F2}x";
            statusText.text = $"<color=#00FFAA>GEMS FOUND: {gameManager.gemsFound}</color>";
        }

        elapsed = 0f;
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2f);
            rt.localRotation = Quaternion.Euler(0, Mathf.Lerp(90f, 0, t), 0);
            rt.localScale = Vector3.one * (1f + Mathf.Sin(t * Mathf.PI) * 0.2f);
            yield return null;
        }
        rt.localRotation = Quaternion.Euler(0, 0, 0);
        rt.localScale = Vector3.one;
    }

    void HandleCashOut(float payout)
    {
        if (payout > 0)
        {
            if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
            statusText.text = $"<color=#00FFAA>YOU WON ${payout:F2}!</color>";
        }
        else
        {
            if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayLoss();
            statusText.text = $"<color=#FF4444>BOOM! YOU LOST.</color>";
        }
    }
}
