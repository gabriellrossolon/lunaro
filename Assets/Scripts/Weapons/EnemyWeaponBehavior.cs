using UnityEngine;

public class EnemyWeaponBehavior : MonoBehaviour
{
    public float damage;
    private MeshCollider _coll;
    private EnemyEffects _enemEffects;
    private EnemyStats _enemyStats;
    private AudioSource _enemySoundSource;
    public bool colliderEnabled = false;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _enemySoundSource.PlayOneShot(_enemEffects.attackImpactSound);
            GameManager.Instance.TriggerSlowMotion();
            PlayerManager.Instance.DoDamage(damage);
            Instantiate(_enemEffects.attackImpactFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }
}
