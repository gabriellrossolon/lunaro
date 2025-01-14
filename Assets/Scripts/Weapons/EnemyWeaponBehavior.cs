using UnityEngine;

public class EnemyWeaponBehavior : MonoBehaviour
{
    public float damage;
    private MeshCollider _coll;
    private EnemyEffects _enemEffects;
    private EnemyStats _enemyStats;
    private AudioSource _enemySoundSource;
    public bool colliderEnabled = false;

    [SerializeField] private bool attackBlockedByPlayer = false;

    private void Awake()
    {
        _coll = GetComponent<MeshCollider>();
        _enemEffects = GetComponentInParent<EnemyEffects>();
        _enemyStats = GetComponentInParent<EnemyStats>();
        _enemySoundSource = GetComponentInParent<AudioSource>();
    }

    private void Start()
    {
        damage = _enemyStats.attackDamage;
    }

    public void CallCollider()
    {
        colliderEnabled = !colliderEnabled;
        if (_coll != null) { _coll.enabled = colliderEnabled; }

        if (colliderEnabled == false) { attackBlockedByPlayer = false; }
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
            _enemySoundSource.PlayOneShot(_enemEffects.attackImpactSound);
            GameManager.Instance.TriggerSlowMotion();
            PlayerManager.Instance.DoDamage(damage);
            Instantiate(_enemEffects.attackImpactFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }
}
