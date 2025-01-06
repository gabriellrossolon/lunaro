using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    [HideInInspector] public bool attack;
    [HideInInspector] public bool block = false;
    public bool canAttack = true;
    private Animator weaponAnim;

    [Header("Only if Melee")]
    private bool changeAttack;

    [Header("Only if Ranged")]
    public GameObject projectilePrefab;
    public GameObject projectileExitPoint;

    [Header("Weapon Type Equipped")]
    public Weapon.WeaponType weaponType; // Usando o enum WeaponType

    private void Awake()
    {
        weaponAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attack)
        {
            if (weaponType == Weapon.WeaponType.Staff || weaponType == Weapon.WeaponType.Bow)
            {
                ExecuteRangedAttack();
            }
            else if (weaponType == Weapon.WeaponType.Sword)
            {
                ExecuteSwordAttack();
            }
            attack = false;
        }

        if (weaponType == Weapon.WeaponType.Sword) { weaponAnim.SetBool("Blocking", block); }
    }

    private void ExecuteRangedAttack()
    {
        weaponAnim.SetTrigger("staffAttack");
    }

    public void ExecuteSwordAttack()
    {
        if (canAttack)
        {
            if (changeAttack)
            {
                weaponAnim.SetTrigger("swordAttackRL");
            }
            else
            {
                weaponAnim.SetTrigger("swordAttackUD");
            }
            changeAttack = !changeAttack;
        }
    }

    public void CanAttack()
    {
        canAttack = !canAttack;
    }

    public void CallProjectile()
    {
        GameObject newProjectile = Instantiate(projectilePrefab, projectileExitPoint.transform.position, Quaternion.identity);
        ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
        //projectileScript.damage = GetComponentInChildren<StaffBehavior>().damage;
        projectileScript.SetDirection(transform.forward);
    }

    public void ActiveCollider()
    {
        SwordBehavior swordBehavior = GetComponentInChildren<SwordBehavior>();
        swordBehavior.colliderEnabled = !swordBehavior.colliderEnabled;
    }
}

