using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class PlinkoGameManager : MonoBehaviour
{
    public enum GameState { Idle, Dropping, Result }
    public GameState currentState = GameState.Idle;
    public int rows = 8;
    public float currentBet = 0;
    public float[] multipliers = new float[] { 29f, 4f, 1.5f, 0.3f, 0.2f, 0.3f, 1.5f, 4f, 29f };
    public event Action<GameState> OnStateChanged;
    public event Action<int[]> OnBallPathGenerated; 
    public event Action<float> OnBallLanded;
    public void DropBall(float betAmount)
    {
        if (currentState != GameState.Idle || betAmount <= 0) return;
        if (PlayerManager.Instance.PlaceBet(betAmount))
        {
            currentBet = betAmount;
            int[] path = new int[rows];
            int finalSlot = 0;
            for (int i = 0; i < rows; i++)
            {
                int dir = UnityEngine.Random.value > 0.5f ? 1 : 0;
                path[i] = dir;
                finalSlot += dir;
            }
            currentState = GameState.Dropping;
            OnStateChanged?.Invoke(currentState);
            OnBallPathGenerated?.Invoke(path);
        }
    }
    public void BallReachedBottom(int finalSlot)
    {
        if (currentState != GameState.Dropping) return;
        float mult = multipliers[finalSlot];
        float payout = currentBet * mult;
        if (payout > 0)
        {
            PlayerManager.Instance.AddWinnings(payout);
        }
        currentState = GameState.Result;
        OnBallLanded?.Invoke(payout);
        OnStateChanged?.Invoke(currentState);
        StartCoroutine(ResetRoutine());
    }
    IEnumerator ResetRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        currentState = GameState.Idle;
        OnStateChanged?.Invoke(currentState);
    }
}
