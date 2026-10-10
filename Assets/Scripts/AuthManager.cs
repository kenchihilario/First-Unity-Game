using UnityEngine;
using System;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    public bool IsLoggedIn { get; private set; } = false;
    public string CurrentPhoneNumber { get; private set; } = "";

    public Action<bool> OnLoginStateChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool Register(string phoneNumber, string password)
    {
        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password)) return false;
        if (PlayerPrefs.HasKey("User_" + phoneNumber)) return false; 

        PlayerPrefs.SetString("User_" + phoneNumber, password);
        PlayerPrefs.SetFloat("Balance_" + phoneNumber, 1000f); 
        PlayerPrefs.Save();
        return true;
    }

    public bool Login(string phoneNumber, string password)
    {
        if (PlayerPrefs.HasKey("User_" + phoneNumber))
        {
            if (PlayerPrefs.GetString("User_" + phoneNumber) == password)
            {
                IsLoggedIn = true;
                CurrentPhoneNumber = phoneNumber;
                if (PlayerManager.Instance != null)
                {
                    PlayerManager.Instance.balance = PlayerPrefs.GetFloat("Balance_" + phoneNumber);
                }
                OnLoginStateChanged?.Invoke(true);
                return true;
            }
        }
        return false;
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
