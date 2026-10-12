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
    private int currentTab = 2;

    private string[] randomAvatars = new string[] { "🤠", "🤖", "👽", "👻", "🐵", "🦁", "🦊", "🐯", "🐼", "😎" };

    void Start()
    {
        if (leaderboardPanel == null) leaderboardPanel = gameObject;
        if (closeButton == null) closeButton = transform.Find("CloseBtn")?.GetComponent<Button>() ?? transform.Find("Header/CloseBtn")?.GetComponent<Button>();
        if (btnDaily == null) btnDaily = transform.Find("Tabs/BtnDaily")?.GetComponent<Button>() ?? transform.Find("BtnDaily")?.GetComponent<Button>();
        if (btnWeekly == null) btnWeekly = transform.Find("Tabs/BtnWeekly")?.GetComponent<Button>() ?? transform.Find("BtnWeekly")?.GetComponent<Button>();
        if (btnAllTime == null) btnAllTime = transform.Find("Tabs/BtnAllTime")?.GetComponent<Button>() ?? transform.Find("BtnAllTime")?.GetComponent<Button>();

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => {
                if (CasinoAudio.Instance) CasinoAudio.Instance.PlayTick();
                if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
                else gameObject.SetActive(false);
            });
        }

        if (btnDaily != null)
        {
            btnDaily.onClick.RemoveAllListeners();
            btnDaily.onClick.AddListener(() => SetTab(0));
        }
        if (btnWeekly != null)
        {
            btnWeekly.onClick.RemoveAllListeners();
            btnWeekly.onClick.AddListener(() => SetTab(1));
        }
        if (btnAllTime != null)
        {
            btnAllTime.onClick.RemoveAllListeners();
            btnAllTime.onClick.AddListener(() => SetTab(2));
        }
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
            if (rowBackgrounds != null && i < rowBackgrounds.Length && rowBackgrounds[i] != null && rowBackgrounds[i].gameObject.activeSelf)
            {
                RectTransform rt = rowBackgrounds[i].GetComponent<RectTransform>();
                if (rt != null)
                {
                    Vector2 originalPos = rt.anchoredPosition;
                    rt.anchoredPosition = new Vector2(originalPos.x - 500f, originalPos.y);

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
                }
                if (CasinoAudio.Instance && i < 3) CasinoAudio.Instance.PlayTick();
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void RefreshLeaderboard()
    {
        if (LeaderboardManager.Instance == null) return;

        if (btnDaily != null) { Image img = btnDaily.GetComponent<Image>(); if (img != null) img.color = currentTab == 0 ? new Color(0.2f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.3f); }
        if (btnWeekly != null) { Image img = btnWeekly.GetComponent<Image>(); if (img != null) img.color = currentTab == 1 ? new Color(0.2f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.3f); }
        if (btnAllTime != null) { Image img = btnAllTime.GetComponent<Image>(); if (img != null) img.color = currentTab == 2 ? new Color(0.2f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.3f); }

        List<LeaderboardManager.PlayerEntry> topPlayers = LeaderboardManager.Instance.GetTopPlayers(currentTab);

        for (int i = 0; i < 10; i++)
        {
            if (topPlayers != null && i < topPlayers.Count)
            {
                if (rankTexts != null && i < rankTexts.Length && rankTexts[i] != null)
                {
                    rankTexts[i].text = $"#{i + 1}";
                    if (i == 0) rankTexts[i].color = new Color(1f, 0.84f, 0f);
                    else if (i == 1) rankTexts[i].color = new Color(0.75f, 0.75f, 0.75f);
                    else if (i == 2) rankTexts[i].color = new Color(0.8f, 0.5f, 0.2f);
                    else rankTexts[i].color = new Color(0.5f, 0.5f, 0.5f);
                }

                string avatar = topPlayers[i].username.EndsWith("(You)") ? "😎" : randomAvatars[Mathf.Abs(topPlayers[i].username.GetHashCode()) % randomAvatars.Length];

                if (nameTexts != null && i < nameTexts.Length && nameTexts[i] != null)
                {
                    nameTexts[i].text = $"{avatar}  {topPlayers[i].username}";
                    nameTexts[i].color = topPlayers[i].username.EndsWith("(You)") ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                }

                if (balanceTexts != null && i < balanceTexts.Length && balanceTexts[i] != null)
                {
                    balanceTexts[i].text = $"₱{topPlayers[i].balance:N2}";
                    balanceTexts[i].color = topPlayers[i].username.EndsWith("(You)") ? new Color(0f, 1f, 0.6f) : Color.white;
                }

                if (rowBackgrounds != null && i < rowBackgrounds.Length && rowBackgrounds[i] != null)
                {
                    if (topPlayers[i].username.EndsWith("(You)"))
                    {
                        rowBackgrounds[i].color = new Color(0.2f, 0.5f, 0.2f, 0.8f);
                    }
                    else
                    {
                        rowBackgrounds[i].color = (i % 2 == 0) ? new Color(0.15f, 0.15f, 0.2f, 0.8f) : new Color(0.1f, 0.1f, 0.15f, 0.8f);
                    }
                    rowBackgrounds[i].gameObject.SetActive(true);
                }
            }
            else
            {
                if (rowBackgrounds != null && i < rowBackgrounds.Length && rowBackgrounds[i] != null)
                {
                    rowBackgrounds[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
