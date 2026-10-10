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

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnLoginStateChanged += HandleLoginState;
            HandleLoginState(AuthManager.Instance.IsLoggedIn);
        }
        CloseAll();
    }

    void HandleLoginState(bool isLoggedIn)
    {
        if (loggedOutGroup) loggedOutGroup.SetActive(!isLoggedIn);
        if (loggedInGroup) loggedInGroup.SetActive(isLoggedIn);
        
        if (isLoggedIn && profileText != null)
        {
            profileText.text = $"<color=#AAAAAA>{AuthManager.Instance.CurrentPhoneNumber}</color>";
            CloseAll();
        }
    }

    public void ShowLogin() { CloseAll(); if(authOverlay) authOverlay.SetActive(true); if(loginPanel) loginPanel.SetActive(true); if(loginErrorText) loginErrorText.text = ""; }
    public void ShowRegister() { CloseAll(); if(authOverlay) authOverlay.SetActive(true); if(registerPanel) registerPanel.SetActive(true); if(registerErrorText) registerErrorText.text = ""; }
    public void ShowDeposit() { CloseAll(); if(bankOverlay) bankOverlay.SetActive(true); if(depositPanel) depositPanel.SetActive(true); if(bankErrorText) bankErrorText.text = ""; }
    public void ShowWithdraw() { CloseAll(); if(bankOverlay) bankOverlay.SetActive(true); if(withdrawPanel) withdrawPanel.SetActive(true); if(bankErrorText) bankErrorText.text = ""; }
    public void ShowAccountSettings() { CloseAll(); if(accSettingsOverlay) accSettingsOverlay.SetActive(true); }
    
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
        if (AuthManager.Instance.Login(loginPhoneInput.text, loginPasswordInput.text))
        {
            CloseAll();
        }
        else if (loginErrorText != null)
        {
            loginErrorText.text = "Invalid phone or password!";
        }
    }

    public void OnRegisterSubmit()
    {
        if (AuthManager.Instance.Register(registerPhoneInput.text, registerPasswordInput.text))
        {
            AuthManager.Instance.Login(registerPhoneInput.text, registerPasswordInput.text);
        }
        else if (registerErrorText != null)
        {
            registerErrorText.text = "Registration failed! Number might exist.";
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
