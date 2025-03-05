using UnityEngine;
using System.Collections;
public class AirCutBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private Vector3 direction;
    private Rigidbody rb;

    [Header("Projectile Commons")]
    public float speed = 10f;
    public AudioClip projectileImpactSound;
    public GameObject collisionParticle;
    public float airCutDamage;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        LaunchProjectile();

        Destroy(gameObject, 2f);
    }

    public void SetDirection(Vector3 lookDirection)
    {
        direction = lookDirection.normalized;
    }

    private void LaunchProjectile()
    {
        rb.useGravity = false;
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            HandleEnemyCollision(other);
        }
        if (!other.CompareTag("Player"))
        {
            HandleAnyCollision(other);
        }
    }

    private void HandleEnemyCollision(Collider enemy)
    {
        var enemyFSM = enemy.GetComponent<EnemyFSM>();
        var enemyAnim = enemy.GetComponent<Animator>();
        var enemyFX = enemy.GetComponent<EnemyEffects>();

        if (enemyAnim != null)
            enemyAnim.SetTrigger("hurt");

        if (enemyFSM != null)
            enemyFSM.DoDamage(airCutDamage + playerStatus.damage);

        if (enemyFX != null)
        {
            GameObject collParticle = Instantiate(enemyFX.attackTakenParticle, transform.position, transform.rotation);
            PlayParticleSound(collParticle, enemyFX.attackTakenSound);
        }

        GameManager.Instance.TriggerSlowMotion();
    }

    private void HandleAnyCollision(Collider other)
    {
        var enemyFX = other.GetComponent<EnemyEffects>();
        GameObject collParticle;

        collParticle = Instantiate(collisionParticle, transform.position, transform.rotation);
        PlayParticleSound(collParticle, projectileImpactSound);

        Destroy(gameObject);
    }

    private void PlayParticleSound(GameObject particle, AudioClip sound)
    {
        var audioSource = particle.GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.PlayOneShot(sound);
    }
}
