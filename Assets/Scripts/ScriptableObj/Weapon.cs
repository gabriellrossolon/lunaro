using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Equippables/Weapon")]
public class Weapon : ItemBase
{
    public enum WeaponType { Empty, Sword, Staff, Bow, Hammer, GreatHammer, GreatSword }
    [Header("Weapon Type")]
    public WeaponType weaponType;

    public int damage;
    public GameObject weaponPrefab;

    [Header("Resource cost for Attack with this (Stamina or Mana)")]
    public float resourceCost;

    [Header("If Ranged")]
    public GameObject projectile;

    [Header("If Staff")]
    public GameObject beam;

    [Header("If GreatHammer")]
    public GameObject earthquake;

    [Header("If GreatSword")]
    public GameObject circularCut;

}
