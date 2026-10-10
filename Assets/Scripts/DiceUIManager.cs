using UnityEngine;
using UnityEngine.UI;

public class DiceUIManager : MonoBehaviour
{
    public Text statusText;
    public Text resultText;
    public Text targetText;
    public Text multiplierText;
    public Text winChanceText;
    public Slider targetSlider;
    public Button toggleOverUnderBtn;
    public Text toggleOverUnderText;
    public Button rollBtn;
    
    public InputField betAmountInput;
    public Button halfBetBtn;
    public Button doubleBetBtn;
    
    private bool rollOver = true;
    
    void Start()
    {
        if (DiceGameManager.Instance != null)
        {
            DiceGameManager.Instance.OnRollComplete += HandleRollComplete;
        }
        
        if (rollBtn != null) rollBtn.onClick.AddListener(OnRollClicked);
        if (toggleOverUnderBtn != null) toggleOverUnderBtn.onClick.AddListener(ToggleOverUnder);
        if (targetSlider != null) targetSlider.onValueChanged.AddListener(UpdateTargetUI);
        
        if (halfBetBtn != null) halfBetBtn.onClick.AddListener(() => AdjustBet(0.5f));
        if (doubleBetBtn != null) doubleBetBtn.onClick.AddListener(() => AdjustBet(2f));
        
        if (targetSlider != null)
        {
            targetSlider.minValue = 2f;
            targetSlider.maxValue = 98f;
            targetSlider.value = 50f;
            UpdateTargetUI(targetSlider.value);
        }
    }

    void UpdateTargetUI(float value)
    {
        float target = (float)System.Math.Round(value, 2);
        if (targetText != null) targetText.text = "Target: " + target.ToString("F2");
        
        float winChance = rollOver ? (100f - target) : target;
        if (winChanceText != null) winChanceText.text = "Chance: " + winChance.ToString("F2") + "%";
        
        float multiplier = 99f / winChance;
        if (multiplierText != null) multiplierText.text = "Mult: " + multiplier.ToString("F2") + "x";
    }

    void ToggleOverUnder()
    {
        rollOver = !rollOver;
        if (toggleOverUnderText != null) toggleOverUnderText.text = rollOver ? "ROLL OVER" : "ROLL UNDER";
        if (targetSlider != null) UpdateTargetUI(targetSlider.value);
    }

    void AdjustBet(float multiplier)
    {
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float amt))
        {
            betAmountInput.text = (amt * multiplier).ToString("F2");
        }
    }

    void OnRollClicked()
    {
        if (betAmountInput != null && float.TryParse(betAmountInput.text, out float bet) && bet > 0)
        {
            if (statusText != null) statusText.text = "ROLLING...";
            if (rollBtn != null) rollBtn.interactable = false;
            float target = (float)System.Math.Round(targetSlider.value, 2);
            DiceGameManager.Instance.RollDice(bet, target, rollOver);
        }
    }

    void HandleRollComplete(float result, bool won, float payout)
    {
        StartCoroutine(AnimateRoll(result, won, payout));
    }

    private System.Collections.IEnumerator AnimateRoll(float finalResult, bool won, float payout)
    {
        if (rollBtn != null) rollBtn.interactable = false;
        if (statusText != null) statusText.text = "ROLLING...";
        
        float duration = 1.0f; 
        float elapsed = 0f;
        
        Vector3 originalScale = resultText != null ? resultText.transform.localScale : Vector3.one;
        Vector3 originalPanelPos = transform.localPosition;
        
        Color[] flashColors = new Color[] { Color.white, Color.yellow, Color.cyan, new Color(1f, 0.5f, 0f) };
        int colorIdx = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            if (resultText != null)
            {
                float randomVal = UnityEngine.Random.Range(0f, 100f);
                resultText.text = randomVal.ToString("F2");
                
                if (Time.frameCount % 5 == 0) 
                {
                    colorIdx = (colorIdx + 1) % flashColors.Length;
                    if (CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
                }
                resultText.color = flashColors[colorIdx];
                
                float jitter = UnityEngine.Random.Range(0.9f, 1.1f);
                resultText.transform.localScale = originalScale * jitter;
            }
            
            yield return null;
        }
        
        if (resultText != null)
        {
            if (won && CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
            else if (!won && CasinoAudio.Instance) CasinoAudio.Instance.PlayExplosion();

            resultText.text = finalResult.ToString("F2");
            resultText.color = won ? Color.green : Color.red;
            resultText.transform.localScale = originalScale * 1.6f;
        }
        
        if (statusText != null)
        {
            if (won)
            {
                statusText.text = $"<color=#00FFAA>WON ${payout:F2}!</color>";
            }
            else
            {
                statusText.text = $"<color=#FF3333>LOST</color>";
            }
        }
        
        float postDuration = 0.5f;
        float postElapsed = 0f;
        while (postElapsed < postDuration)
        {
            postElapsed += Time.deltaTime;
            float t = postElapsed / postDuration;
            
            if (resultText != null)
            {
                resultText.transform.localScale = Vector3.Lerp(originalScale * 1.6f, originalScale, t * t);
            }
            
            if (won)
            {
                float shakeAmt = (1f - t) * 15f; 
                transform.localPosition = originalPanelPos + new Vector3(
                    UnityEngine.Random.Range(-shakeAmt, shakeAmt),
                    UnityEngine.Random.Range(-shakeAmt, shakeAmt),
                    0
                );
            }
            
            yield return null;
        }
        
        if (resultText != null) resultText.transform.localScale = originalScale;
        transform.localPosition = originalPanelPos;
        
        if (rollBtn != null) rollBtn.interactable = true;
    }
}
