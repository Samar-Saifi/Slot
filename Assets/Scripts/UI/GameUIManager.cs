using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text m_scoreText;
    [SerializeField] private TMP_Text m_BetText;

    void Awake()
    {
        m_scoreText.text = BettingManager.instance.m_balance.ToString();
        BettingManager.instance.OnBalanceChanged += UpdateBalance;
        BettingManager.instance.OnBetChanged += UpdateBet;
    }

    private void UpdateBet(int betAmount)
    {
        m_BetText.text = $"Bet: {betAmount}";
    }
    
    private void UpdateBalance(int balance)
    {
        m_scoreText.text = balance.ToString();
    }
}
