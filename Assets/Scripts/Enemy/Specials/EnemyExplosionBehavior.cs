using UnityEngine;

public class EnemyExplosionBehavior : MonoBehaviour
{
    public float explosionDamage; //O dano vem ProjectileBehavior, que vem da SecondaryBehavior

    private Collider coll;

    private PlayerStatus playerStatus;
    private void Start()
    {
        coll = GetComponent<Collider>();
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();

        Destroy(coll, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.DoDamage(explosionDamage);
        }
    }
}
