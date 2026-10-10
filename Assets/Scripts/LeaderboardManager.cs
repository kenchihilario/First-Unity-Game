using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    
    public struct PlayerEntry
    {
        public string username;
        public float balance;
    }
    
    private List<PlayerEntry> dailyPlayers = new List<PlayerEntry>();
    private List<PlayerEntry> weeklyPlayers = new List<PlayerEntry>();
    private List<PlayerEntry> allTimePlayers = new List<PlayerEntry>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GenerateFakePlayers();
        }
    }
    
    private void GenerateFakePlayers()
    {

        dailyPlayers.Add(new PlayerEntry { username = "MoonBoy", balance = 12000f });
        dailyPlayers.Add(new PlayerEntry { username = "Degenerate7", balance = 5000f });
        dailyPlayers.Add(new PlayerEntry { username = "PepeTheFrog", balance = 1500f });
        dailyPlayers.Add(new PlayerEntry { username = "NewGuy99", balance = 800f });
        dailyPlayers.Add(new PlayerEntry { username = "LuckyRoller", balance = 400f });
        

        weeklyPlayers.Add(new PlayerEntry { username = "CasinoKing", balance = 45000f });
        weeklyPlayers.Add(new PlayerEntry { username = "xX_Sniper_Xx", balance = 25000f });
        weeklyPlayers.Add(new PlayerEntry { username = "HighRoller2", balance = 18000f });
        weeklyPlayers.AddRange(dailyPlayers); // Include daily in weekly


        allTimePlayers.Add(new PlayerEntry { username = "CryptoWhale", balance = 500000f });
        allTimePlayers.Add(new PlayerEntry { username = "Satoshi99", balance = 250000f });
        allTimePlayers.Add(new PlayerEntry { username = "LuckyDoge", balance = 125000f });
        allTimePlayers.Add(new PlayerEntry { username = "DiamondHands", balance = 75000f });
        allTimePlayers.AddRange(weeklyPlayers); // Include weekly in all-time
    }
    
    public List<PlayerEntry> GetTopPlayers(int tabIndex = 2) // 0=Daily, 1=Weekly, 2=AllTime
    {
        List<PlayerEntry> source = tabIndex == 0 ? dailyPlayers : (tabIndex == 1 ? weeklyPlayers : allTimePlayers);
        List<PlayerEntry> allPlayers = new List<PlayerEntry>(source);
        

        if (PlayerManager.Instance != null && AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn)
        {
            allPlayers.Add(new PlayerEntry { 
                username = AuthManager.Instance.currentUser + " (You)", 
                balance = PlayerManager.Instance.balance 
            });
        }
        

        allPlayers = allPlayers.OrderByDescending(p => p.balance).ToList();
        
        return allPlayers;
    }
}
