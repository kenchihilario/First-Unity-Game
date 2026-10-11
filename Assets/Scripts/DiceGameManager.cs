using UnityEngine;
using System;

public class DiceGameManager : MonoBehaviour
{
    public static DiceGameManager Instance;

    public Action<float, bool, float> OnRollComplete;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RollDice(float betAmount, float target, bool rollOver)
    {
        if (PlayerManager.Instance.balance < betAmount)
        {
            Debug.Log("Insufficient balance");
            return;
        }

        PlayerManager.Instance.Withdraw(betAmount);

        float result = UnityEngine.Random.Range(0f, 100f);
        result = (float)Math.Round(result, 2);

        bool won = rollOver ? (result > target) : (result < target);

        float payout = 0;
        if (won)
        {
            float winChance = rollOver ? (100f - target) : target;
            if (winChance > 0)
            {
                float multiplier = 99f / winChance;
                payout = betAmount * multiplier;
                PlayerManager.Instance.Deposit(payout);
            }
        }

        OnRollComplete?.Invoke(result, won, payout);
    }
}
