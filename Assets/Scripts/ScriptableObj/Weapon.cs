using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class Weapon : ItemBase
{
    public enum WeaponType { Empty, Sword, Staff, Bow }
    [Header("Weapon Type")]
    public WeaponType weaponType;

    public int damage;
    public GameObject weaponPrefab;

    [Header("Resource cost for Attack with this (Stamina or Mana)")]
    public float resourceCost;

    [Header("If Ranged")]
    public GameObject projectile;
    public GameObject beam;
    
}
