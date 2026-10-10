using UnityEngine;
using UnityEngine.UI;
public class CasinoLobbyManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject crashPanel;
    public GameObject roulettePanel;
    public GameObject slotsPanel;
    public GameObject minesPanel;
    public GameObject plinkoPanel;
    [Header("Lobby UI")]
    public Text lobbyBalanceText;
    public Button playCrashButton;
    public Button playRouletteButton;
    public Button playSlotsButton;
    public Button playMinesButton;
    public Button playPlinkoButton;

    [Header("Top Bar Buttons (Auto-Wired)")]
    public Button loginBtn;
    public Button registerBtn;
    public Button depositBtn;
    public Button withdrawBtn;
    public Button logoutBtn;

    void Start()
    {
        ShowLobby();
        if (playCrashButton != null) playCrashButton.onClick.AddListener(StartCrash);
        if (playRouletteButton != null) playRouletteButton.onClick.AddListener(StartRoulette);
        if (playSlotsButton != null) playSlotsButton.onClick.AddListener(StartSlots);
        if (playMinesButton != null) playMinesButton.onClick.AddListener(StartMines);
        if (playPlinkoButton != null) playPlinkoButton.onClick.AddListener(StartPlinko);

        // UI Buttons are now wired by CasinoSetupWizard dynamically!
    }
    void Update()
    {
        if (PlayerManager.Instance != null && lobbyBalanceText != null)
        {
            lobbyBalanceText.text = $"<color=#00FFAA>${PlayerManager.Instance.balance:F2}</color>";
        }
    }
    public void ShowLobby()
    {
        if(lobbyPanel) lobbyPanel.SetActive(true);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
    }
    private bool CheckLogin()
    {
        if (AuthManager.Instance != null && !AuthManager.Instance.IsLoggedIn)
        {
            if (AuthUIManager.Instance != null) AuthUIManager.Instance.ShowLogin();
            return false;
        }
        return true;
    }
    public void StartCrash()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(true);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
    }
    public void StartRoulette()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(true);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
    }
    public void StartSlots()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(true);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(false);
    }
    public void StartMines()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(true);
        if(plinkoPanel) plinkoPanel.SetActive(false);
    }
    public void StartPlinko()
    {
        if (!CheckLogin()) return;
        if(lobbyPanel) lobbyPanel.SetActive(false);
        if(crashPanel) crashPanel.SetActive(false);
        if(roulettePanel) roulettePanel.SetActive(false);
        if(slotsPanel) slotsPanel.SetActive(false);
        if(minesPanel) minesPanel.SetActive(false);
        if(plinkoPanel) plinkoPanel.SetActive(true);
    }
}
