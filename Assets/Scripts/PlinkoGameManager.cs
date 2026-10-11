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
    public event Action<float, int[]> OnBallPathGenerated;
    public event Action<float> OnBallLanded;
    public void DropBall(float betAmount)
    {
        if (betAmount <= 0) return;
        if (PlayerManager.Instance.PlaceBet(betAmount))
        {
            int[] path = new int[rows];
            int finalSlot = 0;
            for (int i = 0; i < rows; i++)
            {
                int dir = UnityEngine.Random.value > 0.5f ? 1 : 0;
                path[i] = dir;
                finalSlot += dir;
            }
            OnBallPathGenerated?.Invoke(betAmount, path);
        }
    }
    public void BallReachedBottom(float betAmount, int finalSlot)
    {
        float mult = multipliers[finalSlot];
        float payout = betAmount * mult;
        if (payout > 0)
        {
            PlayerManager.Instance.AddWinnings(payout);
        }
        OnBallLanded?.Invoke(payout);
    }
}
