using UnityEngine;

public class BettingManager : MonoBehaviour
{
    [Header("Starting State")]
    [SerializeField] private int m_startingBalance = 30;

    private int[] m_availableBets = {5, 10, 20};

    public static BettingManager instance;
    public int m_balance { get; private set; }
    public int m_betIndex = 0;

    public System.Action<int> OnBalanceChanged;

    public System.Action<int> OnBetChanged;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
        m_balance =  m_startingBalance;
        m_betIndex = 0;
    }

    public void IncreaseBet()
    {
        m_betIndex = Mathf.Clamp(m_betIndex + 1, 0, m_availableBets.Length-1);
        OnBetChanged?.Invoke(m_availableBets[m_betIndex]);
    }

    public void DecreaseBet()
    {
        m_betIndex = Mathf.Clamp(m_betIndex - 1, 0, m_availableBets.Length-1);
        OnBetChanged?.Invoke(m_availableBets[m_betIndex]);
    }

    public bool TryPlaceBet()
    {
        if (m_balance < m_availableBets[m_betIndex])
            return false;

        m_balance -= m_availableBets[m_betIndex];
        OnBalanceChanged?.Invoke(m_balance);
        return true;
    }

    public int GetCurrentBet()
    {
        return m_availableBets[m_betIndex];
    }
    
    public void AddWinnings(int amount)
    {
        if (amount <= 0f) return;

        m_balance += amount;
        OnBalanceChanged?.Invoke(m_balance);
    }
}
