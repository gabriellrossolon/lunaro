using UnityEngine;

[System.Serializable]
public class Weapon : ItemBase
{
    public int damage;
    public GameObject weaponPrefab;

    [Header("Resource cost for Attack with this (Stamina or Mana)")]
    public float resourceCost;

    [Header("Weapon Skill")]
    public Skill weaponSkill;
}
