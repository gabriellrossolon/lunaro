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
        Vector3 playerPosition = PlayerComponents.Instance.playerObject.transform.position;
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
            weaponBehavior._enemySoundSource.PlayOneShot(other.GetComponent<ShieldBehavior>().blockedAttackSound); //Toca o audio de bloqueio do escudo
            Instantiate(weaponBehavior._enemEffects.attackImpactFx, other.transform.position, Quaternion.identity); //Instancia a particula de colisão com o escudo
            other.GetComponent<ShieldBehavior>().ConsumeStamina(); //Consome a Stamina do jogador (Stamina por bloquear)
            attackBlockedByPlayer = true; // Marca o ataque como bloqueado
        }
        if (other.gameObject.CompareTag("Player"))
        {
            if (attackBlockedByPlayer)
            {
                PlayerComponents.Instance.playerManager.DoDamage(damage * (1f - (other.GetComponentInChildren<ShieldBehavior>().blockPercent / 100f)));
                Destroy(this.gameObject);
            }
            else
            {
                PlayerComponents.Instance.playerManager.DoDamage(damage);
                weaponBehavior._enemySoundSource.PlayOneShot(weaponBehavior._enemEffects.attackImpactSound);
                Instantiate(weaponBehavior._enemEffects.attackImpactFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                Destroy(this.gameObject);
            }
        }

        if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Shield"))
        {
            Destroy(this.gameObject);
            Debug.Log("Destruiu na parede");
            weaponBehavior._enemySoundSource.PlayOneShot(weaponBehavior._enemEffects.attackImpactSound);
            Instantiate(weaponBehavior._enemEffects.attackImpactFx, other.transform.position, Quaternion.identity);
        }
    }
}

