using UnityEngine;

[CreateAssetMenu(fileName = "NewSecondary", menuName = "Equippables/Secondarys/Book")]
[System.Serializable]
public class Book : Secondary
{
    [Header("Secondary Skill")]
    public Spell spell;
}
