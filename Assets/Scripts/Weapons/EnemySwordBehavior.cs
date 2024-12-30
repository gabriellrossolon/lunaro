using UnityEngine;

public class EnemySwordBehavior : MonoBehaviour
{
    public float damage;
    private MeshCollider _coll;
    private EnemyStats _enemStats;
    public bool colliderEnabled = false;
    private void Awake()
    {
        _coll = GetComponent<MeshCollider>();
        _enemStats = GetComponentInParent<EnemyStats>();
    }

    private void Start()
    {
        damage = _enemStats.attackDamage;
    }

    private void Update()
    {
        _coll.enabled = colliderEnabled;
    }

    public void EnableCollider()
    {
        colliderEnabled = !colliderEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.TriggerSlowMotion();
            other.GetComponent<PlayerStatus>().DoDamage(damage);
            Instantiate(_enemStats.impactFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }
}
