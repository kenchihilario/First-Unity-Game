using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class PlinkoUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text statusText;
    public Button actionButton;
    public InputField betAmountInput;
    public Button halfBetBtn;
    public Button doubleBetBtn;
    public Transform boardContainer; 
    public GameObject ballPrefab;    
    private PlinkoGameManager gameManager;
    private Vector2[][] pegPositions;
    private Color ballColor = new Color(1f, 0.2f, 0.2f, 1f); 
    private Color activeSlotColor = new Color(0f, 0.9f, 0f, 1f);
    public Text[] slotTexts;
    void Start()
    {
        gameManager = GetComponent<PlinkoGameManager>();
        if (gameManager != null)
        {
            gameManager.OnStateChanged += UpdateState;
            gameManager.OnBallPathGenerated += AnimateBall;
            gameManager.OnBallLanded += HandleCashOut;
        }
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(OnActionClicked);
        }
        if (halfBetBtn != null) halfBetBtn.onClick.AddListener(() => ModifyBet(0.5f));
        if (doubleBetBtn != null) doubleBetBtn.onClick.AddListener(() => ModifyBet(2f));
        pegPositions = new Vector2[gameManager.rows][];
        float startY = 180f;
        float ySpacing = 40f;
        float xSpacing = 35f;
        for (int r = 0; r < gameManager.rows; r++)
        {
            int pegsInRow = r + 1;
            pegPositions[r] = new Vector2[pegsInRow];
            float rowWidth = (pegsInRow - 1) * xSpacing;
            float startX = -rowWidth / 2f;
            for (int c = 0; c < pegsInRow; c++)
            {
                pegPositions[r][c] = new Vector2(startX + c * xSpacing, startY - r * ySpacing);
            }
        }
        UpdateState(PlinkoGameManager.GameState.Idle);
    }
    void OnActionClicked()
    {
        if (gameManager.currentState == PlinkoGameManager.GameState.Idle)
        {
            float bet = 10f;
            if (betAmountInput != null && float.TryParse(betAmountInput.text, out float b)) bet = b;
            gameManager.DropBall(bet);
        }
    }
    void ModifyBet(float multiplier)
    {
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float currentBet))
        {
            betAmountInput.text = (currentBet * multiplier).ToString("0.00");
        }
    }
    void UpdateState(PlinkoGameManager.GameState state)
    {
        Text actionText = actionButton.transform.GetChild(0).GetComponent<Text>();
        betAmountInput.interactable = state == PlinkoGameManager.GameState.Idle;
        halfBetBtn.interactable = state == PlinkoGameManager.GameState.Idle;
        doubleBetBtn.interactable = state == PlinkoGameManager.GameState.Idle;
        actionButton.interactable = state == PlinkoGameManager.GameState.Idle;
        if (state == PlinkoGameManager.GameState.Idle)
        {
            actionText.text = "DROP BALL";
            actionButton.GetComponent<Image>().color = new Color(0f, 0.9f, 0.0f, 1f); 
            statusText.text = "<color=#AAAAAA>READY TO DROP</color>";
            foreach(var txt in slotTexts) {
                if (txt != null) txt.color = Color.white;
            }
        }
        else if (state == PlinkoGameManager.GameState.Dropping)
        {
            actionText.text = "DROPPING...";
            actionButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f); 
            statusText.text = "<color=#FFFF00>WATCH THE BALL!</color>";
        }
    }
    void AnimateBall(int[] path)
    {
        StartCoroutine(BallDropRoutine(path));
    }
    IEnumerator BallDropRoutine(int[] path)
    {
        GameObject ball = new GameObject("Ball", typeof(RectTransform));
        ball.transform.SetParent(boardContainer, false);
        Image img = ball.AddComponent<Image>();
        img.color = ballColor;
        UnityEngine.UI.Shadow shadow = ball.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = new Color(1f, 0.5f, 0f, 0.5f);
        shadow.effectDistance = new Vector2(0, 0); 
        RectTransform ballRt = ball.GetComponent<RectTransform>();
        ballRt.sizeDelta = new Vector2(24, 24);
        int currentCol = 0;
        ballRt.anchoredPosition = new Vector2(0, 220); 
        float dropSpeed = 0.15f;
        for (int r = 0; r < gameManager.rows; r++)
        {
            Vector2 startPos = ballRt.anchoredPosition;
            Vector2 pegPos = pegPositions[r][currentCol];
            float t = 0;
            while(t < 1f)
            {
                t += Time.deltaTime / dropSpeed;
                ballRt.anchoredPosition = Vector2.Lerp(startPos, pegPos + new Vector2(0, 10), t); 
                yield return null;
            }
            int dir = path[r]; 
            currentCol += dir;
            Vector2 endPos = pegPos;
            if (r < gameManager.rows - 1)
            {
                endPos = pegPositions[r+1][currentCol];
            }
            else
            {
                float xSpacing = 35f;
                float ySpacing = 40f;
                float startY = 180f;
                int finalSlots = gameManager.rows + 1;
                float rowWidth = (finalSlots - 1) * xSpacing;
                float startX = -rowWidth / 2f;
                endPos = new Vector2(startX + currentCol * xSpacing, startY - (r+1) * ySpacing);
            }
            t = 0;
            while(t < 1f)
            {
                t += Time.deltaTime / (dropSpeed * 1.2f);
                float arc = Mathf.Sin(t * Mathf.PI) * 15f; 
                ballRt.anchoredPosition = Vector2.Lerp(pegPos, endPos, t) + new Vector2(0, arc);
                yield return null;
            }
        }
        if (slotTexts[currentCol] != null)
        {
            slotTexts[currentCol].color = activeSlotColor;
        }
        Destroy(ball, 1f); 
        gameManager.BallReachedBottom(currentCol);
    }
    void HandleCashOut(float payout)
    {
        if (payout > 0)
        {
            statusText.text = $"<color=#00FFAA>YOU WON ${payout:F2}!</color>";
        }
        else
        {
            statusText.text = $"<color=#FF4444>WON $0.00.</color>";
        }
    }
}
