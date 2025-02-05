using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private Vector3 direction;
    private Rigidbody rb;
    public enum ProjectileType { Nothing, Magic, Arrow, Bolide }


    [Header("Projectile Commons")]
    public ProjectileType projectileType;
    public float speed = 10f;
    public AudioClip projectileImpactSound;
    public GameObject collisionParticle;

    [Header("Only if Bolide")]
    public GameObject bolideExplosion;

    public float projectileDamage; //Dano do projetil vem da arma que o chamou
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();

        if (projectileType == ProjectileType.Magic)
            LaunchProjectileWithoutGravity();
        else if (projectileType == ProjectileType.Arrow || projectileType == ProjectileType.Bolide)
            LaunchProjectileWithGravity();

        Destroy(gameObject, 5f); 
    }

    private void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f) // Garante que só rotaciona se estiver em movimento
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
        }
    }

    public void SetDirection(Vector3 lookDirection)
    {
        direction = lookDirection.normalized;
    }

    private void LaunchProjectileWithoutGravity()
    {
        rb.useGravity = false;
        rb.linearVelocity = direction * speed;
    }
    private void LaunchProjectileWithGravity()
    {
        rb.useGravity = true;
        rb.AddForce(direction * speed, ForceMode.VelocityChange);
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

        if (projectileType != ProjectileType.Bolide) //Caso for Bolide, quem da o dano é a explosão, não o projetil em si!
        {
            if (enemyAnim != null)
                enemyAnim.SetTrigger("hurt");

            if (enemyFSM != null)
                enemyFSM.DoDamage(projectileDamage + playerStatus.damage);

            if (enemyFX != null)
            {
                GameObject collParticle = Instantiate(enemyFX.attackTakenParticle, transform.position, transform.rotation);
                PlayParticleSound(collParticle, enemyFX.attackTakenSound);
            }

            GameManager.Instance.TriggerSlowMotion();
        }
    }

    private void HandleAnyCollision(Collider other)
    {
        var enemyFX = other.GetComponent<EnemyEffects>();
        GameObject collParticle;

        if (projectileType == ProjectileType.Magic)
        {
            collParticle = Instantiate(collisionParticle, transform.position, transform.rotation);
            PlayParticleSound(collParticle, projectileImpactSound);

            Destroy(gameObject);
        }
        else if (projectileType == ProjectileType.Arrow)
        {
            collParticle = Instantiate(collisionParticle, transform.position, transform.rotation);
            PlayParticleSound(collParticle, projectileImpactSound);

            Destroy(gameObject);
        }
        else if (projectileType == ProjectileType.Bolide && !other.CompareTag("Enemy")) //Não deve colidir com inimigo, apenas com o chão, para que a explosão ocorra no chão
        {
            collParticle = Instantiate(bolideExplosion, transform.position, Quaternion.identity);
            collParticle.GetComponent<ExplosionBehavior>().explosionDamage = projectileDamage;
            PlayParticleSound(collParticle, projectileImpactSound);
            Destroy(gameObject);
        }  
    }

    private void PlayParticleSound(GameObject particle, AudioClip sound)
    {
        var audioSource = particle.GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.PlayOneShot(sound);
    }
}
