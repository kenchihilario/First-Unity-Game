using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
public class RouletteGameManager : MonoBehaviour
{
    public static RouletteGameManager Instance;
    public enum GameState { Betting, Spinning, Result }
    public GameState currentState = GameState.Betting;
    public Action<GameState> OnStateChanged;
    public Action<float> OnTimerUpdated;
    public Action<int> OnSpinResult;
    public float bettingTime = 10f;
    private float timer;
    private float redBet = 0f;
    private float blackBet = 0f;
    private float greenBet = 0f;
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
            currentState = GameState.Betting;
            redBet = blackBet = greenBet = 0f;
            timer = bettingTime;
            OnStateChanged?.Invoke(currentState);
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                OnTimerUpdated?.Invoke(timer);
                yield return null;
            }

            // 0 = green, 1-7 = red, 8-14 = black
            currentState = GameState.Spinning;
            OnStateChanged?.Invoke(currentState);
            int result = Random.Range(0, 15);
            OnSpinResult?.Invoke(result);
            yield return new WaitForSeconds(4f);

            currentState = GameState.Result;
            OnStateChanged?.Invoke(currentState);
            if (result == 0 && greenBet > 0) PlayerManager.Instance.AddWinnings(greenBet * 14f);
            else if (result >= 1 && result <= 7 && redBet > 0) PlayerManager.Instance.AddWinnings(redBet * 2f);
            else if (result >= 8 && result <= 14 && blackBet > 0) PlayerManager.Instance.AddWinnings(blackBet * 2f);
            yield return new WaitForSeconds(3f);
        }
    }

    public void PlaceBet(int color, float amount)
    {
        if (currentState == GameState.Betting && PlayerManager.Instance.PlaceBet(amount))
        {
            if (color == 0) greenBet += amount;
            else if (color == 1) redBet += amount;
            else if (color == 2) blackBet += amount;
        }
    }
}
