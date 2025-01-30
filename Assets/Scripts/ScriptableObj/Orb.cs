using UnityEngine;

[CreateAssetMenu(fileName = "NewSecondary", menuName = "Equippables/Secondarys/Orb")]
public class Orb : Secondary
{
    [Header("Secondary Skill")]
    public Spell spell;
}
