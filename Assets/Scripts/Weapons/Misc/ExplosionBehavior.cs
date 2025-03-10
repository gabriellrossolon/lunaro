using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    public float explosionDamage; //O dano vem ProjectileBehavior, que vem da SecondaryBehavior

    private PlayerStatus playerStatus;
    private Collider coll;
    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        coll =  GetComponent<Collider>();
        Destroy(coll, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemyFSM = other.GetComponent<EnemyFSM>();
            var enemyAnim = other.GetComponent<Animator>();
            var enemyFX = other.GetComponent<EnemyEffects>();

            if (enemyAnim != null)
                enemyAnim.SetTrigger("hurt");

            if (enemyFSM != null)
                enemyFSM.DoDamage(explosionDamage + playerStatus.damage);

            if (enemyFX != null)
            {
                GameObject collParticle = Instantiate(enemyFX.attackTakenParticle, transform.position, transform.rotation);
                PlayParticleSound(collParticle, enemyFX.attackTakenSound);
            }
        }
    }

    private void PlayParticleSound(GameObject particle, AudioClip sound)
    {
        var audioSource = particle.GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.PlayOneShot(sound);
    }
}
