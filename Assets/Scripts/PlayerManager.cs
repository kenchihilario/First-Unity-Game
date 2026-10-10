using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public float balance = 0f;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
