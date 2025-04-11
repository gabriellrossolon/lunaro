using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Equippables/Weapons/Bow")]
[System.Serializable]
public class Bow : Weapon
{
    public GameObject projectile;
}
