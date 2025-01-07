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
            if (weaponType == Weapon.WeaponType.Staff)
            {
                ExecuteStaffAttack();
            }
            else if (weaponType == Weapon.WeaponType.Sword)
            {
                ExecuteSwordAttack();
            }
            else if (weaponType == Weapon.WeaponType.Bow)
            {

            }
            attack = false;
        }

        if (weaponType == Weapon.WeaponType.Sword) { weaponAnim.SetBool("Blocking", block); }
    }

    private void ExecuteStaffAttack()
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);
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

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.7f, 1f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSound);
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

