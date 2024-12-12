using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    [HideInInspector] public bool attack;
    [HideInInspector] public bool block;
    private Animator weaponAnim;

    [Header("Only if Ranged")]
    public GameObject projectilePrefab;
    public GameObject projectileExitPoint;

    [Header("Weapon Type Equipped")]
    public bool sword = false;
    public bool staff = false;
    public bool bow = false;

    private void Awake()
    {
        weaponAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attack)
        {
            if (staff || bow)
            {
                ExecuteRangedAttack();
            }
            else if (sword)
            {
                ExecuteSwordAttack();
            }
            attack = false;
        }

        weaponAnim.SetBool("Blocking", block);
    }

    private void ExecuteRangedAttack()
    {
        weaponAnim.SetTrigger("staffAttack"); 
    }

    private void ExecuteSwordAttack()
    {
        weaponAnim.SetTrigger("swordAttack");
    }

    public void CallProjectile()
    {
        GameObject newProjectile = Instantiate(projectilePrefab, projectileExitPoint.transform.position, Quaternion.identity);
        ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
        projectileScript.SetDirection(transform.forward);
    }

    public void ActiveCollider()
    {
        SwordBehavior swordBehavior = GetComponentInChildren<SwordBehavior>();
        swordBehavior.colliderEnabled = !swordBehavior.colliderEnabled;
    }
}
