using UnityEngine;

[CreateAssetMenu(fileName = "NewSecondary", menuName = "Equippables/Secondarys/Book")]
public class Book : Secondary
{
    [Header("Secondary Skill")]
    public Spell spell;
}
