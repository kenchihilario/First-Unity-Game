using UnityEngine;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    public static AuthUIManager Instance;

    [Header("Auth Panels")]
    public GameObject authOverlay;
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Bank Panels")]
    public GameObject bankOverlay;
    public GameObject depositPanel;
    public GameObject withdrawPanel;

    [Header("Account Settings")]
    public GameObject accSettingsOverlay;

    [Header("Auth Inputs")]
    public InputField loginPhoneInput;
    public InputField loginPasswordInput;
    public Text loginErrorText;

    public InputField registerPhoneInput;
    public InputField registerPasswordInput;
    public Text registerErrorText;

    [Header("Bank Inputs")]
    public InputField depositAmountInput;
    public InputField withdrawAmountInput;
    public Text bankErrorText;

    [Header("Top Bar Elements")]
    public GameObject loggedOutGroup;
    public GameObject loggedInGroup;
    public Text profileText;

    private bool _wired = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
#if ENABLE_INPUT_SYSTEM
        var inputModule = FindAnyObjectByType<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (inputModule != null && (inputModule.point == null || inputModule.leftClick == null))
        {
            inputModule.AssignDefaultActions();
        }
#endif
        WireAllButtons();

        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoginStateChanged -= HandleLoginState;
            AuthManager.Instance.OnLoginStateChanged += HandleLoginState;
            HandleLoginState(AuthManager.Instance.IsLoggedIn);
        }
        else
        {
            HandleLoginState(false);
        }
        CloseAll();
    }

    public void WireAllButtons()
    {
        if (_wired) return;
        _wired = true;

        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (var btn in allButtons)
        {
            string n = btn.gameObject.name;
            Transform p = btn.transform.parent;
            string pName = p != null ? p.name : "";

            if (n.Equals("LoginBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                btn.onClick.RemoveListener(ShowLogin);
                btn.onClick.AddListener(ShowLogin);
            }
            else if (n.Equals("RegisterBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                btn.onClick.RemoveListener(ShowRegister);
                btn.onClick.AddListener(ShowRegister);
            }
            else if (n.Equals("DepositBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                btn.onClick.RemoveListener(ShowDeposit);
                btn.onClick.AddListener(ShowDeposit);
            }
            else if (n.Equals("WithdrawBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                btn.onClick.RemoveListener(ShowWithdraw);
                btn.onClick.AddListener(ShowWithdraw);
            }
            else if (n.Equals("LogoutBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                btn.onClick.RemoveListener(OnLogoutClick);
                btn.onClick.AddListener(OnLogoutClick);
            }
            else if (n.Equals("GuestBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                btn.onClick.RemoveListener(OnGuestLogin);
                btn.onClick.AddListener(OnGuestLogin);
            }
            else if (n.Equals("SwitchRegBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                btn.onClick.RemoveListener(ShowRegister);
                btn.onClick.AddListener(ShowRegister);
            }
            else if (n.Equals("SwitchLogBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                btn.onClick.RemoveListener(ShowLogin);
                btn.onClick.AddListener(ShowLogin);
            }
            else if (n.Equals("SubmitBtn", System.StringComparison.OrdinalIgnoreCase) || n.Equals("ActionBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                if (pName.IndexOf("login", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    btn.onClick.RemoveListener(OnLoginSubmit);
                    btn.onClick.AddListener(OnLoginSubmit);
                }
                else if (pName.IndexOf("register", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    btn.onClick.RemoveListener(OnRegisterSubmit);
                    btn.onClick.AddListener(OnRegisterSubmit);
                }
                else if (pName.IndexOf("deposit", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    btn.onClick.RemoveListener(OnDepositSubmit);
                    btn.onClick.AddListener(OnDepositSubmit);
                }
                else if (pName.IndexOf("withdraw", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    btn.onClick.RemoveListener(OnWithdrawSubmit);
                    btn.onClick.AddListener(OnWithdrawSubmit);
                }
            }
            else if (n.Equals("CloseBtn", System.StringComparison.OrdinalIgnoreCase))
            {
                if (pName.IndexOf("panel", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    pName.IndexOf("overlay", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    pName.IndexOf("auth", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    pName.IndexOf("bank", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    btn.onClick.RemoveListener(CloseAll);
                    btn.onClick.AddListener(CloseAll);
                }
            }
        }
    }

    void Update()
    {
        bool loggedIn = AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn;
        if (loggedOutGroup != null && loggedOutGroup.activeSelf == loggedIn)
        {
            loggedOutGroup.SetActive(!loggedIn);
        }
        if (loggedInGroup != null && loggedInGroup.activeSelf != loggedIn)
        {
            loggedInGroup.SetActive(loggedIn);
        }
        if (loggedIn && profileText != null && (string.IsNullOrEmpty(profileText.text) || profileText.text.Contains("Unknown")))
        {
            string user = AuthManager.Instance != null ? AuthManager.Instance.CurrentPhoneNumber : "Kenchi";
            profileText.text = $"<color=#AAAAAA>{user}</color>";
        }
    }

    void HandleLoginState(bool isLoggedIn)
    {
        if (loggedOutGroup) loggedOutGroup.SetActive(!isLoggedIn);
        if (loggedInGroup) loggedInGroup.SetActive(isLoggedIn);

        if (isLoggedIn && profileText != null)
        {
            string user = AuthManager.Instance != null ? AuthManager.Instance.CurrentPhoneNumber : "Kenchi";
            profileText.text = $"<color=#AAAAAA>{user}</color>";
            CloseAll();
        }
    }

    public void ShowLogin()
    {
        CloseAll();
        if (authOverlay) { authOverlay.SetActive(true); authOverlay.transform.SetAsLastSibling(); }
        if (loginPanel) { loginPanel.SetActive(true); loginPanel.transform.SetAsLastSibling(); }
        if (loginErrorText) loginErrorText.text = "";
        if (loginPhoneInput != null && string.IsNullOrEmpty(loginPhoneInput.text)) loginPhoneInput.text = "Kenchi";
        if (loginPasswordInput != null && string.IsNullOrEmpty(loginPasswordInput.text)) loginPasswordInput.text = "1234";
    }

    public void ShowRegister()
    {
        CloseAll();
        if (authOverlay) { authOverlay.SetActive(true); authOverlay.transform.SetAsLastSibling(); }
        if (registerPanel) { registerPanel.SetActive(true); registerPanel.transform.SetAsLastSibling(); }
        if (registerErrorText) registerErrorText.text = "";
        if (registerPhoneInput != null && string.IsNullOrEmpty(registerPhoneInput.text)) registerPhoneInput.text = "Kenchi";
        if (registerPasswordInput != null && string.IsNullOrEmpty(registerPasswordInput.text)) registerPasswordInput.text = "1234";
    }

    public void ShowDeposit() { CloseAll(); if(bankOverlay) { bankOverlay.SetActive(true); bankOverlay.transform.SetAsLastSibling(); } if(depositPanel) { depositPanel.SetActive(true); depositPanel.transform.SetAsLastSibling(); } if(bankErrorText) bankErrorText.text = ""; }
    public void ShowWithdraw() { CloseAll(); if(bankOverlay) { bankOverlay.SetActive(true); bankOverlay.transform.SetAsLastSibling(); } if(withdrawPanel) { withdrawPanel.SetActive(true); withdrawPanel.transform.SetAsLastSibling(); } if(bankErrorText) bankErrorText.text = ""; }
    public void ShowAccountSettings() { CloseAll(); if(accSettingsOverlay) { accSettingsOverlay.SetActive(true); accSettingsOverlay.transform.SetAsLastSibling(); } }

    public void CloseAll()
    {
        if(authOverlay) authOverlay.SetActive(false);
        if(loginPanel) loginPanel.SetActive(false);
        if(registerPanel) registerPanel.SetActive(false);
        if(bankOverlay) bankOverlay.SetActive(false);
        if(depositPanel) depositPanel.SetActive(false);
        if(withdrawPanel) withdrawPanel.SetActive(false);
        if(accSettingsOverlay) accSettingsOverlay.SetActive(false);
    }

    public void OnLoginSubmit()
    {
        string phone = loginPhoneInput != null ? loginPhoneInput.text.Trim() : "";
        string pass = loginPasswordInput != null ? loginPasswordInput.text.Trim() : "";

        if (string.IsNullOrEmpty(phone)) phone = "Kenchi";
        if (string.IsNullOrEmpty(pass)) pass = "1234";

        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.Login(phone, pass);
        }
        CloseAll();
    }

    public void OnRegisterSubmit()
    {
        string phone = registerPhoneInput != null ? registerPhoneInput.text.Trim() : "";
        string pass = registerPasswordInput != null ? registerPasswordInput.text.Trim() : "";

        if (string.IsNullOrEmpty(phone)) phone = "Kenchi";
        if (string.IsNullOrEmpty(pass)) pass = "1234";

        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.Register(phone, pass);
            AuthManager.Instance.Login(phone, pass);
        }
        CloseAll();
    }

    public void OnGuestLogin()
    {
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.GuestLogin();
            CloseAll();
        }
    }

    public void OnDepositSubmit()
    {
        if (float.TryParse(depositAmountInput.text, out float amt) && amt > 0)
        {
            PlayerManager.Instance.Deposit(amt);
            CloseAll();
        }
        else if (bankErrorText != null) bankErrorText.text = "Invalid amount!";
    }

    public void OnWithdrawSubmit()
    {
        if (float.TryParse(withdrawAmountInput.text, out float amt) && amt > 0)
        {
            if (PlayerManager.Instance.balance >= amt)
            {
                PlayerManager.Instance.Withdraw(amt);
                CloseAll();
            }
            else if (bankErrorText != null) bankErrorText.text = "Insufficient balance!";
        }
        else if (bankErrorText != null) bankErrorText.text = "Invalid amount!";
    }

    public void OnLogoutClick()
    {
        AuthManager.Instance.Logout();
    }
}
