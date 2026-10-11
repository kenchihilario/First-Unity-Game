using UnityEngine;
using System;
using System.Collections.Generic;
public class MinesGameManager : MonoBehaviour
{
    public enum GameState { Idle, Playing, Result }
    public GameState currentState = GameState.Idle;
    public int totalMines = 3;
    public float currentBet = 0;
    public int gemsFound = 0;
    public bool[] grid = new bool[25];
    public bool[] revealed = new bool[25];
    public event Action<GameState> OnStateChanged;
    public event Action<int, bool, float> OnTileRevealed;
    public event Action<float> OnCashOut;
    // start a new mines round and randomly place mines on the 5x5 grid
    public void StartGame(float betAmount, int minesCount)
    {
        if (currentState != GameState.Idle || betAmount <= 0) return;
        if (PlayerManager.Instance.PlaceBet(betAmount))
        {
            currentBet = betAmount;
            totalMines = Mathf.Clamp(minesCount, 1, 24);
            gemsFound = 0;
            for (int i = 0; i < 25; i++)
            {
                grid[i] = false;
                revealed[i] = false;
            }

            // randomly place mines
            int minesPlaced = 0;
            while (minesPlaced < totalMines)
            {
                int r = UnityEngine.Random.Range(0, 25);
                if (!grid[r])
                {
                    grid[r] = true;
                    minesPlaced++;
                }
            }
            currentState = GameState.Playing;
            OnStateChanged?.Invoke(currentState);
        }
    }

    // player clicked a tile
    public void ClickTile(int index)
    {
        if (currentState != GameState.Playing || revealed[index]) return;
        revealed[index] = true;
        bool isMine = grid[index];
        if (isMine)
        {
            // hit a mine, player loses
            currentState = GameState.Result;
            OnTileRevealed?.Invoke(index, true, 0);
            OnCashOut?.Invoke(0);
            OnStateChanged?.Invoke(currentState);
        }
        else
        {
            // found a gem, update multiplier
            gemsFound++;
            float multiplier = CalculateMultiplier(gemsFound, totalMines);
            OnTileRevealed?.Invoke(index, false, multiplier);
            if (gemsFound == 25 - totalMines)
            {
                CashOut();
            }
        }
    }

    // take current winnings
    public void CashOut()
    {
        if (currentState != GameState.Playing || gemsFound == 0) return;
        float multiplier = CalculateMultiplier(gemsFound, totalMines);
        float payout = currentBet * multiplier;
        PlayerManager.Instance.AddWinnings(payout);
        currentState = GameState.Result;
        OnCashOut?.Invoke(payout);
        OnStateChanged?.Invoke(currentState);
    }

    // calculate multiplier based on probability
    public float CalculateMultiplier(int gems, int mines)
    {
        if (gems == 0) return 1.0f;
        double probability = 1.0;
        int remainingSafe = 25 - mines;
        int remainingTotal = 25;
        for (int i = 0; i < gems; i++)
        {
            probability *= (double)remainingSafe / remainingTotal;
            remainingSafe--;
            remainingTotal--;
        }
        float multiplier = (float)(0.99 / probability);
        return Mathf.Floor(multiplier * 100f) / 100f;
    }
    public void ResetGame()
    {
        if (currentState == GameState.Result)
        {
            currentState = GameState.Idle;
            OnStateChanged?.Invoke(currentState);
        }
    }
}
