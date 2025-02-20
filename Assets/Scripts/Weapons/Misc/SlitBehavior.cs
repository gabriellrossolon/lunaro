using UnityEngine;

public class SlitBehavior : MonoBehaviour
{
    public float slitDamage;
    private PlayerStatus playerStatus;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            HandleEnemyCollision(other);
        }
    }

    private void HandleEnemyCollision(GameObject enemy)
    {
        var enemyFSM = enemy.GetComponent<EnemyFSM>();
        var enemyAnim = enemy.GetComponent<Animator>();
        var enemyFX = enemy.GetComponent<EnemyEffects>();

        if (enemyAnim != null)
            enemyAnim.SetTrigger("hurt");

        if (enemyFSM != null)
            enemyFSM.DoDamage(slitDamage + playerStatus.damage);

        if (enemyFX != null)
        {
            GameObject collParticle = Instantiate(enemyFX.attackTakenParticle, enemy.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            PlayParticleSound(collParticle, enemyFX.attackTakenSound);
        }

        GameManager.Instance.TriggerSlowMotion();
    }

    private void PlayParticleSound(GameObject particle, AudioClip sound)
    {
        var audioSource = particle.GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.PlayOneShot(sound);
    }
}
