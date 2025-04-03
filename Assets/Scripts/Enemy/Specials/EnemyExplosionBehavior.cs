using UnityEngine;

public class EnemyExplosionBehavior : MonoBehaviour
{
    public float explosionDamage; //O dano vem ProjectileBehavior, que vem da SecondaryBehavior

    private Collider coll;

    private void Start()
    {
        coll = GetComponent<Collider>();

        Destroy(coll, 0.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerComponents.Instance.playerManager.DoDamage(explosionDamage);
        }
    }
}
