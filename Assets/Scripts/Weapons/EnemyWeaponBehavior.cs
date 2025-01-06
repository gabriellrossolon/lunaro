using UnityEngine;

public class EnemyWeaponBehavior : MonoBehaviour
{
    public float damage;
    private Collider _coll;
    private EnemyEffects _enemEffects;
    private EnemyStats _enemyStats;
    public bool colliderEnabled = false;
    private void Awake()
    {
        _coll = GetComponent<Collider>();
        _enemEffects = GetComponentInParent<EnemyEffects>();
        _enemyStats = GetComponentInParent<EnemyStats>();
    }

    private void Start()
    {
        damage = _enemyStats.attackDamage;
    }

    public void CallCollider()
    {
        colliderEnabled = !colliderEnabled;
        _coll.enabled = colliderEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.TriggerSlowMotion();
            PlayerManager.Instance.DoDamage(damage);
            Instantiate(_enemEffects.attackImpactFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }
}
