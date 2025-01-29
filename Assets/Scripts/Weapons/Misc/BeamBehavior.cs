using System.Collections;
using UnityEngine;

public class BeamBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private Collider beamCollider;
    public float damageInterval = 1.0f;
    public float damage;

    public AudioClip beamImpactSound;
    public GameObject beamImpactParticle;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        beamCollider = GetComponent<Collider>();
        StartCoroutine(ActivateBeamDamage());
    }

    private IEnumerator ActivateBeamDamage()
    {
        while (true)
        {
            beamCollider.enabled = true;
            yield return new WaitForSeconds(damageInterval / 2);
            beamCollider.enabled = false;
            yield return new WaitForSeconds(damageInterval / 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyFSM enemyFSM = other.GetComponent<EnemyFSM>();
            if (enemyFSM != null) { enemyFSM.DoDamage((damage + playerStatus.damage) / 5); }
        }
        if (!other.CompareTag("Player"))
        {
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            GameObject collParticle = Instantiate(beamImpactParticle, collisionPoint, Quaternion.identity);

            collParticle.GetComponent<AudioSource>().PlayOneShot(beamImpactSound);
        }
    }
}
