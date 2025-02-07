using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private EnemyStats _enemyStats;
    private EnemyEffects _enemyEffects;
    private EnemyWeaponBehavior _enemyWeaponBehavior;
    private AudioSource _audioSource;

    private void Awake()
    {
        _enemyStats = GetComponent<EnemyStats>();
        _enemyEffects = GetComponent<EnemyEffects>();
        _enemyWeaponBehavior = GetComponentInChildren<EnemyWeaponBehavior>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void ActiveWeaponCollider()
    {
        _enemyWeaponBehavior.CallCollider();
    }

    public void AttackSwoosh()
    {
        _audioSource.PlayOneShot(_enemyEffects.attackSound);
    }


    public void ReableBlockedAttack()
    {
        EnemyWeaponBehavior[] weaponBehaviors = GetComponentsInChildren<EnemyWeaponBehavior>();

        foreach (var weaponBehavior in weaponBehaviors)
        {
            weaponBehavior.attackBlockedByPlayer = false;
        }
    }

    public void CallProjectile()
    {
        _enemyWeaponBehavior.CallProjectile();
    }

    public void DropXp()
    {
        for (int i = 0; i < _enemyStats.xpCarryed; i++)
        {
            GameObject xpOrb = Instantiate(_enemyEffects.xpOrb, transform.position, Quaternion.identity);
        }
    }
}
