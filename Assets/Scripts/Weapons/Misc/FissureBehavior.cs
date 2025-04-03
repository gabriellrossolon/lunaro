using UnityEngine;

public class FissureBehavior : MonoBehaviour
{
    public float fissureDamage;
    private Collider coll;

    private void Start()
    {
        coll = GetComponent<Collider>();
        Destroy(coll, 0.5f);
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            HandleEnemyCollision(other);
        }

        Debug.Log("Colidiu com: " + other.name);
    }

    private void HandleEnemyCollision(GameObject enemy)
    {
        var enemyFSM = enemy.GetComponent<EnemyFSM>();
        var enemyAnim = enemy.GetComponent<Animator>();
        var enemyFX = enemy.GetComponent<EnemyEffects>();

        if (enemyAnim != null)
            enemyAnim.SetTrigger("hurt");

        if (enemyFSM != null)
            enemyFSM.DoDamage(fissureDamage + PlayerComponents.Instance.playerStatus.damage);

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
