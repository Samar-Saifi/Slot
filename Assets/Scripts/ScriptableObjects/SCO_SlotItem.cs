using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Slot/Item")]
public class SCO_SlotItem : ScriptableObject
{
    [Tooltip("Sprite shown on the reel for this item")]
    public Sprite icon;
    
    public ItemType type;
    
    [Tooltip("Multiplier to be applied to the bet if player wins with this item")]
    [Min (0)]
    public float payoutMultiplier = 1;
    
    [Tooltip("Spawn weight for this item (Items with lower weight will be spawned much rarely than those with the higher weight)")]
    [Min(0)]
    public int weight = 1;
}

public enum ItemType
{
    Cherry,
    Bell,
    Bar,
    Seven
}