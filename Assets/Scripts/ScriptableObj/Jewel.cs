using UnityEngine;
public class Jewel : ItemBase
{
    [System.Serializable]
    public enum StatType { Damage, HP, Stamina, Mana }
    public StatType statToModify;
    public enum EffectType { IncreaseBase, IncreaseRegeneration, IncreasePercent }
    public EffectType effectType;

    public float valueToAdd; 
}
