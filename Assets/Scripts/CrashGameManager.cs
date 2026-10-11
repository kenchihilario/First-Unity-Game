using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
public class CrashGameManager : MonoBehaviour
{
    public static CrashGameManager Instance;
    public enum GameState { Waiting, Playing, Crashed }
    public GameState currentState = GameState.Waiting;
    public float currentMultiplier = 1.0f;
    private float targetCrashMultiplier;
    public Action<float> OnMultiplierUpdated;
    public Action<GameState> OnGameStateChanged;
    private float timeSinceStart;
    private float currentBetAmount = 0f;
    private bool hasCashedOut = false;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        StartCoroutine(GameLoop());
    }
    IEnumerator GameLoop()
    {
        while (true)
        {
            currentState = GameState.Waiting;
            currentMultiplier = 1.0f;
            OnGameStateChanged?.Invoke(currentState);
            OnMultiplierUpdated?.Invoke(currentMultiplier);
            yield return new WaitForSeconds(5f);
            currentState = GameState.Playing;
            OnGameStateChanged?.Invoke(currentState);
            hasCashedOut = false;
            float rand = Random.Range(0f, 100f);
            if (rand < 2f)
            {
                targetCrashMultiplier = 1.0f;
            }
            else
            {
                targetCrashMultiplier = 100f / (100f - Random.Range(0f, 99f));
            }
            timeSinceStart = 0f;
            while (currentMultiplier < targetCrashMultiplier)
            {
                timeSinceStart += Time.deltaTime;
                currentMultiplier = Mathf.Pow(2.71828f, timeSinceStart * 0.15f);
                if (currentMultiplier >= targetCrashMultiplier)
                {
                    currentMultiplier = targetCrashMultiplier;
                    break;
                }
                OnMultiplierUpdated?.Invoke(currentMultiplier);
                yield return null;
            }
            currentState = GameState.Crashed;
            OnGameStateChanged?.Invoke(currentState);
            if (!hasCashedOut && currentBetAmount > 0)
            {
                currentBetAmount = 0f;
            }
            yield return new WaitForSeconds(3f);
        }
    }
    public bool HasBet => currentBetAmount > 0 && !hasCashedOut;
    public bool PlaceBet(float amount)
    {
        if (currentState == GameState.Waiting && currentBetAmount == 0)
        {
            if (PlayerManager.Instance != null && PlayerManager.Instance.PlaceBet(amount))
            {
                currentBetAmount = amount;
                return true;
            }
        }
        return false;
    }
    public bool CashOut()
    {
        if (currentState == GameState.Playing && currentBetAmount > 0 && !hasCashedOut)
        {
            hasCashedOut = true;
            float winnings = currentBetAmount * currentMultiplier;
            if (PlayerManager.Instance != null)
            {
                PlayerManager.Instance.AddWinnings(winnings);
            }
            currentBetAmount = 0f;
            return true;
        }
        return false;
    }
}
