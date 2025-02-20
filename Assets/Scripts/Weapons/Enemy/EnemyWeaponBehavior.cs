using UnityEngine;
using System.Collections;

public class EnemyWeaponBehavior : MonoBehaviour
{
    public enum WeaponType { Empty, Sword, Staff}
    public WeaponType weaponType;

    [SerializeField] private bool alreadyTriggered = false;

    [SerializeField] private Collider _coll;
    [HideInInspector] public EnemyEffects _enemEffects;
    [HideInInspector] public EnemyStats _enemyStats;
    [HideInInspector] public AudioSource _enemySoundSource;

    [Header("Only if Melee")]
    public bool colliderEnabled = false;

    [Header("Only if Ranged")]
    public GameObject enemyProjectile;
    public GameObject projectileExit;

    [Header("Any Weapon")]
    public float damage;
    public bool attackBlockedByPlayer = false;

    private void Awake()
    {
        _coll = GetComponent<Collider>();
        _enemEffects = GetComponentInParent<EnemyEffects>();
        _enemyStats = GetComponentInParent<EnemyStats>();
        _enemySoundSource = GetComponentInParent<AudioSource>();
    }

    private void Start()
    {
        damage = _enemyStats.attackDamage;
    }

    //Comportamentos caso a arma seja Melee
    public void CallCollider()
    {
        colliderEnabled = !colliderEnabled;

        if (_coll != null)
        {
            _coll.enabled = colliderEnabled;
            Debug.Log("Collider habilitado");
        }
        else
        {
            Debug.Log("Collider não existe");
        }


        if (colliderEnabled == false)
            attackBlockedByPlayer = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!attackBlockedByPlayer && other.gameObject.CompareTag("Shield"))
        {
            _enemySoundSource.PlayOneShot(other.GetComponent<ShieldBehavior>().blockedAttackSound);
            Instantiate(_enemEffects.attackImpactFx, other.transform.position, Quaternion.identity);
            other.GetComponent<ShieldBehavior>().ConsumeStamina();
            attackBlockedByPlayer = true; // Marca o ataque como bloqueado
        }
        else if (!attackBlockedByPlayer && other.gameObject.CompareTag("Player"))
        {
            if (alreadyTriggered) return;
            alreadyTriggered = true;

            _enemySoundSource.PlayOneShot(_enemEffects.attackImpactSound);
            PlayerManager.Instance.DoDamage(damage);
            Instantiate(_enemEffects.attackImpactFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            StartCoroutine(ResetTrigger());
        }
    }

    private IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(0.3f);
        alreadyTriggered = false;
    }

    //Comportamentos caso a arma seja ranged (solte um projectile)
    public void CallProjectile()
    {
        GameObject projectile = Instantiate(enemyProjectile, projectileExit.transform.position, Quaternion.identity);
        EnemyProjectileBehavior enemProjBeh = projectile.GetComponent<EnemyProjectileBehavior>();
        enemProjBeh.damage = damage;
        enemProjBeh.weaponBehavior = this;
    }
}

