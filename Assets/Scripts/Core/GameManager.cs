using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }

    public void CheckSlots(ReelController[] reels)
    {
        if (reels == null || reels.Length == 0)
        {
            Debug.LogError("No reels supplied.");
            return;
        }

        SCO_SlotItem firstSlot = reels[0].GetReelSlot();

        if (firstSlot == null)
        {
            Debug.LogError("First reel has no slot.");
            return;
        }

        bool won = true;

        for (int i = 1; i < reels.Length; i++)
        {
            SCO_SlotItem slot = reels[i].GetReelSlot();

            if (slot == null)
            {
                Debug.LogError($"Reel {i} has no slot.");
                return;
            }

            if (slot.type != firstSlot.type)
            {
                won = false;
                break;
            }
        }
        
        if(!won) return;

        Debug.Log($"Won: {won}");
        int currentAmount = BettingManager.instance.m_balance;
        int earnings = currentAmount * (int)firstSlot.payoutMultiplier;
        BettingManager.instance.AddWinnings(earnings);
    }
}
