using UnityEngine;
public class Jewel : ItemBase
{
    public enum StatType { Damage, HP, Stamina, Mana }

    public enum EffectType { IncreaseBase, IncreaseRegeneration, IncreasePercent }

    [Header("Jewel Type")]
    public StatType statToModify;
    public EffectType effectType;

    public float valueToAdd; 
}
