using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Equippables/Weapons/Staff")]
[System.Serializable]
public class Staff : Weapon
{
    public GameObject projectile;
}
