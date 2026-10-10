using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardUIManager : MonoBehaviour
{
    public Text[] rankTexts = new Text[10];
    public Text[] nameTexts = new Text[10];
    public Text[] balanceTexts = new Text[10];
    public Image[] rowBackgrounds = new Image[10];
    
    public Button closeButton;
    public Button btnDaily;
    public Button btnWeekly;
    public Button btnAllTime;
    
    public GameObject leaderboardPanel;
    private int currentTab = 2; // 0=Daily, 1=Weekly, 2=AllTime
    
    private string[] randomAvatars = new string[] { "🤠", "🤖", "👽", "👻", "🐵", "🦁", "🦊", "🐯", "🐼", "😎" };
    
    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => {
                if (CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
                leaderboardPanel.SetActive(false);
            });
        }
        
        if (btnDaily != null) btnDaily.onClick.AddListener(() => SetTab(0));
        if (btnWeekly != null) btnWeekly.onClick.AddListener(() => SetTab(1));
        if (btnAllTime != null) btnAllTime.onClick.AddListener(() => SetTab(2));
    }
    
    void SetTab(int index)
    {
        if (CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
        currentTab = index;
        RefreshLeaderboard();
    }
    
    void OnEnable()
    {
        RefreshLeaderboard();
        StartCoroutine(AnimateRows());
    }
    
    System.Collections.IEnumerator AnimateRows()
    {
        for (int i = 0; i < 10; i++)
        {
            if (rowBackgrounds[i] != null && rowBackgrounds[i].gameObject.activeSelf)
            {
                RectTransform rt = rowBackgrounds[i].GetComponent<RectTransform>();
                Vector2 originalPos = rt.anchoredPosition;
                rt.anchoredPosition = new Vector2(originalPos.x - 500f, originalPos.y); // slide from left
                
                float duration = 0.3f;
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    float ease = 1f - Mathf.Pow(1f - t, 3f);
                    rt.anchoredPosition = Vector2.Lerp(new Vector2(originalPos.x - 500f, originalPos.y), originalPos, ease);
                    yield return null;
                }
                rt.anchoredPosition = originalPos;
                if (CasinoAudio.Instance && i < 3) CasinoAudio.Instance.PlayTick(); 
            }
            yield return new WaitForSeconds(0.05f); 
        }
    }
    
    public void RefreshLeaderboard()
    {
        if (LeaderboardManager.Instance == null) return;
        

        if (btnDaily != null) btnDaily.GetComponent<Image>().color = currentTab == 0 ? new Color(0.2f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.3f);
        if (btnWeekly != null) btnWeekly.GetComponent<Image>().color = currentTab == 1 ? new Color(0.2f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.3f);
        if (btnAllTime != null) btnAllTime.GetComponent<Image>().color = currentTab == 2 ? new Color(0.2f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.3f);
        
        List<LeaderboardManager.PlayerEntry> topPlayers = LeaderboardManager.Instance.GetTopPlayers(currentTab);
        
        for (int i = 0; i < 10; i++)
        {
            if (i < topPlayers.Count)
            {
                rankTexts[i].text = $"#{i + 1}";
                
                string avatar = topPlayers[i].username.EndsWith("(You)") ? "😎" : randomAvatars[Mathf.Abs(topPlayers[i].username.GetHashCode()) % randomAvatars.Length];
                nameTexts[i].text = $"{avatar}  {topPlayers[i].username}";
                balanceTexts[i].text = $"${topPlayers[i].balance:F2}";
                

                if (topPlayers[i].username.EndsWith("(You)"))
                {
                    rowBackgrounds[i].color = new Color(0.2f, 0.5f, 0.2f, 0.8f);
                    nameTexts[i].color = Color.white;
                    balanceTexts[i].color = new Color(0f, 1f, 0.6f);
                }
                else
                {
                    rowBackgrounds[i].color = (i % 2 == 0) ? new Color(0.15f, 0.15f, 0.2f, 0.8f) : new Color(0.1f, 0.1f, 0.15f, 0.8f);
                    nameTexts[i].color = new Color(0.8f, 0.8f, 0.8f);
                    balanceTexts[i].color = Color.white;
                }
                

                if (i == 0) rankTexts[i].color = new Color(1f, 0.84f, 0f); // Gold
                else if (i == 1) rankTexts[i].color = new Color(0.75f, 0.75f, 0.75f); // Silver
                else if (i == 2) rankTexts[i].color = new Color(0.8f, 0.5f, 0.2f); // Bronze
                else rankTexts[i].color = new Color(0.5f, 0.5f, 0.5f);
                
                rowBackgrounds[i].gameObject.SetActive(true);
            }
            else
            {
                rowBackgrounds[i].gameObject.SetActive(false);
            }
        }
    }
}
