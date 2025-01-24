using UnityEngine;

public class EnemyProjectileBehavior : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    private bool attackBlockedByPlayer = false;

    private Vector3 direction;
    private Rigidbody rb;
    [HideInInspector] public EnemyWeaponBehavior weaponBehavior;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Vector3 playerPosition = PlayerManager.Instance.transform.position;
        playerPosition += Vector3.up * 0.5f;

        direction = (playerPosition - transform.position).normalized;

        Destroy(gameObject, 3f);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attackBlockedByPlayer && other.gameObject.CompareTag("Shield"))
        {
            weaponBehavior._enemySoundSource.PlayOneShot(other.GetComponent<ShieldBehavior>().blockedAttackSound);
            Instantiate(weaponBehavior._enemEffects.attackImpactFx, other.transform.position, Quaternion.identity);
            other.GetComponent<ShieldBehavior>().ConsumeStamina();
            attackBlockedByPlayer = true;
        }
        else if (!attackBlockedByPlayer && other.gameObject.CompareTag("Player"))
        {
            weaponBehavior._enemySoundSource.PlayOneShot(weaponBehavior._enemEffects.attackImpactSound);
            PlayerManager.Instance.DoDamage(damage);
            Instantiate(weaponBehavior._enemEffects.attackImpactFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }
}
