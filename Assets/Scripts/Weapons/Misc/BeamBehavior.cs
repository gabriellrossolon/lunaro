using UnityEngine;
using System.Collections;

public class BeamBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    public float damageInterval = 1.0f;
    public float damage;

    public AudioClip beamImpactSound;
    public GameObject beamImpactParticle;

    private bool canDoDamage = true;
    private float damageTimer = 0f;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        if (!canDoDamage)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                canDoDamage = true;
                damageTimer = 0f; 
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy") && canDoDamage)
        {
            EnemyFSM enemyFSM = other.GetComponent<EnemyFSM>();
            if (enemyFSM != null)
            {
                enemyFSM.DoDamage((damage + playerStatus.damage) / 5);
            }

            Vector3 collisionPoint = other.transform.position;
            GameObject collParticle = Instantiate(beamImpactParticle, collisionPoint, Quaternion.identity);

            AudioSource audioSource = collParticle.GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.PlayOneShot(beamImpactSound);

            canDoDamage = false;
        }
    }
}
