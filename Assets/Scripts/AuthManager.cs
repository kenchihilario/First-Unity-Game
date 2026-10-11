using UnityEngine;
using System;

public class AuthManager : MonoBehaviour
{
    private static AuthManager _instance;
    public static AuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AuthManager>();
            }
            return _instance;
        }
    }

    public bool IsLoggedIn { get; private set; } = false;
    public string CurrentPhoneNumber { get; private set; } = "";
    public string currentUser => CurrentPhoneNumber;

    public Action<bool> OnLoginStateChanged;

    void Awake()
    {
        if (_instance == null) _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        PlayerPrefs.SetString("User_Kenchi", "1234");
        if (!PlayerPrefs.HasKey("Balance_Kenchi") || PlayerPrefs.GetFloat("Balance_Kenchi") <= 0)
        {
            PlayerPrefs.SetFloat("Balance_Kenchi", 10000f);
        }
        PlayerPrefs.SetString("User_demo", "1234");
        if (!PlayerPrefs.HasKey("Balance_demo") || PlayerPrefs.GetFloat("Balance_demo") <= 0)
        {
            PlayerPrefs.SetFloat("Balance_demo", 1000f);
        }
        PlayerPrefs.Save();
    }

    void Start()
    {
        AutoLogin();
    }

    public void AutoLogin()
    {
        if (IsLoggedIn) return;
        string username = PlayerPrefs.GetString("LastLoggedInUser", "Kenchi");
        if (string.IsNullOrEmpty(username)) username = "Kenchi";
        string pass = PlayerPrefs.GetString("User_" + username, "1234");
        Login(username, pass);
    }

    public void ForceLogin(string username = "Kenchi")
    {
        if (string.IsNullOrEmpty(username)) username = "Kenchi";
        Login(username, "1234");
    }

    public bool Register(string phoneNumber, string password)
    {
        if (string.IsNullOrEmpty(phoneNumber)) phoneNumber = "Kenchi";
        if (string.IsNullOrEmpty(password)) password = "1234";
        phoneNumber = phoneNumber.Trim();
        password = password.Trim();

        PlayerPrefs.SetString("User_" + phoneNumber, password);
        if (!PlayerPrefs.HasKey("Balance_" + phoneNumber) || PlayerPrefs.GetFloat("Balance_" + phoneNumber) <= 0)
        {
            PlayerPrefs.SetFloat("Balance_" + phoneNumber, 10000f);
        }
        PlayerPrefs.Save();
        return true;
    }

    public bool Login(string phoneNumber, string password)
    {
        if (string.IsNullOrEmpty(phoneNumber)) phoneNumber = "Kenchi";
        if (string.IsNullOrEmpty(password)) password = "1234";
        phoneNumber = phoneNumber.Trim();
        password = password.Trim();

        if (IsLoggedIn && CurrentPhoneNumber == phoneNumber)
        {
            return true;
        }

        PlayerPrefs.SetString("User_" + phoneNumber, password);

        float bal = PlayerPrefs.GetFloat("Balance_" + phoneNumber, 10000f);
        if (bal <= 0) bal = 10000f;
        PlayerPrefs.SetFloat("Balance_" + phoneNumber, bal);
        PlayerPrefs.SetString("LastLoggedInUser", phoneNumber);
        PlayerPrefs.Save();

        IsLoggedIn = true;
        CurrentPhoneNumber = phoneNumber;

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.balance = bal;
        }

        OnLoginStateChanged?.Invoke(true);
        return true;
    }

    public void GuestLogin()
    {
        string guestNum = "Guest_" + UnityEngine.Random.Range(1000, 9999);
        Register(guestNum, "guest");
        Login(guestNum, "guest");
    }

    public void Logout()
    {
        SaveBalance();
        IsLoggedIn = false;
        CurrentPhoneNumber = "";
        OnLoginStateChanged?.Invoke(false);
    }

    public void SaveBalance()
    {
        if (IsLoggedIn && PlayerManager.Instance != null)
        {
            PlayerPrefs.SetFloat("Balance_" + CurrentPhoneNumber, PlayerManager.Instance.balance);
            PlayerPrefs.Save();
        }
    }
}
