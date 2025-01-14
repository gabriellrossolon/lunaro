using UnityEngine;

[CreateAssetMenu(fileName = "NewJewel", menuName = "Equippables/Jewel")]
public class Jewel : ItemBase
{
    public enum JewelType { Empty, Ring, Necklace, Amulet }

    public enum StatType { Damage, HP, Stamina, Mana }

    public enum EffectType { IncreaseBase, IncreaseRegeneration, IncreasePercent }

    [Header("Jewel Type")]
    public JewelType jewelType;    
    public StatType statToModify;
    public EffectType effectType;

    public float valueToAdd; 
}
