using UnityEngine;

public class EnemyWeaponBehavior : MonoBehaviour
{
    public float damage;
    private MeshCollider _coll;
    private SphereCollider _sphColl;
    private EnemyEffects _enemEffects;
    private EnemyStats _enemyStats;
    private AudioSource _enemySoundSource;
    public bool colliderEnabled = false;

    public bool miscWeapon = false;
    private void Awake()
    {
        _coll = GetComponent<MeshCollider>();
        _sphColl = GetComponent<SphereCollider>();
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
        if (miscWeapon)
        {
            colliderEnabled = !colliderEnabled;
            if (_coll != null) { _coll.enabled = colliderEnabled; }
            if (_sphColl != null) { _sphColl.enabled = colliderEnabled; }
        }
        else
        {
            colliderEnabled = !colliderEnabled;
            if (_coll != null) { _coll.enabled = colliderEnabled; }
            if (_sphColl != null) { _sphColl.enabled = colliderEnabled; }
        }
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
