using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class Weapon : ItemBase
{
    public enum WeaponType { Empty, Sword, Staff, Bow }
    [Header("Weapon Type")]
    public WeaponType weaponType;

    public int damage;
    public GameObject weaponPrefab;

    [Header("If Ranged")]
    public GameObject projectile;
}
