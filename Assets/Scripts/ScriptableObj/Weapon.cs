using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class Weapon : ItemBase
{
    [Header("Weapon Type")]
    public bool sword;
    public bool staff;

    //public Sprite icon;

    //public string weaponName;
    public int damage;
    public GameObject weaponPrefab;

    [Header("If Ranged")]
    public GameObject projectile;
}