using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float speed = 10f;
    public AudioClip projectileImpactSound;

    private Vector3 direction;
    private Rigidbody rb;
    public GameObject collisionParticle;

    public enum ProjectileType { Nothing, Magic, Arrow }
    public ProjectileType projectileType;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector3 lookDirection)
    {
        direction = lookDirection.normalized;
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();

        if (projectileType == ProjectileType.Magic)
        {
            rb.useGravity = false; 
            rb.linearVelocity = direction * speed; 
        }
        else if (projectileType == ProjectileType.Arrow)
        {
            rb.useGravity = true; 
            LaunchArrow(); 
        }

        Destroy(gameObject, 5f); // Destroi o projétil após 5 segundos
    }

    private void LaunchArrow()
    {
        rb.AddForce(direction * speed, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyFSM enemyFSM = other.GetComponent<EnemyFSM>();
            Animator enemyAnim = other.GetComponent<Animator>();

            if (enemyAnim != null)
                enemyAnim.SetTrigger("hurt");

            if (enemyFSM != null)
                enemyFSM.DoDamage(playerStatus.damage);

            GameManager.Instance.TriggerSlowMotion();
        }

        if (!other.CompareTag("Player"))
        {
            EnemyEffects enemyFX = other.GetComponent<EnemyEffects>();

            if (projectileType == ProjectileType.Magic)
            {
                GameObject collParticle = Instantiate(collisionParticle, transform.position, transform.rotation);
                collParticle.GetComponent<AudioSource>().PlayOneShot(projectileImpactSound);
            }
            else if (projectileType == ProjectileType.Arrow || projectileType == ProjectileType.Nothing)
            {
                if (other.CompareTag("Enemy"))
                {
                    GameObject collParticle = Instantiate(enemyFX.attackTakenParticle, transform.position, transform.rotation);
                    collParticle.GetComponent<AudioSource>().PlayOneShot(enemyFX.attackTakenSound);
                }
                else
                {
                    GameObject collParticle = Instantiate(collisionParticle, transform.position, transform.rotation);
                    collParticle.GetComponent<AudioSource>().PlayOneShot(projectileImpactSound);
                }
            }

            Destroy(this.gameObject);
        }
    }
}
