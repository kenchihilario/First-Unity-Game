using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
public class SlotsGameManager : MonoBehaviour
{
    public static SlotsGameManager Instance;
    public enum GameState { Idle, Spinning, Result }
    public GameState currentState = GameState.Idle;
    public Action<GameState> OnStateChanged;
    public Action<int[]> OnSpinResult; 
    public Action<float> OnPayout;
    private float currentBet = 0f;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void Spin(float amount)
    {
        if (currentState == GameState.Idle && PlayerManager.Instance.PlaceBet(amount))
        {
            currentBet = amount;
            StartCoroutine(SpinRoutine());
        }
    }
    IEnumerator SpinRoutine()
    {
        currentState = GameState.Spinning;
        OnStateChanged?.Invoke(currentState);
        yield return new WaitForSeconds(3f);
        try
        {
            int[] result = new int[3];
            for(int i=0; i<3; i++) {
                float r = Random.value;
                if (r < 0.40f) result[i] = 0;      
                else if (r < 0.70f) result[i] = 1; 
                else if (r < 0.90f) result[i] = 2; 
                else if (r < 0.98f) result[i] = 3; 
                else result[i] = 4;                
            }
            OnSpinResult?.Invoke(result);
            float payout = 0;
            if (result[0] == result[1] && result[1] == result[2]) {
                if (result[0] == 4) payout = currentBet * 100f; 
                else if (result[0] == 3) payout = currentBet * 50f; 
                else if (result[0] == 2) payout = currentBet * 20f; 
                else if (result[0] == 1) payout = currentBet * 10f; 
                else payout = currentBet * 5f; 
            }
            else if (result[0] == result[1] || result[1] == result[2] || result[0] == result[2]) {
                payout = currentBet * 1.5f;
            }
            if (payout > 0)
            {
                PlayerManager.Instance.AddWinnings(payout);
            }
            currentState = GameState.Result;
            OnPayout?.Invoke(payout);
            OnStateChanged?.Invoke(currentState);
        }
        finally
        {
            StartCoroutine(ResetIdleRoutine());
        }
    }
    IEnumerator ResetIdleRoutine()
    {
        yield return new WaitForSeconds(2f); 
        currentState = GameState.Idle;
        OnStateChanged?.Invoke(currentState);
    }
}
