using UnityEngine;
using System;
using System.Collections.Generic;

public class BlackjackGameManager : MonoBehaviour
{
    public static BlackjackGameManager Instance;
    
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public struct Card
    {
        public int value; // 1 (Ace) to 13 (King)
        public Suit suit;
        public override string ToString()
        {
            string v = value == 1 ? "A" : value == 11 ? "J" : value == 12 ? "Q" : value == 13 ? "K" : value.ToString();
            string s = suit == Suit.Hearts ? "<color=red>♥</color>" : suit == Suit.Diamonds ? "<color=red>♦</color>" : 
                       suit == Suit.Clubs ? "<color=white>♣</color>" : "<color=white>♠</color>";
            return v + s;
        }
    }

    public List<Card> playerHand = new List<Card>();
    public List<Card> dealerHand = new List<Card>();
    private List<Card> deck = new List<Card>();
    
    public Action<List<Card>, List<Card>, bool> OnGameStarted; // playerHand, dealerHand, dealerHidden
    public Action<List<Card>, int> OnPlayerHit; // playerHand, newScore
    public Action<List<Card>, int, bool, float> OnGameOver; // dealerHand, finalScore, playerWon/Bust/Push, payout
    
    public float currentBet = 0;
    private bool gameActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void InitializeDeck()
    {
        deck.Clear();
        for(int s=0; s<4; s++)
        {
            for(int v=1; v<=13; v++)
            {
                deck.Add(new Card { suit = (Suit)s, value = v });
            }
        }

        for(int i=0; i<deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    private Card DrawCard()
    {
        if (deck.Count == 0) InitializeDeck();
        Card c = deck[0];
        deck.RemoveAt(0);
        return c;
    }

    public int GetScore(List<Card> hand)
    {
        int score = 0;
        int aces = 0;
        foreach(var c in hand)
        {
            if (c.value >= 10) score += 10;
            else if (c.value == 1) { score += 11; aces += 1; }
            else score += c.value;
        }
        while(score > 21 && aces > 0)
        {
            score -= 10;
            aces -= 1;
        }
        return score;
    }

    public void StartGame(float betAmount)
    {
        if (gameActive) return;
        if (PlayerManager.Instance.balance < betAmount)
        {
            Debug.Log("Insufficient balance");
            return;
        }

        PlayerManager.Instance.Withdraw(betAmount);
        currentBet = betAmount;
        gameActive = true;
        
        InitializeDeck();
        playerHand.Clear();
        dealerHand.Clear();
        
        playerHand.Add(DrawCard());
        dealerHand.Add(DrawCard());
        playerHand.Add(DrawCard());
        dealerHand.Add(DrawCard()); // 2nd is hidden
        
        OnGameStarted?.Invoke(playerHand, dealerHand, true);
        
        if (GetScore(playerHand) == 21)
        {
            EndGame(true, true); // Blackjack!
        }
    }

    public void Hit()
    {
        if (!gameActive) return;
        playerHand.Add(DrawCard());
        int score = GetScore(playerHand);
        OnPlayerHit?.Invoke(playerHand, score);
        
        if (score > 21)
        {
            EndGame(false, false); // Bust
        }
        else if (score == 21)
        {
            Stand(); // Auto stand on 21
        }
    }

    public void Stand()
    {
        if (!gameActive) return;
        

        while(GetScore(dealerHand) < 17)
        {
            dealerHand.Add(DrawCard());
        }
        
        int pScore = GetScore(playerHand);
        int dScore = GetScore(dealerHand);
        
        if (dScore > 21 || pScore > dScore) EndGame(true, false);
        else if (pScore == dScore) EndGame(true, true, true); // Push
        else EndGame(false, false);
    }

    private void EndGame(bool playerWon, bool blackjack = false, bool push = false)
    {
        gameActive = false;
        float payout = 0;
        if (push)
        {
            payout = currentBet; // return bet
            PlayerManager.Instance.Deposit(payout);
        }
        else if (playerWon)
        {
            payout = blackjack ? currentBet * 2.5f : currentBet * 2f;
            PlayerManager.Instance.Deposit(payout);
        }
        
        OnGameOver?.Invoke(dealerHand, GetScore(dealerHand), playerWon, payout);
    }
}
