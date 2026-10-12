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
        if (boardContainer == null) boardContainer = transform.Find("BoardContainer");
        if (actionButton == null)
        {
            Transform ab = transform.Find("BetPanel/ActionBtn");
            if (ab != null) actionButton = ab.GetComponent<Button>();
        }
        if (statusText == null)
        {
            Transform st = transform.Find("StatusText");
            if (st != null) statusText = st.GetComponent<Text>();
        }
        if (betAmountInput == null)
        {
            Transform inp = transform.Find("BetPanel/InputBg");
            if (inp != null) betAmountInput = inp.GetComponent<InputField>();
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

        if (gameManager == null) gameManager = FindAnyObjectByType<PlinkoGameManager>();
        if (gameManager == null) gameManager = PlinkoGameManager.Instance;
        if (gameManager != null)
        {
            gameManager.OnBallPathGenerated -= AnimateBall;
            gameManager.OnBallPathGenerated += AnimateBall;
            gameManager.OnBallLanded -= HandleCashOut;
            gameManager.OnBallLanded += HandleCashOut;
        }
        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnActionClicked);
        }
        if (halfBetBtn != null)
        {
            halfBetBtn.onClick.RemoveAllListeners();
            halfBetBtn.onClick.AddListener(() => ModifyBet(0.5f));
        }
        if (doubleBetBtn != null)
        {
            doubleBetBtn.onClick.RemoveAllListeners();
            doubleBetBtn.onClick.AddListener(() => ModifyBet(2f));
        }

        int rows = gameManager != null ? gameManager.rows : 8;
        pegPositions = new Vector2[rows][];
        for (int r = 0; r < rows; r++)
        {
            int pegsInRow = r + 1;
            pegPositions[r] = new Vector2[pegsInRow];
            for (int c = 0; c < pegsInRow; c++)
            {
                Transform peg = boardContainer != null ? boardContainer.Find($"Peg_{r}_{c}") : null;
                if (peg != null)
                {
                    pegPositions[r][c] = peg.GetComponent<RectTransform>().anchoredPosition;
                }
            }
        }

        if (slotTexts == null || slotTexts.Length == 0)
        {
            if (boardContainer != null)
            {
                List<Text> stList = new List<Text>();
                for (int i = 0; i <= rows; i++)
                {
                    Transform s = boardContainer.Find($"Slot_{i}");
                    if (s != null) stList.Add(s.GetComponentInChildren<Text>(true));
                }
                slotTexts = stList.ToArray();
            }
        }

        UpdateState(PlinkoGameManager.GameState.Idle);
    }
    void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnBallPathGenerated -= AnimateBall;
            gameManager.OnBallLanded -= HandleCashOut;
        }
    }
    void OnActionClicked()
    {
        if (gameManager == null) gameManager = FindAnyObjectByType<PlinkoGameManager>();
        if (gameManager == null) return;
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
        if (actionButton != null && actionButton.transform.childCount > 0)
        {
            Text actionText = actionButton.transform.GetChild(0).GetComponent<Text>();
            if (actionText != null) actionText.text = "DROP BALL";
            Image img = actionButton.GetComponent<Image>();
            if (img != null) img.color = new Color(0f, 0.9f, 0.0f, 1f);
        }
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
            if (gameObject.activeInHierarchy && CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
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

        ballRt.localScale = new Vector3(1.3f, 0.7f, 1f);
        if (slotTexts != null && currentCol >= 0 && currentCol < slotTexts.Length && slotTexts[currentCol] != null)
        {
            slotTexts[currentCol].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            slotTexts[currentCol].color = activeSlotColor;
        }
        if (gameObject.activeInHierarchy && CasinoAudio.Instance)
        {
            if (currentCol == 0 || (gameManager != null && currentCol == gameManager.rows)) CasinoAudio.Instance.PlayWin();
            else CasinoAudio.Instance.PlayCoin();
        }

        yield return new WaitForSeconds(0.1f);
        ballRt.localScale = Vector3.one;
        if (slotTexts != null && currentCol >= 0 && currentCol < slotTexts.Length && slotTexts[currentCol] != null)
        {
            slotTexts[currentCol].transform.localScale = Vector3.one;
        }

        Destroy(ball, 1f);
        if (gameManager != null) gameManager.BallReachedBottom(betAmount, currentCol);
    }
    void HandleCashOut(float payout)
    {
        if (statusText != null)
        {
            if (payout > 0) statusText.text = $"<color=#00FFAA>YOU WON ₱{payout:F2}!</color>";
            else statusText.text = $"<color=#FF4444>WON ₱0.00.</color>";
        }
    }
}
