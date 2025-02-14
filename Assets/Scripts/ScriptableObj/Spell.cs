using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Powers/Spell")]
public class Spell : ScriptableObject
{
    public string spellName;
    public Sprite spellIcon;
    public float spellDamage;
    public GameObject spellPrefab;
    public float spellResourceCost;

    public enum SpellType
    {
        None,
        Bolide,
        MagicMissile,
        Slit
    }

    public SpellType spellType;
}
