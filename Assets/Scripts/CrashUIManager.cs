using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CrashUIManager : MonoBehaviour
{
    public Text multiplierText;
    public Text statusText;
    public Text balanceText;
    public Button actionButton;
    public Text actionButtonText;
    public Image backgroundImage;
    public Transform graphContainer;
    public InputField betAmountInput;
    public Button halfBetBtn;
    public Button doubleBetBtn;
    private float betAmount = 10f;
    private Color bgWaiting = new Color(0.1f, 0.1f, 0.15f);
    private Color bgPlaying = new Color(0.1f, 0.2f, 0.1f);
    private Color bgCrashed = new Color(0.2f, 0.1f, 0.1f);
    private LineRenderer lineRenderer;
    private GameObject rocketDot;
    void Start()
    {
        if (CrashGameManager.Instance != null)
        {
            CrashGameManager.Instance.OnMultiplierUpdated += UpdateMultiplier;
            CrashGameManager.Instance.OnGameStateChanged += UpdateGameState;
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
        actionButton.onClick.AddListener(OnActionButtonClicked);
        SetupGraph();
    }
    void UpdateBetDisplay()
    {
        if (betAmountInput != null) betAmountInput.text = betAmount.ToString("F2");
        if (CrashGameManager.Instance.currentState == CrashGameManager.GameState.Waiting)
        {
            actionButtonText.text = $"BET ${betAmount:F2}";
        }
    }
    void Update()
    {
        if (PlayerManager.Instance != null && balanceText != null)
        {
            balanceText.text = $"<color=#888888>BALANCE</color>\n<color=#00FFAA><size=50>${PlayerManager.Instance.balance:F2}</size></color>";
        }
    }
    void SetupGraph()
    {
        GameObject lineGo = new GameObject("CrashLine");
        lineGo.transform.SetParent(graphContainer, false);
        lineRenderer = lineGo.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0.2f, 0.8f, 1f);
        lineRenderer.endColor = new Color(0.8f, 0.2f, 1f);
        lineRenderer.startWidth = 10f;
        lineRenderer.endWidth = 10f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = false;
        RectTransform rt = lineGo.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rocketDot = new GameObject("RocketDot");
        rocketDot.transform.SetParent(graphContainer, false);
        Image dotImg = rocketDot.AddComponent<Image>();
        dotImg.color = Color.white;
        RectTransform dotRt = rocketDot.GetComponent<RectTransform>();
        dotRt.sizeDelta = new Vector2(30, 30);
        rocketDot.SetActive(false);
    }
    void OnActionButtonClicked()
    {
        if (CrashGameManager.Instance.currentState == CrashGameManager.GameState.Waiting)
        {
            if (CrashGameManager.Instance.PlaceBet(betAmount))
            {
                actionButtonText.text = "BET PLACED";
                actionButton.interactable = false;
            }
        }
        else if (CrashGameManager.Instance.currentState == CrashGameManager.GameState.Playing)
        {
            if (CrashGameManager.Instance.CashOut())
            {
                actionButtonText.text = "CASHED OUT";
                actionButton.interactable = false;
            }
        }
    }
    void UpdateMultiplier(float mult)
    {
        if (multiplierText != null)
        {
            multiplierText.text = mult.ToString("F2") + "x";
            float scale = 1f + (Mathf.Sin(Time.time * 20f) * 0.05f * Mathf.Clamp01(mult / 5f));
            multiplierText.transform.localScale = new Vector3(scale, scale, 1f);
            if (Time.frameCount % 10 == 0 && CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
        }
        if (lineRenderer != null && CrashGameManager.Instance.currentState == CrashGameManager.GameState.Playing)
        {
            int count = lineRenderer.positionCount;
            lineRenderer.positionCount = count + 1;
            float duration = Mathf.Log(Mathf.Max(1f, mult)) / 0.15f; 
            float rawX = (duration / 10f) * 800f; 
            float rawY = (mult - 1f) * 50f; 
            Vector3 pos = new Vector3(rawX - 400f, rawY - 150f, 0);
            lineRenderer.SetPosition(count, pos);
            if (rocketDot != null)
            {
                rocketDot.SetActive(true);
                rocketDot.GetComponent<RectTransform>().anchoredPosition = pos;
            }
            RectTransform graphRect = graphContainer.GetComponent<RectTransform>();
            float panX = 0f;
            float panY = 0f;
            if (rawX > 800f) panX = 800f - rawX;
            if (rawY > 300f) panY = 300f - rawY;
            graphRect.anchoredPosition = new Vector2(panX, panY);
        }
    }
    void UpdateGameState(CrashGameManager.GameState state)
    {
        halfBetBtn.interactable = state == CrashGameManager.GameState.Waiting;
        doubleBetBtn.interactable = state == CrashGameManager.GameState.Waiting;
        betAmountInput.interactable = state == CrashGameManager.GameState.Waiting;
        switch (state)
        {
            case CrashGameManager.GameState.Waiting:
                statusText.text = "WAITING FOR NEXT ROUND...";
                statusText.color = Color.white;
                multiplierText.text = "1.00x";
                multiplierText.color = Color.white;
                actionButtonText.text = $"BET ${betAmount:F2}";
                actionButton.interactable = true;
                backgroundImage.color = bgWaiting;
                if (lineRenderer != null) lineRenderer.positionCount = 0;
                if (rocketDot != null) rocketDot.SetActive(false);
                if (graphContainer != null) graphContainer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                break;
            case CrashGameManager.GameState.Playing:
                statusText.text = "FLYING!";
                statusText.color = new Color(0.2f, 1f, 0.2f);
                if (CrashGameManager.Instance.HasBet)
                {
                    actionButtonText.text = "CASH OUT";
                    actionButton.interactable = true;
                }
                else
                {
                    actionButtonText.text = "WAITING...";
                    actionButton.interactable = false;
                }
                backgroundImage.color = bgPlaying;
                break;
            case CrashGameManager.GameState.Crashed:
                statusText.text = "CRASHED!";
                statusText.color = new Color(1f, 0.2f, 0.2f);
                multiplierText.color = new Color(1f, 0.2f, 0.2f);
                actionButton.interactable = false;
                backgroundImage.color = bgCrashed;
                if (rocketDot != null) rocketDot.SetActive(false);
                if (CasinoAudio.Instance) CasinoAudio.Instance.PlayExplosion();
                StartCoroutine(ScreenShake());
                break;
        }
    }
    IEnumerator ScreenShake()
    {
        if (graphContainer == null || multiplierText == null) yield break;
        Vector3 origGraphPos = graphContainer.localPosition;
        Vector3 origTextPos = multiplierText.transform.localPosition;
        
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            float intensity = (0.5f - elapsed) * 60f; 
            graphContainer.localPosition = origGraphPos + (Vector3)Random.insideUnitCircle * intensity;
            multiplierText.transform.localPosition = origTextPos + (Vector3)Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        graphContainer.localPosition = origGraphPos;
        multiplierText.transform.localPosition = origTextPos;
    }
}
