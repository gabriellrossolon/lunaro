using UnityEngine;

[CreateAssetMenu(fileName = "NewSecondary", menuName = "Equippables/Secondarys/Orb")]
[System.Serializable]
public class Orb : Secondary
{
    [Header("Secondary Skill")]
    public Spell spell;
}
