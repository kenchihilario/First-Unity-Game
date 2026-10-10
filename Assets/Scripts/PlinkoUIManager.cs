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
        gameManager = FindFirstObjectByType<PlinkoGameManager>();
        if (gameManager != null)
        {
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
        for (int r = 0; r < gameManager.rows; r++)
        {
            int pegsInRow = r + 1;
            pegPositions[r] = new Vector2[pegsInRow];
            for (int c = 0; c < pegsInRow; c++)
            {
                Transform peg = boardContainer.Find($"Peg_{r}_{c}");
                if (peg != null)
                {
                    pegPositions[r][c] = peg.GetComponent<RectTransform>().anchoredPosition;
                }
            }
        }
        UpdateState(PlinkoGameManager.GameState.Idle);
    }
    void OnActionClicked()
    {
        float bet = 10f;
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float b)) bet = b;
        gameManager.DropBall(bet);
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
        // Removed state logic so users can drop multiple balls!
        Text actionText = actionButton.transform.GetChild(0).GetComponent<Text>();
        actionText.text = "DROP BALL";
        actionButton.GetComponent<Image>().color = new Color(0f, 0.9f, 0.0f, 1f); 
    }
    void AnimateBall(float betAmount, int[] path)
    {
        StartCoroutine(BallDropRoutine(betAmount, path));
    }
    IEnumerator BallDropRoutine(float betAmount, int[] path)
    {
        GameObject ball = new GameObject("Ball", typeof(RectTransform));
        ball.transform.SetParent(boardContainer, false);
        Image img = ball.AddComponent<Image>();
        img.color = ballColor;
        
        Transform topPeg = boardContainer.Find("Peg_0_0");
        if (topPeg != null) img.sprite = topPeg.GetComponent<Image>().sprite;

        UnityEngine.UI.Shadow shadow = ball.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = new Color(1f, 0.5f, 0f, 0.5f);
        shadow.effectDistance = new Vector2(0, -2); 
        RectTransform ballRt = ball.GetComponent<RectTransform>();
        ballRt.sizeDelta = new Vector2(28, 28);
        int currentCol = 0;
        
        float dropSpeed = 0.22f; 
        float totalRotation = 0f;
        float ySpacingOffset = 60f;
        if (gameManager.rows > 1) ySpacingOffset = pegPositions[0][0].y - pegPositions[1][0].y;
        
        ballRt.anchoredPosition = pegPositions[0][0] + new Vector2(0, ySpacingOffset); 
        
        for (int r = 0; r < gameManager.rows; r++)
        {
            Vector2 startPos = ballRt.anchoredPosition;
            Vector2 pegPos = pegPositions[r][currentCol];
            float t = 0;
            
            while(t < 1f)
            {
                t += Time.deltaTime / (dropSpeed * 0.6f);
                float easeY = t * t; 
                ballRt.anchoredPosition = Vector2.Lerp(startPos, pegPos + new Vector2(0, 14), easeY); 
                yield return null;
            }
            
            ballRt.localScale = new Vector3(1.3f, 0.7f, 1f);
            yield return new WaitForSeconds(0.015f);
            ballRt.localScale = Vector3.one;

            int dir = path[r]; 
            currentCol += dir;
            Vector2 endPos = pegPos;
            if (r < gameManager.rows - 1)
            {
                endPos = pegPositions[r+1][currentCol];
            }
            else
            {
                Transform slotObj = boardContainer.Find($"Slot_{currentCol}");
                if (slotObj != null) endPos = slotObj.GetComponent<RectTransform>().anchoredPosition;
                else endPos = pegPos + new Vector2((dir == 1 ? 1 : -1) * (pegPositions[1][1].x - pegPositions[1][0].x), -ySpacingOffset);
            }
            
            t = 0;
            float arcHeight = 35f;
            
            while(t < 1f)
            {
                t += Time.deltaTime / dropSpeed;
                float arc = 4f * arcHeight * t * (1f - t); 
                ballRt.anchoredPosition = Vector2.Lerp(pegPos, endPos, t) + new Vector2(0, arc);
                
                totalRotation += (dir == 1 ? -1 : 1) * 500f * Time.deltaTime;
                ballRt.localRotation = Quaternion.Euler(0, 0, totalRotation);
                yield return null;
            }
        }
        
        // Final bounce in slot
        ballRt.localScale = new Vector3(1.3f, 0.7f, 1f);
        if (slotTexts[currentCol] != null)
        {
            slotTexts[currentCol].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            slotTexts[currentCol].color = activeSlotColor;
        }
        
        yield return new WaitForSeconds(0.1f);
        ballRt.localScale = Vector3.one;
        if (slotTexts[currentCol] != null) slotTexts[currentCol].transform.localScale = Vector3.one;

        Destroy(ball, 1f); 
        gameManager.BallReachedBottom(betAmount, currentCol);
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
