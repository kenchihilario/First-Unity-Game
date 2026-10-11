using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<PlayerManager>();
            }
            return _instance;
        }
    }

    public float balance = 10000f;

    void Awake()
    {
        if (_instance == null) _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        string user = PlayerPrefs.GetString("LastLoggedInUser", "Kenchi");
        float savedBal = PlayerPrefs.GetFloat("Balance_" + user, 10000f);
        balance = savedBal > 0 ? savedBal : 10000f;
    }
    public bool PlaceBet(float amount)
    {
        if (amount <= 0) return false;
        if (balance >= amount)
        {
            balance -= amount;
            if (AuthManager.Instance != null) AuthManager.Instance.SaveBalance();
            return true;
        }
        return false;
    }
    public void AddWinnings(float amount)
    {
        if (amount > 0)
        {
            balance += amount;
            if (AuthManager.Instance != null) AuthManager.Instance.SaveBalance();
        }
    }

    public void Deposit(float amount)
    {
        if (amount > 0)
        {
            balance += amount;
            if (AuthManager.Instance != null) AuthManager.Instance.SaveBalance();
        }
    }

    public void Withdraw(float amount)
    {
        if (amount > 0 && balance >= amount)
        {
            balance -= amount;
            if (AuthManager.Instance != null) AuthManager.Instance.SaveBalance();
        }
    }
}
